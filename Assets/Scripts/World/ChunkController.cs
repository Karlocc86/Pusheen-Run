using UnityEngine;

// mueve el chunk hacia la izquierda igual que el resto del mundo
// cuando sale completamente de pantalla avisa a PathGenerator y se destruye
// PathGenerator lo añade automáticamente al instanciar el chunk; no hace falta ponerlo en el prefab
public class ChunkController : MonoBehaviour
{
    [Tooltip("Velocidad de desplazamiento; debe coincidir con Movimiento.cs.")]
    public float velocidad = 5f;

    [Tooltip("Ancho manual del chunk en unidades de mundo. Si es 0 se calcula automáticamente desde los Renderers.")]
    public float anchoManual = 0f;

    // offset desde el pivote del chunk hasta su borde derecho en unidades de mundo
    // se calcula una sola vez en Start para no hacer GetComponents cada frame
    private float _offsetBordeDerecho;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        _offsetBordeDerecho = CalcularOffsetBordeDerecho();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        transform.Translate(Vector3.left * velocidad * Time.deltaTime, Space.World);

        if (SalioCompletamente())
        {
            PathGenerator.Instance?.OnChunkSalido();
            Destroy(gameObject);
        }
    }

    // calcula cuánto hay desde el pivote hasta el borde derecho más lejano de los hijos
    // usa anchoManual si está configurado; si no, busca todos los Renderers del chunk
    private float CalcularOffsetBordeDerecho()
    {
        if (anchoManual > 0f) return anchoManual;

        // Renderer cubre SpriteRenderer, MeshRenderer, etc.
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[ChunkController] {name}: no se encontraron Renderers. Pon el ancho en 'Ancho Manual'.", this);
            return 20f; // fallback genérico
        }

        float maxX = float.MinValue;
        foreach (Renderer r in renderers)
            if (r.bounds.max.x > maxX) maxX = r.bounds.max.x;

        return Mathf.Max(0f, maxX - transform.position.x);
    }

    // el chunk salió cuando su borde derecho cruzó el borde izquierdo de la cámara
    private bool SalioCompletamente()
    {
        float bordeIzqCam = _cam.transform.position.x - _cam.orthographicSize * _cam.aspect;
        return transform.position.x + _offsetBordeDerecho < bordeIzqCam;
    }
}
