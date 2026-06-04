# PUSHEEEEEEEN RUN - Mejoras de Jugabilidad

## Cambios Implementados

### 1. **Física Mejorada del Salto** (Pusheen.cs)

#### Antes:
```csharp
_fuerzaSalto = 8f;  // fuerza fija
```

#### Después:
```csharp
_fuerzaSalto = 10f;        // +25% más responsivo
_maxVelocidad = 15f;       // limita velocidad de caída
_rb.gravityScale = 2f;     // cae más rápido
_rb.linearDamping = 0.5f;  // caídas más suaves
```

**Efecto:**
- ✅ Saltos más altos y ágiles
- ✅ Caídas suaves (sin acelerar infinitamente)
- ✅ Mejor control en el aire
- ✅ Sensación más "responsiva" al jugar

---

### 2. **Feedback Visual al Recoger Postres** (Pusheen.cs)

**Nuevo:**
```csharp
private void PlayCollectEffect()
{
    StartCoroutine(FlashColor(Color.yellow, 0.15f));
}
```

**Efecto:**
- ✅ Pusheen hace flash amarillo (0.15s) al recoger postre
- ✅ Feedback visual claro de colisión exitosa
- ✅ Más satisfactorio para el jugador

---

### 3. **Highscore Visible en Menú** (UIManager.cs)

**Antes:**
- Solo se mostraba score actual en gameplay
- No había referencia al récord anterior

**Después:**
```csharp
public TMP_Text textoHighscoreMenu;

void Start()
{
    if (textoHighscoreMenu != null && ScoreManager.Instance != null)
        textoHighscoreMenu.text = "Best: " + ScoreManager.Instance.Highscore;
}
```

**Efecto:**
- ✅ Al entrar al juego ves tu mejor score
- ✅ Te motiva a superarlo
- ✅ Persiste entre sesiones (guardado en JSON)

---

### 4. **Indicador de Dificultad en Pantalla** (UIManager.cs)

**Nuevo:**
```csharp
// Muestra durante el juego: "Difficulty: Easy" → "Difficulty: INSANE"
private string ObtenerNivelDificultad(float score)
{
    if (score < 50) return "Easy";
    if (score < 150) return "Medium";
    if (score < 300) return "Hard";
    if (score < 500) return "Very Hard";
    return "INSANE";
}
```

**Niveles de Dificultad:**
| Score | Dificultad | Obstáculos | Intervalo |
|---|---|---|---|
| 0-50 | Easy | 30% | 1.5s |
| 50-150 | Medium | 40% | 1.3s |
| 150-300 | Hard | 55% | 1.0s |
| 300-500 | Very Hard | 70% | 0.8s |
| 500+ | INSANE | 80% | 0.6s |

**Efecto:**
- ✅ Jugador sabe en qué nivel está
- ✅ Feedback visual del progreso
- ✅ Motivación para seguir jugando

---

### 5. **Mejor Lógica de Colisión** (Pusheen.cs)

**Antes (ROTO):**
```csharp
if (!other.CompareTag("Pusheen")) return;  // ❌ Lógica invertida
```

**Después:**
```csharp
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
```

**Efecto:**
- ✅ Colisiones funcionan correctamente
- ✅ Items se recogen sin errores
- ✅ Game Over se dispara al tocar obstáculos

---

## Resumen de Mejoras

| Característica | Antes | Después | Impacto |
|---|---|---|---|
| **Feel del salto** | Rígido | Suave y ágil | Alto |
| **Feedback items** | Silent | Flash visual | Medio |
| **Highscore visible** | No | Sí, en menú | Medio |
| **Indicador dificultad** | No | Sí, en pantalla | Medio |
| **Física consistente** | Buggy | Correcta | Alto |

---

## Cómo Ajustar Más (desde Inspector)

### En **Pusheen** (el personaje):
```
Fuerza Salto: 10 (aumenta para saltos más altos)
Max Velocidad: 15 (aumenta para caídas más rápidas)
Drag Factor: 0.5 (disminuye para caídas más rápidas)
```

### En **PathGenerator** (generador de obstáculos):
```
Difficulty Start Score: 50 (cuándo empieza a escalar)
Difficulty Max Score: 500 (cuándo llega al máximo)
Min Spawn Interval: 0.6 (qué tan rápido mínimo)
Max Obstacle Chance: 0.8 (probabilidad máxima)
```

---

## Próximas Mejoras Sugeridas

1. **Audio:**
   - SFX al recoger postres (pequeño "ding" o "pop")
   - Música que acelera con la dificultad
   - SFX de salto

2. **Efectos Visuales:**
   - Partículas al recoger postres
   - Screenshake pequeño al obstáculo
   - Trail effect en Pusheen mientras salta

3. **Gameplay:**
   - Poder (invencibilidad temporal, magnet items)
   - Enemigos que persiguen
   - Plataformas para saltos encadenados

4. **UI/UX:**
   - Animación de transición menú → juego
   - Leaderboard local
   - Estadísticas (items recogidos, mejor streak)

---

## Cómo Testear los Cambios

1. **Abre el juego** en el editor de Unity
2. **Presiona Play**
3. **Click en PLAY** para entrar a la partida
4. **Observa:**
   - ✅ Pusheen hace flash amarillo al recoger postres
   - ✅ Arriba dice "High Score: [tu máximo]"
   - ✅ Derecha/abajo dice "Difficulty: Easy/Medium/Hard/Very Hard/INSANE"
   - ✅ El salto se siente más responsivo
   - ✅ La caída es suave (no acelera infinitamente)

---

**Versión:** Mejora de Jugabilidad v1.0  
**Fecha:** 2026-06-04
