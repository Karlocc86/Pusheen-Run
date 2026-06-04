using UnityEngine;

// mueve el suelo hacia la izquierda y cuando sale de pantalla lo regresa al otro lado
// asi parece que el suelo es infinito sin tener que crear objetos nuevos, re eficiente
// necesitas al menos 2 segmentos de suelo en escena para que no haya huecos pues
public class Suelo : MonoBehaviour
{
    // debe ser la misma velocidad que Movimiento.cs para que todo se mueva parejo
    [Tooltip("Velocidad de desplazamiento hacia la izquierda (debe coincidir con Movimiento.cs).")]
    [SerializeField] private float _velocidad = 5f;

    // el ancho del sprite de suelo en unidades del mundo; midetelo en el inspector
    [Tooltip("Ancho del segmento en unidades de mundo (igual al ancho del sprite).")]
    [SerializeField] private float _anchoSegmento = 20f;

    // cuantos pedazos de suelo hay en total en la escena, para calcular cuanto brincar al reposicionar
    [Tooltip("Número total de segmentos de suelo en escena (para calcular el salto de reposición).")]
    [SerializeField] private int _totalSegmentos = 2;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main; // guardamos la camara principal para calcular los bordes de pantalla
    }

    void Update()
    {
        // si no estamos jugando el suelo no se mueve
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MoverIzquierda();
        ReposicionarSiSaleDePantalla();
    }

    // mueve el segmento hacia la izquierda cada frame a velocidad constante
    private void MoverIzquierda()
    {
        transform.Translate(Vector3.left * _velocidad * Time.deltaTime, Space.World);
    }

    // cuando el segmento sale completamente por la izquierda lo manda al otro extremo
    // lo brinca la distancia total de todos los segmentos para quedar justo despues del ultimo
    private void ReposicionarSiSaleDePantalla()
    {
        // bordeIzquierdoCam: la X del limite izquierdo de lo que ve la camara
        // orthographicSize es la mitad del alto en unidades, y aspect es ancho dividido entre alto
        float bordeIzquierdoCam = _cam.transform.position.x
                                  - _cam.orthographicSize * _cam.aspect;

        // si el borde derecho del segmento ya paso el borde izquierdo de la camara, reposiciona
        if (transform.position.x + _anchoSegmento / 2f < bordeIzquierdoCam)
        {
            // salta la distancia total de todos los segmentos para quedar al final de la fila
            transform.position += Vector3.right * _anchoSegmento * _totalSegmentos;
        }
    }
}
