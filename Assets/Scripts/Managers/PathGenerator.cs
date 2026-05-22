using UnityEngine;
using System.Collections;

public class PathGenerator : MonoBehaviour
{
    public static PathGenerator Instance { get; private set; }

    [Header("Obstáculos")]
    [Tooltip("Prefabs de obstáculos que se pueden spawnear aleatoriamente.")]
    public GameObject[] obstaclePrefabs;

    [Tooltip("Probabilidad (0-1) de que cada intervalo genere un obstáculo.")]
    [Range(0f, 1f)]
    public float obstacleChance = 0.3f;

    [Tooltip("Posición Y de spawn de los obstáculos sobre el suelo.")]
    public float obstacleY = -2f;

    [Header("Consumibles")]
    [Tooltip("Prefabs de consumibles (postres) que se pueden spawnear aleatoriamente.")]
    public GameObject[] consumiblePrefabs;

    [Tooltip("Probabilidad (0-1) de que cada intervalo genere un consumible.")]
    [Range(0f, 1f)]
    public float consumibleChance = 0.2f;

    [Tooltip("Posición Y de spawn de los consumibles (normalmente más alta que el suelo).")]
    public float consumibleY = -1.5f;

    [Header("Timing y posición")]
    [Tooltip("Segundos entre cada intento de spawn.")]
    public float spawnInterval = 1.5f;

    [Tooltip("Distancia extra a la derecha del borde visible de la cámara donde aparecen los objetos.")]
    public float spawnOffsetX = 2f;

    private Camera _mainCamera;

    // Instancias cacheadas para no generar garbage en cada iteración de las corrutinas
    private WaitForSecondsRealtime _spawnWait;
    private WaitForSecondsRealtime _cleanupWait;

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

    void Start()
    {
        _mainCamera = Camera.main;

        // Se inicializan aquí (no en Awake) para respetar los valores del Inspector
        _spawnWait = new WaitForSecondsRealtime(spawnInterval);
        _cleanupWait = new WaitForSecondsRealtime(2f);

        StartCoroutine(SpawnCoroutine());
        StartCoroutine(CleanupCoroutine());
    }

    /// <summary>
    /// Corrutina principal: intenta generar obstáculos y consumibles cada spawnInterval
    /// mientras el juego esté en estado Playing.
    /// </summary>
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                TrySpawnObstacle();
                TrySpawnConsumible();
            }

            // WaitForSecondsRealtime ignora timeScale, evitando que la corrutina
            // quede congelada cuando el juego está en pausa (timeScale = 0)
            yield return _spawnWait;
        }
    }

    /// <summary>
    /// Calcula la X de spawn: borde derecho de cámara más el offset configurado.
    /// Así los objetos siempre aparecen justo fuera de la vista, sin importar
    /// cuánto tiempo lleva corriendo el juego.
    /// </summary>
    private float GetSpawnX()
    {
        return _mainCamera.transform.position.x
               + _mainCamera.orthographicSize * _mainCamera.aspect
               + spawnOffsetX;
    }

    /// <summary>
    /// Según la probabilidad configurada, elige un prefab de obstáculo aleatorio
    /// y lo instancia justo fuera del borde derecho de la cámara.
    /// </summary>
    private void TrySpawnObstacle()
    {
        Debug.Log("TrySpawnObstacle llamado");
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (Random.value > obstacleChance) return;

        int index = Random.Range(0, obstaclePrefabs.Length);
        Vector3 spawnPos = new Vector3(GetSpawnX(), obstacleY, 0f);
        Debug.Log("Spawneando obstaculo");
        Instantiate(obstaclePrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    /// <summary>
    /// Según la probabilidad configurada, elige un prefab de consumible aleatorio
    /// y lo instancia justo fuera del borde derecho de la cámara.
    /// </summary>
    private void TrySpawnConsumible()
    {
        if (consumiblePrefabs == null || consumiblePrefabs.Length == 0) return;
        if (Random.value > consumibleChance) return;

        int index = Random.Range(0, consumiblePrefabs.Length);
        Vector3 spawnPos = new Vector3(GetSpawnX(), consumibleY, 0f);
        Instantiate(consumiblePrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    /// <summary>
    /// Corrutina de limpieza: destruye periódicamente los hijos que hayan salido
    /// del borde izquierdo de la cámara para liberar memoria.
    /// </summary>
    private IEnumerator CleanupCoroutine()
    {
        while (true)
        {
            yield return _cleanupWait;
            DestroyOffscreenChildren();
        }
    }

    /// <summary>
    /// Recorre los hijos del PathGenerator y destruye aquellos cuya posición X
    /// queda por detrás del límite izquierdo visible de la cámara.
    /// </summary>
    private void DestroyOffscreenChildren()
    {
        float leftEdge = _mainCamera.transform.position.x
                         - _mainCamera.orthographicSize * _mainCamera.aspect
                         - spawnOffsetX;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.position.x < leftEdge)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
