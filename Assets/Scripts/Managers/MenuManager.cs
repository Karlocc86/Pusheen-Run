using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Tooltip("Panel de UI que representa el menú principal.")]
    public GameObject panelMenu;

    // Awake se llama antes que Start, ideal para inicializar el singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Muestra el menú y pausa el juego hasta que el jugador pulse Iniciar
    public void MostrarMenu()
    {
        panelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    // Oculta el menú y le pide al GameManager que arranque la partida
    public void IniciarJuego()
    {
        panelMenu.SetActive(false);
        GameManager.Instance.StartGame();
    }
}
