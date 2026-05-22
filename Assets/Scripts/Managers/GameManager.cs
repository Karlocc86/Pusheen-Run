using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance accesible desde cualquier script
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }
    public GameState CurrentState { get; private set; }

    // Awake se llama antes que Start, ideal para inicializar el singleton
    private void Awake()
    {
        // Si ya existe una instancia y no es esta, destruye el duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // El juego arranca en estado Menu; StartGame() lo activa el UIManager al pulsar Jugar
        CurrentState = GameState.Menu;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Inicializa el juego y reactiva el tiempo
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    // Pausa el juego, cambia el estado a GameOver y notifica al UIManager
    public void TriggerGameOver()
    {
        // Evita llamar GameOver múltiples veces
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;
        // Muestra la pantalla de Game Over si el UIManager existe en escena
        UIManager.Instance?.MostrarGameOver();
        Debug.Log("Game Over");
    }
}