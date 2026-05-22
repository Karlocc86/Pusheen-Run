using UnityEngine;

// Mueve el segmento de suelo hacia la izquierda y lo reposiciona a la derecha
// cuando sale de la pantalla, creando un efecto de suelo infinito sin Instantiate.
// Requiere que todos los segmentos de suelo tengan este componente y el mismo ancho.
public class Suelo : MonoBehaviour
{
    [Tooltip("Velocidad de desplazamiento hacia la izquierda (debe coincidir con Movimiento.cs).")]
    [SerializeField] private float _velocidad = 5f;

    [Tooltip("Ancho del segmento en unidades de mundo (igual al ancho del sprite).")]
    [SerializeField] private float _anchoSegmento = 20f;

    [Tooltip("Número total de segmentos de suelo en escena (para calcular el salto de reposición).")]
    [SerializeField] private int _totalSegmentos = 2;

    private Camera _cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Solo se mueve mientras el juego está activo
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MoverIzquierda();
        ReposicionarSiSaleDePantalla();
    }

    // Desplaza el segmento hacia la izquierda a velocidad constante
    private void MoverIzquierda()
    {
        transform.Translate(Vector3.left * _velocidad * Time.deltaTime, Space.World);
    }

    // Cuando el borde derecho del segmento cruza el borde izquierdo de la cámara,
    // lo teletransporta a la derecha del último segmento para mantener el loop
    private void ReposicionarSiSaleDePantalla()
    {
        float bordeIzquierdoCam = _cam.transform.position.x
                                  - _cam.orthographicSize * _cam.aspect;

        // El segmento salió completamente por la izquierda
        if (transform.position.x + _anchoSegmento / 2f < bordeIzquierdoCam)
        {
            // Salta hacia la derecha la distancia total que ocupan todos los segmentos
            transform.position += Vector3.right * _anchoSegmento * _totalSegmentos;
        }
    }
}
