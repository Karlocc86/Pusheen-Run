using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// controla toda la interfaz de usuario: menu, score en pantalla y pantalla de game over
// los botones del canvas deben llamar a los metodos publicos de este script
// el nombre de clase debe coincidir exactamente con el nombre del archivo para que Unity lo reconozca
public class UIManager : MonoBehaviour
{
    // singleton para que GameManager pueda llamar MostrarGameOver() sin buscar referencias
    public static UIManager Instance { get; private set; }

    // arrastra estos objetos desde el inspector al script en el editor
    [Tooltip("Panel del menú principal; se activa al inicio y se oculta al comenzar la partida.")]
    public GameObject panelMenu;

    [Tooltip("Panel o texto que se activa al perder.")]
    public GameObject textoGameOver;

    // TMP_Text es TextMeshPro, mas bonito y flexible que el Text normal de Unity
    [Tooltip("Texto TMP donde se muestra el score en tiempo real.")]
    public TMP_Text textoScore;

    [Tooltip("Texto TMP donde se muestra el highscore (máximo histórico).")]
    public TMP_Text textoHighscore;

    [Tooltip("Texto para mostrar el highscore en el menú.")]
    public TMP_Text textoHighscoreMenu;

    [Tooltip("Texto para mostrar el nivel de dificultad durante el juego.")]
    public TMP_Text textoDificultad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // esconde el game over al inicio para que no este ahi desde el principio
        textoGameOver.SetActive(false);

        // muestra el menu y pausa el juego hasta que el jugador le de a jugar
        panelMenu.SetActive(true);
        Time.timeScale = 0f;

        // muestra el highscore en el menu
        if (textoHighscoreMenu != null && ScoreManager.Instance != null)
            textoHighscoreMenu.text = "Best: " + ScoreManager.Instance.Highscore;
    }

    void Update()
    {
        // actualiza el texto de score cada frame para que se vea en tiempo real
        // el (int) convierte el float a entero para que no muestre decimales feos tipo 123.456789
        if (textoScore != null)
            textoScore.text = "Score: " + (int)ScoreManager.Instance.Score;

        // actualiza el highscore (maximo historico)
        if (textoHighscore != null)
            textoHighscore.text = "High Score: " + ScoreManager.Instance.Highscore;

        // muestra el nivel de dificultad basado en el score
        if (textoDificultad != null && GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            string dificultad = ObtenerNivelDificultad(ScoreManager.Instance.Score);
            textoDificultad.text = "Difficulty: " + dificultad;
        }
    }

    // retorna el nivel de dificultad segun el score actual
    private string ObtenerNivelDificultad(float score)
    {
        if (score < 50) return "Easy";
        if (score < 150) return "Medium";
        if (score < 300) return "Hard";
        if (score < 500) return "Very Hard";
        return "INSANE";
    }

    // este metodo lo llama el boton "Jugar" del menu; oculta el menu y arranca el juego
    public void IniciarJuego()
    {
        panelMenu.SetActive(false);
        GameManager.Instance.StartGame();
    }

    // lo llama GameManager.TriggerGameOver() automaticamente cuando pusheen choca con algo
    public void MostrarGameOver()
    {
        textoGameOver.SetActive(true);
    }

    // este metodo va en el boton "Reiniciar" del panel de game over
    // cargar la misma escena de nuevo es la forma mas facil de resetear todo el juego
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
