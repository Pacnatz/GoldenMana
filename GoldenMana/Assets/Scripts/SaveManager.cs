using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance { get; private set; }

    private const string SAVE_PATH = "/save.txt";
    private const string KEY_PATH = "/key.txt";

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // ############################################################################## SAVE AND LOAD SYSTEM
    public void Save() {
        // Save data here
        SaveObject saveObject = new SaveObject {
            health = 10,
            scene = "CaveLevel1"
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
        if (File.Exists(Application.dataPath + SAVE_PATH) && File.Exists(Application.dataPath + KEY_PATH)) {
            // Load Key Data
            string keyJson = File.ReadAllText(Application.dataPath + KEY_PATH);
            KeyObject keyObject = JsonUtility.FromJson<KeyObject>(keyJson);
            // Load Save Data
            string cipherJson = File.ReadAllText(Application.dataPath + SAVE_PATH);
            string json = DecryptDataWithAes(cipherJson, keyObject.keyBase64, keyObject.vectorBase64);

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

            // Save data here
            // health = saveObject.health;
        }
        else {
            Debug.LogWarning("Key not found");
        }
        
    }


    // Save data class
    public class SaveObject {
        public float health;
        public string scene;
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
            Console.WriteLine($"Aes Cipher Mode : {aesAlgorithm.Mode}");
            Console.WriteLine($"Aes Padding Mode: {aesAlgorithm.Padding}");
            Console.WriteLine($"Aes Key Size : {aesAlgorithm.KeySize}");
            Console.WriteLine($"Aes Block Size : {aesAlgorithm.BlockSize}");

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

            Console.WriteLine($"Aes Cipher Mode : {aesAlgorithm.Mode}");
            Console.WriteLine($"Aes Padding Mode: {aesAlgorithm.Padding}");
            Console.WriteLine($"Aes Key Size : {aesAlgorithm.KeySize}");
            Console.WriteLine($"Aes Block Size : {aesAlgorithm.BlockSize}");

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


