using UnityEngine;

// este es el manager mas importante pues, controla el estado del juego
// menu, jugando o game over; todos los demas scripts lo consultan para saber que hacer
public class GameManager : MonoBehaviour
{
    // Singleton: solo existe una instancia y la puedes agarrar desde cualquier script con GameManager.Instance
    // es como una variable global pero mas ordenada pues
    public static GameManager Instance { get; private set; }

    // los tres estados posibles del juego, como una maquinita de estados
    public enum GameState { Menu, Playing, GameOver }
    public GameState CurrentState { get; private set; }

    // Awake se llama antes que Start, por eso ponemos el singleton aqui
    // asi otros scripts pueden usarlo desde su propio Start sin problemas de orden
    private void Awake()
    {
        // si ya hay una instancia de GameManager en escena destruye este duplicado
        // puede pasar si cargas la escena dos veces o si tienes DontDestroyOnLoad
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // el juego arranca en Menu; StartGame() lo cambia a Playing cuando el jugador le da jugar
        CurrentState = GameState.Menu;
    }

    void Update()
    {

    }

    // cambia el estado a Playing y reactiva el tiempo (que estaba en 0 mientras el menu estaba abierto)
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f; // timeScale 0 = todo pausado, 1 = velocidad normal
    }

    // esto se llama cuando Pusheen choca con un obstaculo; para el juego y muestra el game over
    public void TriggerGameOver()
    {
        // evita llamarlo dos veces seguidas por si dos cosas colisionan al mismo tiempo
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f; // pausa todo el juego congelando el tiempo
        // le dice al UIManager que muestre la pantalla de game over
        // el ? es para que no truene si por alguna razon no hay UIManager en escena
        UIManager.Instance?.MostrarGameOver();
        Debug.Log("Game Over");
    }
}
