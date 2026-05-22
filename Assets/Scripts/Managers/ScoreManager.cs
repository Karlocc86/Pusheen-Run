using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string HighscoreKey = "Highscore";

    public float Score { get; private set; }
    public int Highscore { get; private set; }

    /// Inicializa el Singleton y carga el highscore almacenado en PlayerPrefs.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Highscore = PlayerPrefs.GetInt(HighscoreKey, 0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Score = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // El score solo aumenta mientras el juego esté en estado Playing
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            IncreaseScoreOverTime();
        }
    }

    /// Aumenta el score basado en el tiempo real transcurrido.
    /// Se llama desde Update únicamente cuando el juego está activo.
    private void IncreaseScoreOverTime()
    {
        Score += Time.deltaTime * 10f;
        TrySaveHighscore();
    }

    // Suma puntos al score; llamado por Consumibles o Pusheen al recolectar un ítem
    public void AddPoints(float amount)
    {
        Score += amount;
        TrySaveHighscore();
    }

    // Compara el score actual con el highscore y lo persiste si es mayor
    private void TrySaveHighscore()
    {
        if ((int)Score > Highscore)
        {
            Highscore = (int)Score;
            PlayerPrefs.SetInt(HighscoreKey, Highscore);
            PlayerPrefs.Save();
        }
    }

    // Reinicia el score a cero al comenzar una nueva partida
    public void ResetScore()
    {
        Score = 0f;
    }
}
