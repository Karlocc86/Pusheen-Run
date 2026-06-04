using UnityEngine;
using System.IO;

// sistema de guardado que persiste el highscore en un archivo JSON en disco
// la carpeta se guarda en Application.persistentDataPath (carpeta de datos del juego)
[System.Serializable]
public class GameSaveData
{
    public int highscore;
    public string lastPlayDate;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string _savePath;
    private const string SaveFileName = "gamesave.json";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // persistentDataPath es la carpeta donde unity permite guardar datos entre sesiones
        // en Windows: C:\Users\[usuario]\AppData\LocalLow\[CompanyName]\[ProductName]
        _savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    // carga el highscore desde el JSON; si no existe devuelve 0
    public int LoadHighscore()
    {
        if (!File.Exists(_savePath))
        {
            return 0;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log($"Highscore cargado desde JSON: {data.highscore}");
            return data.highscore;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar save: {e.Message}");
            return 0;
        }
    }

    // guarda el highscore en JSON con timestamp
    public void SaveHighscore(int highscore)
    {
        GameSaveData data = new GameSaveData
        {
            highscore = highscore,
            lastPlayDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            string json = JsonUtility.ToJson(data, true); // true = pretty print para legibilidad
            File.WriteAllText(_savePath, json);
            Debug.Log($"Highscore guardado en JSON: {_savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar: {e.Message}");
        }
    }

    // retorna la ruta del archivo para debugging
    public string GetSaveFilePath()
    {
        return _savePath;
    }
}
