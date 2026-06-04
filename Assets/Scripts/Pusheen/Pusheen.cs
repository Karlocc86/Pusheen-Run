using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Pusheen : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private float _fuerzaSalto = 13.5f;
    [SerializeField] private float _maxVelocidad = 15f; // limita velocidad de caida
    [SerializeField] private float _dragFactor = 0.5f; // usar para q caiga mas rapido

    private bool _laObesaEstaEnElSuelo = true;
    private Color _colorOriginal;

    // Awake se llama antes que Start; inicializa componentes antes de que otros scripts los usen
    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _colorOriginal = _spriteRenderer.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // configura fisica para mejor feel del juego
        _rb.gravityScale = 2f; // cae mas rapido
        _rb.linearDamping = _dragFactor; // resistencia al aire (caidas mas suaves)
    }

    void OnEnable()
    {
        // _inputActions se inicializa en Awake; si OnEnable llega primero lo creamos aqui
        _inputActions ??= new InputSystem_Actions();
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions?.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Solo dejamos saltar si el juego está activo
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (_inputActions.Player.Jump.triggered && _laObesaEstaEnElSuelo)
        {
            Jump();
        }

        // limita la velocidad de caida maxima para evitar saltos pegajosos
        if (_rb.linearVelocity.y < -_maxVelocidad)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_maxVelocidad);
        }
    }

    // Aplica un impulso vertical al Rigidbody2D y marca que Pusheen está en el aire
    void Jump()
    {
        // En 2D usamos Rigidbody2D y Vector2 en lugar de Rigidbody y Vector3
        _rb.AddForce(Vector2.up * _fuerzaSalto, ForceMode2D.Impulse);
        _laObesaEstaEnElSuelo = false;
    }

    // Detecta cuando Pusheen aterriza sobre el suelo o una plataforma para habilitar el salto
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cuando toca el suelo ya puede volver a saltar
        if (collision.gameObject.CompareTag("Suelo"))
        {
            _laObesaEstaEnElSuelo = true;
        }
    }

    // Detecta triggers de ítems coleccionables y obstáculos
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Postre"))
        {
            ScoreManager.Instance.AddPoints(50f);
            PlayCollectEffect();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstaculo"))
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    // efecto visual al recoger un postre: flash amarillo
    private void PlayCollectEffect()
    {
        StartCoroutine(FlashColor(Color.yellow, 0.15f));
    }

    // corrutina para hacer un flash de color
    private System.Collections.IEnumerator FlashColor(Color flashColor, float duration)
    {
        _spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = _colorOriginal;
    }
}
