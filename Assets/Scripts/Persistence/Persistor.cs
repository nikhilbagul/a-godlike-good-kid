using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GUIDAlphabet
{
    public static char[] alphabet = {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };
}

[System.Serializable]
public class SaveData
{    
    [SerializeField]
    private string GUID;
    public string GetGUID { get { return GUID; } }
    public string levelName;

    public SaveData()
    {
        GUID = "NOT_INITIALISED";
        levelName = "NULL";
    }

    public void GenerateGUID()
    {
        string newGUID = "";
        for (int i = 0; i < 8; ++i)
        {
            char next = GUIDAlphabet.alphabet[Random.Range(0, GUIDAlphabet.alphabet.Length)];
            newGUID += next;
        }

        Debug.Log("Generated GUID = " + newGUID);
        GUID = newGUID;
    }

    public override string ToString()
    {
        return "GUID: " + GUID + ", scene: " + levelName;
    }
}

public static class Persistor
{
    static string fileName = "data.sav";
    static string prefsKey = "save_data";

    public static SaveData Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return LoadWebGL();
#else
        return LoadFile();
#endif
    }

    public static void Save(SaveData saveData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveWebGL(saveData);
#else
        SaveFile(saveData);
#endif
    }

    public static void Delete()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.DeleteKey(prefsKey);
        PlayerPrefs.Save();
#else
        string path = GetPath();
        if (File.Exists(path))
            File.Delete(path);
#endif
    }

    // --- WebGL: PlayerPrefs with JSON ---

    static SaveData LoadWebGL()
    {
        if (PlayerPrefs.HasKey(prefsKey))
        {
            string json = PlayerPrefs.GetString(prefsKey);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();
    }

    static void SaveWebGL(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(prefsKey, json);
        PlayerPrefs.Save();
        if (Debugger.LogMessage != null)
            Debugger.LogMessage(saveData + " saved.");
    }

    // --- Standalone/Editor: BinaryFormatter + FileStream ---

    static string GetPath()
    {
        string path = "";
#if !UNITY_EDITOR
        path = Application.persistentDataPath + "/";
#endif
        return path + fileName;
    }

    static SaveData LoadFile()
    {
        string path = GetPath();
        SaveData saveData = new SaveData();

        if (File.Exists(path))
        {
            FileStream saveFile = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            saveData = (SaveData)formatter.Deserialize(saveFile);
            saveFile.Close();
        }
        return saveData;
    }

    static void SaveFile(SaveData saveData)
    {
        string path = GetPath();
        FileStream saveFile = new FileStream(path, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(saveFile, saveData);
            if (Debugger.LogMessage != null)
                Debugger.LogMessage(saveData + " saved.");
        }        
        catch (System.Exception e)
        {
            Debug.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            saveFile.Close();
        }
    }

    public static void ObliterateCheckpoint()
    {
        SaveData save = Load();
        save.levelName = "NULL";
        Save(save);
    }
}