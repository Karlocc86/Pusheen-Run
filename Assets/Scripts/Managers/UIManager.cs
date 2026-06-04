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

    [Tooltip("Panel de pausa que cubre la pantalla; contiene el botón de reanudar.")]
    public GameObject panelPausa;

    [Tooltip("Botón de pausa visible solo durante Playing (esquina superior).")]
    public GameObject botonPausa;

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
        textoGameOver.SetActive(false);
        panelMenu.SetActive(true);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (botonPausa != null) botonPausa.SetActive(false);
        Time.timeScale = 0f;

        // fallback directo desde PlayerPrefs por si ScoreManager aun no esta listo
        if (textoHighscoreMenu != null)
            textoHighscoreMenu.text = "Best: " + PlayerPrefs.GetInt("Highscore", 0);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        var estado = GameManager.Instance.CurrentState;

        // el botón de pausa solo se muestra mientras el juego está activo
        if (botonPausa != null)
            botonPausa.SetActive(estado == GameManager.GameState.Playing);

        if (ScoreManager.Instance == null) return;

        string bestText = "Best: " + ScoreManager.Instance.Highscore;
        if (textoHighscore != null) textoHighscore.text = bestText;
        if (textoHighscoreMenu != null) textoHighscoreMenu.text = bestText;

        if (estado != GameManager.GameState.Playing) return;

        if (textoScore != null)
            textoScore.text = "Score: " + (int)ScoreManager.Instance.Score;

        if (textoDificultad != null)
            textoDificultad.text = ObtenerNivelDificultad(ScoreManager.Instance.Score);
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

    // muestra el panel de pausa; lo llama GameManager.PauseGame()
    public void MostrarPausa()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
    }

    // oculta el panel de pausa; lo llama GameManager.ResumeGame()
    public void OcultarPausa()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    // enlaza al botón "||" visible durante el juego
    public void PausarJuego()
    {
        GameManager.Instance?.PauseGame();
    }

    // enlaza al botón "Reanudar" dentro del panelPausa
    public void ReanudarJuego()
    {
        GameManager.Instance?.ResumeGame();
    }

    // este metodo va en el boton "Reiniciar" del panel de game over
    // cargar la misma escena de nuevo es la forma mas facil de resetear todo el juego
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
