using UnityEngine;
using System.Collections;

// este script genera obstaculos y postres mientras el juego esta corriendo
// cada cierto tiempo "tira una moneda" y dependiendo de la probabilidad spawnea algo o no
// tambien limpia los objetos que ya salieron de pantalla para no desperdiciar memoria
public class PathGenerator : MonoBehaviour
{
    public static PathGenerator Instance { get; private set; }

    [Header("Obstáculos")]
    // mete aqui todos los prefabs de obstaculos que quieras que aparezcan aleatoriamente
    [Tooltip("Prefabs de obstáculos que se pueden spawnear aleatoriamente.")]
    public GameObject[] obstaclePrefabs;

    // probabilidad de que aparezca un obstaculo: 0 = nunca, 1 = siempre, 0.3 = 30% de las veces
    [Tooltip("Probabilidad (0-1) de que cada intervalo genere un obstáculo.")]
    [Range(0f, 1f)]
    public float obstacleChance = 0.3f;

    // en que altura del mundo spawnean los obstaculos, ajustalo para que queden bien sobre el suelo
    [Tooltip("Posición Y de spawn de los obstáculos sobre el suelo.")]
    public float obstacleY = -2f;

    [Header("Consumibles")]
    // prefabs de postres y comidita que pusheen puede agarrar para ganar puntos
    [Tooltip("Prefabs de consumibles (postres) que se pueden spawnear aleatoriamente.")]
    public GameObject[] consumiblePrefabs;

    // igual que obstacleChance pero para los postres, generalmente menos comun que los obstaculos
    [Tooltip("Probabilidad (0-1) de que cada intervalo genere un consumible.")]
    [Range(0f, 1f)]
    public float consumibleChance = 0.2f;

    // los postres van mas arriba del suelo para que pusheen tenga que saltar a agarrarlos
    [Tooltip("Posición Y de spawn de los consumibles (normalmente más alta que el suelo).")]
    public float consumibleY = -1.5f;

    [Header("Timing y posición")]
    // cada cuantos segundos se intenta generar algo nuevo en pantalla (valor inicial)
    [Tooltip("Intervalo inicial entre cada intento de spawn.")]
    public float spawnInterval = 1.5f;

    // los objetos aparecen un poco mas alla del borde derecho para que no se vean aparecer de golpe
    [Tooltip("Distancia extra a la derecha del borde visible de la cámara donde aparecen los objetos.")]
    public float spawnOffsetX = 2f;

    [Header("Dificultad progresiva")]
    // score a partir del cual empieza la escalada de dificultad
    [Tooltip("Score a partir del cual empieza a escalar la dificultad.")]
    public float difficultyStartScore = 50f;

    // score en el que la dificultad llega al tope; mas alla de este no sigue subiendo
    [Tooltip("Score en el que la dificultad llega al máximo.")]
    public float difficultyMaxScore = 500f;

    // intervalo minimo al que llega el spawn cuando la dificultad esta al maximo
    [Tooltip("Intervalo mínimo de spawn al llegar al máximo de dificultad.")]
    public float minSpawnInterval = 0.6f;

    // probabilidad maxima de obstaculo al llegar al maximo de dificultad
    [Tooltip("Probabilidad máxima de obstáculo al llegar al máximo de dificultad.")]
    [Range(0f, 1f)]
    public float maxObstacleChance = 0.8f;

    private Camera _mainCamera;

    // guardamos estos para no crear basura (garbage) en cada iteracion del loop de las corrutinas
    // WaitForSecondsRealtime funciona aunque el juego este pausado (timeScale = 0)
    private WaitForSecondsRealtime _spawnWait;
    private WaitForSecondsRealtime _cleanupWait;

    // valores activos que cambian con la dificultad (los del Inspector son los valores base)
    private float _currentInterval;
    private float _currentObstacleChance;

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

        // arrancamos con los valores base del Inspector
        _currentInterval = spawnInterval;
        _currentObstacleChance = obstacleChance;

        // se inicializan en Start (no en Awake) para que ya tengan los valores del Inspector
        _spawnWait = new WaitForSecondsRealtime(_currentInterval);
        _cleanupWait = new WaitForSecondsRealtime(2f);

        // corrutinas: son como funciones que pueden "pausarse" con yield y continuar despues
        // mas eficientes que revisar todo en Update cuando no necesitas correr cada frame
        StartCoroutine(SpawnCoroutine());
        StartCoroutine(CleanupCoroutine());
    }

    // loop principal de spawn: cada spawnInterval intenta generar un obstaculo y un postre
    // usa WaitForSecondsRealtime para que siga corriendo aunque el juego este pausado
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                UpdateDifficulty();
                TrySpawnObstacle();
                TrySpawnConsumible();
            }

            // yield return = "espera esto y luego sigue desde aqui la proxima iteracion"
            yield return _spawnWait;
        }
    }

    // ajusta el intervalo y la probabilidad segun el score actual
    // InverseLerp devuelve un t de 0 a 1: 0 = dificultad base, 1 = dificultad maxima
    // _spawnWait solo se recrea cuando el intervalo cambia, para no generar garbage cada ciclo
    private void UpdateDifficulty()
    {
        if (ScoreManager.Instance == null) return;

        float t = Mathf.InverseLerp(difficultyStartScore, difficultyMaxScore, ScoreManager.Instance.Score);

        float newInterval = Mathf.Lerp(spawnInterval, minSpawnInterval, t);
        if (!Mathf.Approximately(newInterval, _currentInterval))
        {
            _currentInterval = newInterval;
            _spawnWait = new WaitForSecondsRealtime(_currentInterval);
        }

        _currentObstacleChance = Mathf.Lerp(obstacleChance, maxObstacleChance, t);
    }

    // calcula donde esta el borde derecho de la camara para spawnear justo afuera de la vista
    // orthographicSize es la mitad del alto en unidades del mundo, y aspect es ancho/alto de pantalla
    private float GetSpawnX()
    {
        return _mainCamera.transform.position.x
               + _mainCamera.orthographicSize * _mainCamera.aspect
               + spawnOffsetX;
    }

    // tira los dados y si pasan la probabilidad instancia un obstaculo aleatorio del array
    private void TrySpawnObstacle()
    {
        Debug.Log("TrySpawnObstacle llamado");
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        // Random.value da un numero entre 0 y 1; si es mayor que la chance pues no spawnea nada
        if (Random.value > _currentObstacleChance) return;

        int index = Random.Range(0, obstaclePrefabs.Length);
        Vector3 spawnPos = new Vector3(GetSpawnX(), obstacleY, 0f);
        Debug.Log("Spawneando obstaculo");
        // el "transform" al final hace que el objeto sea hijo de PathGenerator, util para la limpieza
        Instantiate(obstaclePrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    // igual que TrySpawnObstacle pero para postres; separados para poder controlar probabilidades distintas
    private void TrySpawnConsumible()
    {
        if (consumiblePrefabs == null || consumiblePrefabs.Length == 0) return;
        if (Random.value > consumibleChance) return;

        int index = Random.Range(0, consumiblePrefabs.Length);
        Vector3 spawnPos = new Vector3(GetSpawnX(), consumibleY, 0f);
        Instantiate(consumiblePrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    // cada 2 segundos revisa si hay hijos del PathGenerator que ya salieron de pantalla y los borra
    private IEnumerator CleanupCoroutine()
    {
        while (true)
        {
            yield return _cleanupWait;
            DestroyOffscreenChildren();
        }
    }

    // recorre todos los hijos del PathGenerator y destruye los que ya pasaron el borde izquierdo
    // el bucle va de atras hacia adelante para no saltarse indices al ir destruyendo elementos
    private void DestroyOffscreenChildren()
    {
        float leftEdge = _mainCamera.transform.position.x - _mainCamera.orthographicSize * _mainCamera.aspect - spawnOffsetX;

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
