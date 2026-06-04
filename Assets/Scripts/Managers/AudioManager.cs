using UnityEngine;
using System.Collections;

// controla todo el audio del juego: música de menú, ingame, playagain y sonido de derrota
// usa singleton para que GameManager y UIManager lo llamen sin buscar referencias
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de audio")]
    [Tooltip("AudioSource con la música del menú principal (en loop).")]
    public AudioSource musicaMenu;

    [Tooltip("AudioSource con la música durante la partida (en loop).")]
    public AudioSource musicaIngame;

    [Tooltip("AudioSource con la música de la pantalla play again (en loop).")]
    public AudioSource musicaPlayAgain;

    [Tooltip("AudioSource con el sonido/jingle de derrota (sin loop).")]
    public AudioSource sonidoDerrota;

    [Tooltip("AudioSource con el sonido de colectar un ítem (postre, burger). Sin loop.")]
    public AudioSource sonidoColectar;

    [Header("Configuración de transiciones")]
    [Tooltip("Segundos que tarda cada fade in o fade out.")]
    public float tiempoFade = 1f;

    [Tooltip("Volumen al que baja la música en juego cuando el juego está pausado (0 = mudo, 1 = original).")]
    [Range(0f, 1f)]
    public float volumenDuck = 0.3f;

    // volúmenes originales leídos del Inspector; se usan como objetivo en FadeIn y al restaurar tras duck
    private float volMenu;
    private float volIngame;
    private float volPlayAgain;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // guardamos los volúmenes aquí (Awake corre antes de Start en todos los scripts)
        // así PlayMenu() en GameManager.Start() ya tiene los valores correctos
        if (musicaMenu != null)      volMenu      = musicaMenu.volume;
        if (musicaIngame != null)    volIngame    = musicaIngame.volume;
        if (musicaPlayAgain != null) volPlayAgain = musicaPlayAgain.volume;
    }

    // --- métodos públicos llamados por GameManager según el estado ---

    // arranca la música del menú con fade in; para todo lo demás antes
    public void PlayMenu()
    {
        StopAllCoroutines();
        PararTodas();
        StartCoroutine(FadeIn(musicaMenu, volMenu));
    }

    // hace cross-fade a ingame parando todo lo que pudiera estar sonando antes
    public void PlayIngame()
    {
        StopAllCoroutines();
        if (sonidoDerrota != null) sonidoDerrota.Stop();
        StartCoroutine(FadeOut(musicaMenu));
        StartCoroutine(FadeOut(musicaPlayAgain));
        StartCoroutine(FadeIn(musicaIngame, volIngame));
    }

    // para la música activa y reproduce el sonido de derrota
    public void PlayGameOver()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(musicaIngame));
        StartCoroutine(FadeOut(musicaPlayAgain));
        if (sonidoDerrota != null) sonidoDerrota.Play();
    }

    // espera a que termine el sonido de derrota y luego hace fade in de playAgain
    // debe llamarse inmediatamente después de PlayGameOver()
    public void PlayPlayAgain()
    {
        StartCoroutine(EsperarYPlayPlayAgain());
    }

    // reproduce el sonido de colectar; usa PlayOneShot para que se solape si se recogen varios rápido
    public void PlayColectar()
    {
        if (sonidoColectar == null || sonidoColectar.clip == null) return;
        sonidoColectar.PlayOneShot(sonidoColectar.clip);
    }

    // para todo sin fade (útil para resets de escena o situaciones de emergencia)
    public void StopAll()
    {
        StopAllCoroutines();
        PararTodas();
    }

    // baja el volumen de la música ingame al porcentaje configurado (se llama al pausar)
    // para el sonido de derrota por si quedó activo antes de pausar
    public void DuckMusica()
    {
        if (sonidoDerrota != null && sonidoDerrota.isPlaying) sonidoDerrota.Stop();
        if (musicaIngame == null || !musicaIngame.isPlaying) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(musicaIngame, volIngame * volumenDuck, tiempoFade * 0.5f));
    }

    // restaura el volumen original de la música ingame (se llama al reanudar)
    public void RestaurarMusica()
    {
        if (musicaIngame == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(musicaIngame, volIngame, tiempoFade * 0.5f));
    }

    // --- corrutinas privadas de fade ---

    // sube el volumen de source de 0 hasta volObjetivo y arranca la reproducción
    private IEnumerator FadeIn(AudioSource source, float volObjetivo)
    {
        if (source == null) yield break;
        source.volume = 0f;
        source.Play();
        float t = 0f;
        while (t < tiempoFade)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, volObjetivo, t / tiempoFade);
            yield return null;
        }
        source.volume = volObjetivo;
    }

    // baja el volumen de source a 0 y lo para
    private IEnumerator FadeOut(AudioSource source)
    {
        if (source == null || !source.isPlaying) yield break;
        float volInicial = source.volume;
        float t = 0f;
        while (t < tiempoFade)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(volInicial, 0f, t / tiempoFade);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    // ajusta el volumen de source hasta volObjetivo en 'duracion' segundos sin parar la reproducción
    // se usa para duck y restore durante la pausa
    private IEnumerator FadeTo(AudioSource source, float volObjetivo, float duracion)
    {
        if (source == null) yield break;
        float volInicial = source.volume;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(volInicial, volObjetivo, t / duracion);
            yield return null;
        }
        source.volume = volObjetivo;
    }

    // espera a que el sonido de derrota termine y luego hace fade in de musicaPlayAgain
    private IEnumerator EsperarYPlayPlayAgain()
    {
        float espera = (sonidoDerrota != null && sonidoDerrota.clip != null)
            ? sonidoDerrota.clip.length
            : 2f;
        yield return new WaitForSecondsRealtime(espera);
        yield return StartCoroutine(FadeIn(musicaPlayAgain, volPlayAgain));
    }

    // para todas las fuentes sin fade (sin corrutinas, para uso inmediato)
    private void PararTodas()
    {
        if (musicaMenu != null)      musicaMenu.Stop();
        if (musicaIngame != null)    musicaIngame.Stop();
        if (musicaPlayAgain != null) musicaPlayAgain.Stop();
        if (sonidoDerrota != null)   sonidoDerrota.Stop();
    }
}
