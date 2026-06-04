using UnityEngine;
using System.IO;

// estructura para serializar el highscore en JSON
[System.Serializable]
public class HighscoreSaveData
{
    public int highscore;
    public string lastPlayDate;
}

// lleva la cuenta del score durante la partida y guarda el highscore entre sesiones
// el score sube solo con el tiempo y tambien cuando pusheen se come un postre
public class ScoreManager : MonoBehaviour
{
    // singleton para accederlo con ScoreManager.Instance desde cualquier script
    public static ScoreManager Instance { get; private set; }

    // esta es la clave con la que se guarda el highscore en el dispositivo (PlayerPrefs)
    // PlayerPrefs es como un mini diccionario que persiste aunque cierres el juego
    private const string HighscoreKey = "Highscore";
    private const string SaveFileName = "gamesave.json";

    public float Score { get; private set; }
    public int Highscore { get; private set; }

    // inicializa el singleton y carga el highscore que se guardo la ultima vez que jugaron
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // GetInt busca el valor guardado con esa clave; si no existe devuelve el default (0)
        Highscore = PlayerPrefs.GetInt(HighscoreKey, 0);
    }

    void Start()
    {
        Score = 0f; // empieza en cero cada partida
    }

    void Update()
    {
        // solo sumamos score si el juego esta activo, no en menu ni en game over
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            IncreaseScoreOverTime();
        }
    }

    // suma puntos cada frame basado en cuanto tiempo paso (Time.deltaTime = segundos del frame)
    // asi si el juego va a 60fps o 30fps el score sube igual de rapido, independiente del hardware
    private void IncreaseScoreOverTime()
    {
        Score += Time.deltaTime * 10f;
        TrySaveHighscore();
    }

    // suma puntos de golpe; lo llama Interactuables.cs cuando pusheen se come un postre
    public void AddPoints(float amount)
    {
        Score += amount;
        TrySaveHighscore();
    }

    // revisa si el score actual es el nuevo record y si es asi lo guarda en el dispositivo
    private void TrySaveHighscore()
    {
        if ((int)Score > Highscore)
        {
            Highscore = (int)Score;
            PlayerPrefs.SetInt(HighscoreKey, Highscore); // guarda en memoria
            PlayerPrefs.Save(); // escribe en disco, por si el juego truena antes de cerrarse bien
            SaveHighscoreToJSON(Highscore); // tambien guarda en JSON para legibilidad
        }
    }

    // guarda el highscore en JSON en la carpeta de datos del juego
    private void SaveHighscoreToJSON(int highscore)
    {
        try
        {
            string savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            string json = JsonUtility.ToJson(new HighscoreSaveData
            {
                highscore = highscore,
                lastPlayDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Highscore guardado en JSON: {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"No se pudo guardar JSON: {e.Message}");
        }
    }

    // resetea el score a cero; llamalo al iniciar una nueva partida
    public void ResetScore()
    {
        Score = 0f;
    }
}
