using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    // Translates data to and from JSON format.

    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData load()
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullpath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad =  reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Something went wrong when trying to load data from file: " + e);
            }
        }
        return loadedData;
    }

    public void save(GameData data)
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Something went wrong when trying to save data to file: " + e);
        }
    }

    public bool delete()
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullpath))
        {
            try
            {
                File.Delete(fullpath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Something went wrong when trying to delete save file: " + e);
            }
        }
        return true;
    }

    public bool fileExists()
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullpath))
        {
            return true;
        }
        return false;
    }
}
