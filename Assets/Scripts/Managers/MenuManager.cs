using UnityEngine;

// maneja el panel del menu principal
// puede ser que eventualmente lo unamos con UIManager pero por ahora lo dejamos separado pues
public class MenuManager : MonoBehaviour
{
    // singleton para accederlo con MenuManager.Instance desde cualquier script
    public static MenuManager Instance { get; private set; }

    // arrastra el panel del menu desde el inspector a este campo
    [Tooltip("Panel de UI que representa el menú principal.")]
    public GameObject panelMenu;

    // Awake para inicializar el singleton antes de que otros scripts lo usen
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // activa el panel y pausa el juego mientras el jugador decide si quiere jugar
    public void MostrarMenu()
    {
        panelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    // oculta el menu y le dice al GameManager que arranque la partida
    // este metodo debe estar enlazado al boton "Iniciar" del canvas
    public void IniciarJuego()
    {
        panelMenu.SetActive(false);
        GameManager.Instance.StartGame();
    }
}
