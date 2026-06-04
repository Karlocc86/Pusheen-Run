using UnityEngine;

// este script mueve cualquier objeto del mundo hacia la izquierda
// como Pusheen no se mueve, lo que hacemos es mover TODO lo demas hacia ella
// cuando el objeto ya salio de pantalla por la izquierda simplemente lo destruimos, ya no sirve
public class Movimiento : MonoBehaviour
{
    // que tan rapido va el objeto hacia la izquierda, ajustalo en el inspector segun el obstaculo
    [SerializeField] private float _velocidad = 5f;

    // si el objeto llega a esta distancia negativa en X pues ya se destruye, ya se fue de pantalla
    [Tooltip("Distancia negativa en X a partir de la cual el objeto se destruye.")]
    [SerializeField] private float _longitud = 50f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // si el juego no esta en Playing no movemos nada, como cuando esta en menu o game over
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Vector3.left es lo mismo que (-1, 0, 0), o sea hacia la izquierda
        // Space.World es para moverse en coordenadas del mundo y no relativas al objeto
        // Time.deltaTime lo multiplicamos para que la velocidad sea igual sin importar los fps
        transform.Translate(Vector3.left * _velocidad * Time.deltaTime, Space.World);

        // si ya paso la pantalla pues adios objeto, liberamos memoria
        if (transform.position.x < -_longitud)
        {
            Destroy(gameObject);
        }
    }
}
