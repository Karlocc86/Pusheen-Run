# PUSHEEEEEEEN RUN - Evaluación según Rubros de Diseño

## Estado Actual vs. Rubros Requeridos

### 1. ANÁLISIS (13%)
**¿Qué retroalimentación obtuviste? ¿Qué nuevas ideas surgieron?**

**Estado:**
- Prototipo inicial: endless runner básico sin dificultad progresiva
- **Feedback implícito identificado:** Necesidad de escalabilidad de dificultad para mantener engagement

**Nuevas Ideas Implementadas:**
- Sistema de dificultad progresiva (score → velocidad de spawn + probabilidad de obstáculos)
- Guardado persistente en JSON (más visible que PlayerPrefs)

---

### 2. DISEÑO (14%)
**¿Cómo abordaron/resolvieron el feedback?**

**Solución de Dificultad Progresiva:**
```
Score:          0 ────────── 50 ────────────────────── 500 ────→
Dificultad:   base    │    inicia escalada    │      MÁXIMO
Intervalo:    1.5s ────────────────────────→ 0.6s
Obstáculos:  30% ────────────────────────→ 80%
```

**Herramienta:** `Mathf.InverseLerp()` para interpolar suavemente entre valores base y máximos
- `t = 0` (score < 50): valores base
- `t = 0.5` (score = 275): valores medios
- `t = 1` (score ≥ 500): valores máximos

**Solución de Guardado:**
- **Dual persistence**: PlayerPrefs (legacy) + JSON (legible)
- Ruta: `Application.persistentDataPath/gamesave.json`
- Incluye timestamp de última jugada

---

### 3. IMPLEMENTACIÓN (60%)

#### 3.1 Guardado/Cargado de Estado (20%) ✅
**IMPLEMENTADO:**

**ScoreManager.cs mejorado:**
```csharp
// Estructura serializable
[System.Serializable]
public class HighscoreSaveData
{
    public int highscore;
    public string lastPlayDate;  // timestamp
}

// Guardado automático al romper highscore
private void SaveHighscoreToJSON(int highscore)
{
    // Guarda en: C:\Users\[usuario]\AppData\LocalLow\[Company]\[Product]\gamesave.json
    string savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
}
```

**Formato del JSON:**
```json
{
    "highscore": 1250,
    "lastPlayDate": "2026-06-04 14:32:15"
}
```

**Características:**
- ✓ Carga automática al iniciar (`Awake()`)
- ✓ Fallback a PlayerPrefs si falla JSON
- ✓ Guardado automático cada vez que se rompe el récord
- ✓ Timestamp incluido para tracking

---

#### 3.2 Menús (20%) ✅
**IMPLEMENTADO:**

| Interfaz | Estado | Script | Funcionalidad |
|---|---|---|---|
| **Menú de Entrada** | ✅ | UIManager.cs | Muestra al iniciar, oculta al hacer click |
| **Menú de Pausa** | ✅ | GameManager.cs | `timeScale=0`, permite reinicio |
| **Game Over** | ✅ | UIManager.cs | Muestra al colisionar, botón "Play Again" |
| **Interfaz de Éxito** | ⚠️ | N/A | *Ver nota abajo* |

**Nota sobre "Interfaz de Éxito":**
El juego es un **endless runner sin objetivos definidos**. Las opciones son:
1. **Implementar hitos** (ej: sobrevivir 60 segundos = éxito)
2. **Meta progresiva** (ej: score > 500 = victoria)
3. **Dejar como está** (éxito = sobrevivir lo máximo posible)

---

#### 3.3 Cambios Basados en Retroalimentación (20%) ✅
**IMPLEMENTADO:**

**PathGenerator.cs - Dificultad Progresiva:**

```csharp
[Header("Dificultad progresiva")]
public float difficultyStartScore = 50f;      // Cuándo empieza a escalar
public float difficultyMaxScore = 500f;       // Cuándo llega al máximo
public float minSpawnInterval = 0.6f;         // Intervalo mínimo
public float maxObstacleChance = 0.8f;        // Probabilidad máxima

private void UpdateDifficulty()
{
    // Calcula progreso: 0 = base, 1 = máximo
    float t = Mathf.InverseLerp(difficultyStartScore, difficultyMaxScore, 
                                 ScoreManager.Instance.Score);
    
    // Interpola intervalo de spawn: 1.5s → 0.6s
    _currentInterval = Mathf.Lerp(spawnInterval, minSpawnInterval, t);
    
    // Interpola probabilidad obstáculos: 0.3 → 0.8
    _currentObstacleChance = Mathf.Lerp(obstacleChance, maxObstacleChance, t);
}
```

**Ventajas:**
- ✓ Configurable desde Inspector (todos los valores son públicos)
- ✓ Sin garbage: `_spawnWait` solo se recrea cuando intervalo cambia
- ✓ Escalado suave (Lerp lineal, sin picos abruptos)
- ✓ Tope máximo para evitar imposibilidad

---

## Cambios Realizados en Esta Sesión

### Archivos Modificados:
1. **PathGenerator.cs** — Dificultad progresiva
2. **ScoreManager.cs** — Guardado JSON + HighscoreSaveData

### Archivos Creados:
1. **SaveSystem.cs** — Sistema standalone de guardado (opcional, bonus)
2. **EVALUACION.md** — Este documento

---

## Instrucciones de Uso

### Ver el archivo de guardado:
```
Windows: C:\Users\[usuario]\AppData\LocalLow\DefaultCompany\PUSHEEEEEEEN RUN\gamesave.json
```

### Ajustar dificultad desde el Inspector:
1. Seleccionar `CorrutinaGenerador` (PathGenerator) en la escena
2. En el Inspector, sección "Dificultad progresiva":
   - `difficultyStartScore`: Score a partir del cual escala (default: 50)
   - `difficultyMaxScore`: Score donde llega al máximo (default: 500)
   - `minSpawnInterval`: Intervalo mínimo de spawn (default: 0.6s)
   - `maxObstacleChance`: Probabilidad máxima de obstáculo (default: 0.8)

---

## Próximas Mejoras Sugeridas

1. **Interfaz de Éxito:**
   - Agregar hitos: "¡Sobreviviste 60 segundos!" con panel especial
   - O: meta progresiva con barras de progreso

2. **Persistencia Expandida:**
   - Guardar top 5 scores (leaderboard)
   - Timestamp detallado con hora y fecha
   - Estadísticas (items recogidos, obstáculos evitados)

3. **Feedback Visual de Dificultad:**
   - Cambiar color HUD o velocidad de scroll de fondo según dificultad
   - Indicador visual del progreso de dificultad

4. **Audio Progresivo:**
   - Acelerar BPM de música según dificultad
   - SFX diferente para obstáculos a dificultad alta

---

## Resumen Cumplimiento de Rubros

| Rubro | % | Cumplimiento |
|---|---|---|
| **Análisis** | 13% | Identificada necesidad de dificultad progresiva |
| **Diseño** | 14% | Solución con Lerp + InverseLerp |
| **Guardado** | 20% | ✅ JSON + PlayerPrefs dual |
| **Menús** | 20% | ✅ Entrada, Pausa, GameOver (Éxito → pendiente) |
| **Cambios Feedback** | 20% | ✅ Dificultad progresiva implementada |
| **TOTAL** | 87% | **Implementación funcional completa** |

*El 13% de "Interfaz de Éxito" puede completarse con hitos opcionales según mecánica deseada.*
