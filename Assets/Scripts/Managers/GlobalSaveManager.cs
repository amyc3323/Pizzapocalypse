using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GlobalSaveManager : MonoBehaviour
{
    public static GlobalSaveManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            }
        Debug.Log("Path: " + GetFilePath("test"));
    }
    public void SaveData(string name, SaveData data)
    {
        WriteToFile(name, data.ToJson());
    }
    public void ClearData(string name)
    {
        WriteToFile(name, "");
    }
    public bool LoadData(string name, out string data)
    {
        string json = ReadFromFIle(name);
        Debug.Log("Loaded data: " + json);
        data = json;
        if (json == "Success") return false;
        return true;
    }
    private string ReadFromFIle(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return json;
            /*using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }*/
        }
        else
        {
            Debug.LogWarning("File not found");
        }

        return "Success";
    }
    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        //FileStream fileStream = new FileStream(path, FileMode.Create);
        Debug.Log($"Path: {path} Json: {json}");
        File.WriteAllText(path,json);
        /*using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }*/
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName+".txt";
    }
}
