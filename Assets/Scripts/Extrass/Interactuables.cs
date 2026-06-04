using UnityEngine;

// este script va en los objetos del mundo, postres y obstaculos pues
// basicamente detecta si pusheen los toca y hace lo que toque segun el tag
// ojo: si le pones Consumibles.cs u Obstaculos.cs en el mismo objeto quita este
// porque si no va a sumar puntos dos veces o algo asi, no se pues
[RequireComponent(typeof(SpriteRenderer))]
public class Interactuables : MonoBehaviour
{
    // aqui metes los sprites que puede tener el objeto, luego en Start elige uno al azar
    // asi con un solo prefab puedes tener varios aspectos diferentes, re util
    public Sprite[] sprites;

    // pues esto es el tamanio que quieres que tenga el objeto en pantalla
    // lo que hace luego es calcular cuanto hay que escalar para que quede de ese tamanio
    public float tamanoObjetivo = 1f;

    void Start()
    {
        // si hay sprites en el array pues agarra uno al azar y se lo asigna al SpriteRenderer
        if (sprites != null && sprites.Length > 0)
        {
            Sprite sprite = sprites[Random.Range(0, sprites.Length)];
            GetComponent<SpriteRenderer>().sprite = sprite;

            // esto esta medio raro pero pues basicamente el sprite sabe cuantos pixeles tiene
            // y tambien sabe cuantos pixeles equivalen a una unidad en el mundo del juego (pixelsPerUnit)
            // entonces divides y ya sabes que tan grande es el sprite en "unidades de juego" pues
            // luego divides el tamanio que quieres entre eso y te da el scale que necesitas, asi
            float tamanoEnUnidades = sprite.rect.width / sprite.pixelsPerUnit;
            float escala = tamanoObjetivo / tamanoEnUnidades;
            transform.localScale = Vector3.one * escala;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // aqui es donde pasan las cosas, cuando pusheen toca el objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        // si lo que entro al trigger no es pusheen pues no hace nada
        if (!other.CompareTag("Pusheen")) return;

        if (gameObject.CompareTag("Postre"))
        {
            ScoreManager.Instance.AddPoints(50f);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayColectar();
            Destroy(gameObject);
            print("Pusheen se comio un tiramisu"); // Cam hazme uno xfi
        }
        else if (gameObject.CompareTag("Burger"))
        {
            ScoreManager.Instance.AddPoints(25f);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayColectar();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Obstaculo"))
        {
            // era un obstaculo, game over pues
            GameManager.Instance.TriggerGameOver();
            print("Game Over");
        }
    }
}