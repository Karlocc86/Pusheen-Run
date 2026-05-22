using UnityEngine;

// Mueve el objeto hacia la izquierda y lo destruye cuando supera la longitud máxima.
// Se usa en obstáculos y elementos del mundo que no necesitan reposicionarse.
public class Movimiento : MonoBehaviour
{
    [SerializeField] private float _velocidad = 5f;

    [Tooltip("Distancia negativa en X a partir de la cual el objeto se destruye.")]
    [SerializeField] private float _longitud = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Solo movemos si el juego está activo
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Mueve el objeto a la izquierda en coordenadas de mundo
        transform.Translate(Vector3.left * _velocidad * Time.deltaTime, Space.World);

        // Destruye el objeto cuando sale completamente de la pantalla por la izquierda
        if (transform.position.x < -_longitud)
        {
            Destroy(gameObject);
        }
    }
}