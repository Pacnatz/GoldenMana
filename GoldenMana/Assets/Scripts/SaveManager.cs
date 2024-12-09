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
            HealthPickupPos = new();
            EnemyKillPos = new();
        }
        public string SceneName;
        public List<Vector2> OpenedChestPos;
        public List<Vector3Int> BrokenCellPos;
        public List<Vector2> HealthPickupPos;
        public List<Vector2> EnemyKillPos;
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
        yield return new WaitForSeconds(.02f);

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
        if (SceneManager.GetActiveScene().name != "CaveLevel3") { // Not the minotaur fight scene
            GameInput.Instance.UnPausePlayer();
        }
        else {
            // If the minotaur has already been killed
            if (Player.Instance.minotaurKilled) {
                GameInput.Instance.UnPausePlayer();
            }

        }
    }
    private void ShowSceneData() {
        // Debugging purposes
    }


    // ############################################################################## SAVE AND LOAD SYSTEM
    public void Save(Vector2 savePos) {
        // Save data here
        SaveObject saveObject = new SaveObject {
            Health = Player.Instance.health,
            MaxHealth = Player.Instance.maxHealth,
            Mana = Player.Instance.mana,
            ManaLevel = Player.Instance.manaLevel,
            FireballUnlocked = Player.Instance.fireballUnlocked,
            MinotaurKilled = Player.Instance.minotaurKilled,
            SavePos = savePos,
            SceneList = sceneList,
            LastScene = selectedScene
            
        };

        // Encrypt save data
        string json = JsonUtility.ToJson(saveObject);

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

    }

    public void Load() {
        if (File.Exists(Application.dataPath + SAVE_PATH)){//&& File.Exists(Application.dataPath + KEY_PATH)) {

            // Load Key Data
            string keyJson = File.ReadAllText(Application.dataPath + KEY_PATH);
            KeyObject keyObject = JsonUtility.FromJson<KeyObject>(keyJson);
            // Load Save Data
            string encryptedSaveJson = File.ReadAllText(Application.dataPath + SAVE_PATH);
            string json = DecryptDataWithAes(encryptedSaveJson, keyObject.keyBase64, keyObject.vectorBase64);
            
            saveObject = JsonUtility.FromJson<SaveObject>(json);


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
        Player.Instance.health = saveObject.Health;
        Player.Instance.maxHealth = saveObject.MaxHealth;
        Player.Instance.mana = saveObject.Mana;
        Player.Instance.manaLevel = saveObject.ManaLevel;
        PlayerMove.Instance.gameObject.transform.position = saveObject.SavePos;
        if (saveObject.FireballUnlocked) {
            Player.Instance.UnlockFireSpell();
        }
        Player.Instance.minotaurKilled = saveObject.MinotaurKilled;
    }

    public void LoadSceneFromDoor(string sceneName, Vector2 loadPos) {
        // Need to call coroutine from this function or else won't work
        StartCoroutine(LoadPlayerFromScene(sceneName, loadPos));
    }

    private IEnumerator LoadPlayerFromScene(string sceneName, Vector2 loadPos) {
        // Saving before scene change
        int playerHealth = Player.Instance.health;
        int playerMaxHealth = Player.Instance.maxHealth;
        int playerManaLevel = Player.Instance.manaLevel;
        int playerMana = Player.Instance.mana;
        bool fireballUnlocked = Player.Instance.fireballUnlocked;
        bool minotaurKilled = Player.Instance.minotaurKilled;
        SceneManager.LoadScene(sceneName);
        float sceneDelay = .1f;
        yield return new WaitForSeconds(sceneDelay);
        // Loading after scene change
        Player.Instance.health = playerHealth;
        Player.Instance.maxHealth = playerMaxHealth;
        Player.Instance.fireballUnlocked = fireballUnlocked;
        Player.Instance.minotaurKilled = minotaurKilled;
        Player.Instance.manaLevel = playerManaLevel;
        Player.Instance.mana = playerMana;

        if (PlayerMove.Instance) {
            PlayerMove.Instance.gameObject.transform.position = loadPos;
        }


        
    }

    // Save data class
    public class SaveObject {
        // Player saving
        public int Health;
        public int MaxHealth;
        public int Mana;
        public int ManaLevel;
        public bool FireballUnlocked;
        public bool MinotaurKilled;
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

    public void DeleteSaveFile() {
        if (File.Exists(Application.dataPath + SAVE_PATH) && File.Exists(Application.dataPath + KEY_PATH)) {
            File.Delete(Application.dataPath + SAVE_PATH);
            File.Delete(Application.dataPath + KEY_PATH);
        }
        else {
            Debug.LogWarning("Key not found");
        }
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


