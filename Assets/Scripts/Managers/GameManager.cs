using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// este es el manager mas importante pues, controla el estado del juego
// menu, jugando o game over; todos los demas scripts lo consultan para saber que hacer
public class GameManager : MonoBehaviour
{
    // Singleton: solo existe una instancia y la puedes agarrar desde cualquier script con GameManager.Instance
    // es como una variable global pero mas ordenada pues
    public static GameManager Instance { get; private set; }

    // estados posibles del juego
    public enum GameState { Menu, Playing, GameOver, Paused }
    public GameState CurrentState { get; private set; }

    // static sobrevive la recarga de escena; UIManager lo lee en Start para saltar el menú
    public static bool SaltarMenuAlCargar = false;

    // recuerda en qué estado estaba el juego antes de pausar para volver a él al reanudar
    private GameState estadoAntesDePausa;

    // true mientras esta corriendo el evento LluviaDeBurguesas
    // PathGenerator lo consulta para suspender obstaculos y spawnear solo hamburguesas
    public bool EventoActivo { get; private set; }

    // Awake se llama antes que Start, por eso ponemos el singleton aqui
    // asi otros scripts pueden usarlo desde su propio Start sin problemas de orden
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Awake corre antes que cualquier Start(), así UIManager ya lee el estado correcto
        // si viene de un Play Again arrancamos directo en Playing sin pasar por menú
        if (SaltarMenuAlCargar)
        {
            SaltarMenuAlCargar = false;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }
        else
        {
            CurrentState = GameState.Menu;
        }
    }

    void Start()
    {
        // el audio se inicializa aquí porque AudioManager.Awake() corre en paralelo al nuestro
        if (CurrentState == GameState.Playing)
            AudioManager.Instance?.PlayIngame();
        else
            AudioManager.Instance?.PlayMenu();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Escape alterna entre pausar y reanudar durante la partida
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Playing) PauseGame();
            else if (CurrentState == GameState.Paused) ResumeGame();
        }

        // R reinicia desde game over o desde pausa
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.GameOver || CurrentState == GameState.Paused)
                UIManager.Instance?.Reiniciar();
        }
    }

    // cambia el estado a Playing y reactiva el tiempo (que estaba en 0 mientras el menu estaba abierto)
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayIngame();
    }

    // esto se llama cuando Pusheen choca con un obstaculo; para el juego y muestra el game over
    public void TriggerGameOver()
    {
        // evita llamarlo dos veces seguidas por si dos cosas colisionan al mismo tiempo
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        EventoActivo = false;
        Time.timeScale = 0f;
        UIManager.Instance?.MostrarGameOver();
        // PlayGameOver para el sonido de derrota; PlayPlayAgain espera y luego hace fade in del loop
        AudioManager.Instance?.PlayGameOver();
        AudioManager.Instance?.PlayPlayAgain();
        Debug.Log("Game Over");
    }

    // pausa el juego: congela el tiempo, muestra el panel de pausa y baja el volumen
    // solo funciona si el estado actual es Playing
    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        estadoAntesDePausa = CurrentState;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        UIManager.Instance?.MostrarPausa();
        AudioManager.Instance?.DuckMusica();
    }

    // reanuda el juego: restaura el tiempo, oculta el panel de pausa y sube el volumen
    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = estadoAntesDePausa;
        Time.timeScale = 1f;
        UIManager.Instance?.OcultarPausa();
        AudioManager.Instance?.RestaurarMusica();
    }

    // activa el evento LluviaDeBurguesas por 'duracion' segundos
    // si ya hay un evento activo lo ignora para no apilarlos
    public IEnumerator ActivarLluviaBurguesas(float duracion = 10f)
    {
        if (EventoActivo) yield break;

        EventoActivo = true;
        Debug.Log("Evento: LluviaDeBurguesas iniciado por " + duracion + "s");

        // WaitForSecondsRealtime para que funcione aunque timeScale cambie
        yield return new WaitForSecondsRealtime(duracion);

        EventoActivo = false;
        Debug.Log("Evento: LluviaDeBurguesas terminado");
    }
}
