using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance { get; private set; }

    [HideInInspector] public List<SceneData> sceneList = new List<SceneData>();
    [HideInInspector] public SceneData selectedScene;

    private SaveObject saveObject;

    // Scene data class
    [System.Serializable]
    public class SceneData{
        public SceneData(string sceneName) {
            SceneName = sceneName;
            OpenedChestPos = new();
            BrokenCellPos = new();
            PlayerLoadPos = Vector2.zero;
        }
        public string SceneName;
        public List<Vector2> OpenedChestPos;
        public List<Vector3Int> BrokenCellPos;
        public Vector2 PlayerLoadPos;
    }


    // Save paths
    private const string SAVE_PATH = "/save.txt";
    private const string KEY_PATH = "/key.txt";

    private void Awake() { 
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void Start() { 
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        // Runs for the first scene when entering playmode
        SceneManager_sceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


    // Testing
    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            Load();
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            SceneManager.LoadScene("CaveLevel1");
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            ShowSceneData();
        }
    }

    // ############################################################################## SCENE DATA SYSTEM
    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode sceneMode) {
        bool sceneFound = false;
        // Select our scene
        foreach (SceneData scene in sceneList) {
            if (scene.SceneName == loadedScene.name) {
                sceneFound = true;
                selectedScene = scene;
            }
        }
        // If haven't found scene in list, add it and select that scene
        if (!sceneFound) {
            SceneData newScene = new SceneData(loadedScene.name);
            sceneList.Add(newScene);
            selectedScene = newScene;
        }
        // Delay then load scene data
        StartCoroutine(LoadSceneData());


    }



    private IEnumerator LoadSceneData() {
        yield return new WaitForSeconds(.1f);
        if (GameManager.Instance) {
            if (GameManager.Instance.FireballUnlocked) {
                Player.Instance.UnlockFireSpell();
            }
        }
        

        // Pausing player input
        if (GameInput.Instance) {
            GameInput.Instance.PausePlayer();
            StartCoroutine(OnTransitionFinished(.5f));
        }
        
        if (SceneTransitions.Instance) {
            SceneTransitions.Instance.StartScene();
        }
        
        
    }

    private IEnumerator OnTransitionFinished(float sceneDelay) {
        yield return new WaitForSeconds(sceneDelay);
        GameInput.Instance.UnPausePlayer();
    }
    private void ShowSceneData() {
        // Debugging purposes
    }

    public void SetPlayerPosition(Vector2 playerLoadPos) {
        Player.Instance.transform.position = playerLoadPos;
    }

    // ############################################################################## SAVE AND LOAD SYSTEM
    public void Save(Vector2 savePos) {
        // Save data here
        SaveObject saveObject = new SaveObject {
            Health = 10,
            FireballUnlocked = Player.Instance.fireballUnlocked,
            SavePos = savePos,
            SceneList = sceneList,
            LastScene = selectedScene
            
        };

        // Encrypt save data
        string json = JsonUtility.ToJson(saveObject);
        /*
        string encryptedSaveJson = EncryptDataWithAes(json, out string keyBase64, out string vectorBase64);

        // Write key data
        KeyObject keyObject = new KeyObject();
        keyObject.keyBase64 = keyBase64;
        keyObject.vectorBase64 = vectorBase64;

        string keyJson = JsonUtility.ToJson(keyObject);

        // Save game data to txt file
        File.WriteAllText(Application.dataPath + SAVE_PATH, encryptedSaveJson);

        // Save key data to txt file
        File.WriteAllText(Application.dataPath + KEY_PATH, keyJson);
        */
        File.WriteAllText(Application.dataPath + SAVE_PATH, json);

    }

    public void Load() {
        if (File.Exists(Application.dataPath + SAVE_PATH)){//&& File.Exists(Application.dataPath + KEY_PATH)) {
            /*
            // Load Key Data
            string keyJson = File.ReadAllText(Application.dataPath + KEY_PATH);
            KeyObject keyObject = JsonUtility.FromJson<KeyObject>(keyJson);
            // Load Save Data
            */
            string encryptedSaveJson = File.ReadAllText(Application.dataPath + SAVE_PATH);
            /*
            string json = DecryptDataWithAes(encryptedSaveJson, keyObject.keyBase64, keyObject.vectorBase64);
            
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);
            */
            saveObject = JsonUtility.FromJson<SaveObject>(encryptedSaveJson);
            // Load data here
            // health = saveObject.health;

            StartCoroutine(LoadPlayerFromSave());
        }
        else {
            Debug.LogWarning("Key not found");
        }
        
    }

    private IEnumerator LoadPlayerFromSave() {
        // Scene Transition
        if (SceneTransitions.Instance) {
            SceneTransitions.Instance.SetBlack();
        }
        // Scene loading
        sceneList = saveObject.SceneList;
        selectedScene = saveObject.LastScene;
        
        SceneManager.LoadScene(selectedScene.SceneName);
        float sceneDelay = .1f;
        yield return new WaitForSeconds(sceneDelay);
        // Player loading
        PlayerMove.Instance.gameObject.transform.position = saveObject.SavePos;
        if (saveObject.FireballUnlocked) {
            Player.Instance.UnlockFireSpell();
        }
    }

    public void LoadSceneFromDoor(string sceneName, Vector2 loadPos) {
        // Need to call coroutine from this function or else won't work
        StartCoroutine(LoadPlayerFromScene(sceneName, loadPos));
    }

    private IEnumerator LoadPlayerFromScene(string sceneName, Vector2 loadPos) {
        SceneManager.LoadScene(sceneName);
        float sceneDelay = .1f;
        yield return new WaitForSeconds(sceneDelay);
        // Player loading
        PlayerMove.Instance.gameObject.transform.position = loadPos;
        if (saveObject != null) {
            if (saveObject.FireballUnlocked) {
                Player.Instance.UnlockFireSpell();
            }
        }
        
    }

    // Save data class
    public class SaveObject {
        // Player saving
        public float Health;
        public bool FireballUnlocked;
        public Vector2 SavePos;
        // Scene saving
        public List<SceneData> SceneList;
        public SceneData LastScene;
    }
    // Class to store keys
    public class KeyObject {
        public string keyBase64;
        public string vectorBase64;
    }



    // ############################################################################## SAVE AND LOAD SYSTEM



    // ############################################################################## AES ENCRYPTION FROM https://www.siakabaro.com/how-to-perform-aes-encryption-in-net/
    private static string EncryptDataWithAes(string plainText, out string keyBase64, out string vectorBase64) {
        using (Aes aesAlgorithm = Aes.Create()) {
            //set the parameters with out keyword
            keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
            vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

            // Create encryptor object
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter sw = new StreamWriter(cs)) {
                        sw.Write(plainText);
                    }
                    encryptedData = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }
    }




    private static string DecryptDataWithAes(string cipherText, string keyBase64, string vectorBase64) {
        using (Aes aesAlgorithm = Aes.Create()) {
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

            // Create decryptor object
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

            byte[] cipher = Convert.FromBase64String(cipherText);

            //Decryption will be done in a memory stream through a CryptoStream object
            using (MemoryStream ms = new MemoryStream(cipher)) {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader sr = new StreamReader(cs)) {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
    // ############################################################################## AES ENCRYPTION FROM https://www.siakabaro.com/how-to-perform-aes-encryption-in-net/

}


