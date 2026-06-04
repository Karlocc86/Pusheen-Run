using UnityEngine;

// objeto coleccionable que activa el evento LluviaDeBurguesas al ser tocado por Pusheen
// arrastra el prefab a PathGenerator.triggerEventoPrefab en el Inspector
// asegurate de que el GameObject tenga: Collider2D con isTrigger=true, tag "Pusheen" en el jugador
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TriggerEvento : MonoBehaviour
{
    // duracion del evento en segundos; configurable desde el Inspector sin tocar codigo
    [Tooltip("Cuántos segundos dura la LluviaDeBurguesas al activarse.")]
    public float duracionEvento = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Pusheen")) return;
        if (GameManager.Instance == null) return;

        // si ya hay un evento activo no hacemos nada; GameManager lo ignora de todas formas
        // pero evitamos la corrutina innecesaria
        if (GameManager.Instance.EventoActivo) return;

        GameManager.Instance.StartCoroutine(
            GameManager.Instance.ActivarLluviaBurguesas(duracionEvento)
        );

        // se destruye a si mismo para que no se pueda activar dos veces
        Destroy(gameObject);
    }
}
