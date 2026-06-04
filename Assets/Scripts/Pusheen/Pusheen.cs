using UnityEngine;
using UnityEngine.InputSystem;

public class Pusheen : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private Rigidbody2D _rb;

    [SerializeField] private float _fuerzaSalto = 8f;

    private bool _laObesaEstaEnElSuelo = true;

    // Awake se llama antes que Start; inicializa componentes antes de que otros scripts los usen
    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Solo dejamos saltar si el juego está activo
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (_inputActions.Player.Jump.triggered && _laObesaEstaEnElSuelo)
        {
            Jump();
            print("Jump");
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
        if (collision.gameObject.CompareTag("Suelo") || collision.gameObject.CompareTag("Plataforma"))
        {
            _laObesaEstaEnElSuelo = true;
        }
    }

    // Detecta triggers de ítems coleccionables y obstáculos -> Interactuables
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reaccionamos si quien colisiona es Pusheen
        if (!other.CompareTag("Pusheen")) return;

        if (other.CompareTag("Postre"))
        {
            ScoreManager.Instance.AddPoints(50f);
            Destroy(other.gameObject);
            print("Pusheen se comio un postre o lo q le caiga"); // Cam hazme uno xfi
        }
        else if (other.CompareTag("Obstaculo"))
        {
            GameManager.Instance.TriggerGameOver();
            print("Game Over");
        }
    }
}
