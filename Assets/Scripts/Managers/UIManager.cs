using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// El nombre de clase debe coincidir exactamente con el nombre del archivo para que Unity lo reconozca
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Tooltip("Panel del menú principal; se activa al inicio y se oculta al comenzar la partida.")]
    public GameObject panelMenu;

    [Tooltip("Panel o texto que se activa al perder.")]
    public GameObject textoGameOver;

    [Tooltip("Texto TMP donde se muestra el score en tiempo real.")]
    public TMP_Text textoScore;

    // Awake se llama antes que Start, ideal para inicializar el singleton
    private void Awake()
    {
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
        // Nos aseguramos que el texto de Game Over esté oculto al inicio
        textoGameOver.SetActive(false);

        // Mostramos el menú principal y pausamos el juego hasta que el jugador pulse Iniciar
        panelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Actualiza el texto de score cada frame con el valor actual del ScoreManager
        if (textoScore != null)
            textoScore.text = "Score: " + (int)ScoreManager.Instance.Score;
    }

    // Oculta el menú principal y arranca la partida; debe estar enlazado al botón Jugar del panel
    public void IniciarJuego()
    {
        panelMenu.SetActive(false);
        GameManager.Instance.StartGame();
    }

    // Activa el panel de Game Over; llamado por GameManager.TriggerGameOver()
    public void MostrarGameOver()
    {
        textoGameOver.SetActive(true);
    }

    // Recarga la escena activa para reiniciar la partida desde cero
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}