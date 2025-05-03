using System;
using System.IO;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace SavePack
{
    public static class FileManager
    {
        private static readonly string SAVE_DIR_PATH = Path.Combine(Application.persistentDataPath, "playerSave");
        private static readonly string SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "playerSave/baseData.txt");
        public static SaveManager.PlayerSaveData GetPlayerData()
        {
            if (!File.Exists(SAVE_FILE_PATH)) return new SaveManager.PlayerSaveData();

            try
            {
                using var fileStream = new FileStream(SAVE_FILE_PATH, FileMode.Open);
                using var reader = new StreamReader(fileStream);
                return JsonUtility.FromJson<SaveManager.PlayerSaveData>(reader.ReadToEnd());
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }

        public static void SavePlayerData(SaveManager.PlayerSaveData saveData)
        {
            try
            {
                Directory.CreateDirectory(SAVE_DIR_PATH);
                var data = JsonUtility.ToJson(saveData, true);
                using var fileStream = new FileStream(SAVE_FILE_PATH, FileMode.Create);
                using var writer = new StreamWriter(fileStream);
                writer.Write(data);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }
        
        public static void DeletePlayerData()
        {
            try
            {
                if (File.Exists(SAVE_FILE_PATH))
                {
                    File.Delete(SAVE_FILE_PATH);
                    UnityEngine.Debug.Log("Deleted file: " + SAVE_FILE_PATH);
                    return;
                }
                UnityEngine.Debug.LogWarning("No save file found in: " + SAVE_FILE_PATH);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
        }
    }
}