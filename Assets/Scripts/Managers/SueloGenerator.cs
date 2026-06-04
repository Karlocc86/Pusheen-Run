using UnityEngine;
using System.Collections.Generic;

// genera y recicla los tiles de suelo de forma centralizada
// al inicio llena la pantalla; durante Playing sigue generando tiles adelante
// y destruye los que quedan atras, sin necesitar Suelo.cs en cada tile
public class SueloGenerator : MonoBehaviour
{
    [Header("Prefab y dimensiones")]
    [Tooltip("Prefab del tile de suelo. NO debe tener Suelo.cs para evitar doble movimiento.")]
    public GameObject sueloPrefab;

    [Tooltip("Ancho del tile en unidades de mundo (mídelo en el Inspector del prefab).")]
    public float anchoTile = 5f;

    [Tooltip("Posición Y donde se colocan todos los tiles.")]
    public float posY = -4f;

    [Header("Generación")]
    [Tooltip("Tiles que se instancian al inicio para cubrir la pantalla.")]
    public int tilesIniciales = 6;

    [Tooltip("Tiles extra de buffer más allá del borde derecho visible.")]
    public int tilesBuffer = 2;

    [Header("Movimiento")]
    [Tooltip("Velocidad de desplazamiento hacia la izquierda; debe coincidir con Movimiento.cs.")]
    public float velocidad = 5f;

    // lista de tiles activos; los añadimos por la derecha y eliminamos por la izquierda
    private List<Transform> _tiles = new List<Transform>();
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        LlenarPantallaInicial();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MoverTiles();
        LimpiarTilesSalidos();
        GenerarTilesAdelante();
    }

    // instancia los tiles iniciales empezando desde el borde izquierdo de la camara
    private void LlenarPantallaInicial()
    {
        float bordeIzq = _cam.transform.position.x - _cam.orthographicSize * _cam.aspect;

        for (int i = 0; i < tilesIniciales; i++)
        {
            // centro del tile i: bordeIzq + media anchura + i * anchura completa
            float centroX = bordeIzq + anchoTile * 0.5f + anchoTile * i;
            SpawnTile(centroX);
        }
    }

    // mueve todos los tiles hacia la izquierda a velocidad constante
    private void MoverTiles()
    {
        float desplazamiento = velocidad * Time.deltaTime;
        foreach (Transform t in _tiles)
            t.position += Vector3.left * desplazamiento;
    }

    // elimina tiles cuyo borde derecho ya pasó completamente el borde izquierdo de la cámara
    private void LimpiarTilesSalidos()
    {
        float limiteIzq = _cam.transform.position.x - _cam.orthographicSize * _cam.aspect - anchoTile;

        for (int i = _tiles.Count - 1; i >= 0; i--)
        {
            if (_tiles[i] == null || _tiles[i].position.x + anchoTile * 0.5f < limiteIzq)
            {
                if (_tiles[i] != null) Destroy(_tiles[i].gameObject);
                _tiles.RemoveAt(i);
            }
        }
    }

    // si el tile más a la derecha no llega al umbral, genera nuevos tiles hasta cubrirlo
    private void GenerarTilesAdelante()
    {
        float bordeDer = _cam.transform.position.x + _cam.orthographicSize * _cam.aspect;
        float umbral = bordeDer + anchoTile * tilesBuffer;

        // máximo 10 spawns por frame como seguro contra loops infinitos
        int seguro = 10;
        while (seguro-- > 0)
        {
            float bordeDerechoUltimo = _tiles.Count > 0
                ? _tiles[_tiles.Count - 1].position.x + anchoTile * 0.5f
                : bordeDer;

            if (bordeDerechoUltimo >= umbral) break;

            // el centro del nuevo tile queda justo pegado al borde derecho del último
            SpawnTile(bordeDerechoUltimo + anchoTile * 0.5f);
        }
    }

    private void SpawnTile(float centroX)
    {
        Vector3 pos = new Vector3(centroX, posY, 0f);
        GameObject go = Instantiate(sueloPrefab, pos, Quaternion.identity, transform);
        _tiles.Add(go.transform);
    }
}
