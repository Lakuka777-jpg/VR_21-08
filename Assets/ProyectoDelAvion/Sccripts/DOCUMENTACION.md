# Documentación técnica — Proyecto del Avión (Unity VR)

Este documento describe los scripts y prefabs de la carpeta `Assets/ProyectoDelAvion/Sccripts`: qué hace cada uno, cómo se configuran en el Inspector y cómo se relacionan entre sí.

## Índice

1. [Visión general](#1-visión-general)
2. [Flujo del juego](#2-flujo-del-juego)
3. [Relación entre scripts](#3-relación-entre-scripts)
4. [Scripts de la torreta](#4-scripts-de-la-torreta)
5. [Scripts de enemigos](#5-scripts-de-enemigos)
6. [Scripts de partida y oleadas](#6-scripts-de-partida-y-oleadas)
7. [Scripts de utilidad](#7-scripts-de-utilidad)
8. [Prefabs](#8-prefabs)
9. [Configuración necesaria en Unity](#9-configuración-necesaria-en-unity)
10. [Puntos a mejorar](#10-puntos-a-mejorar)

---

## 1. Visión general

| Aspecto | Detalle |
|---|---|
| Motor | Unity (el código usa `angularDamping`, propio de Unity 6) |
| Lenguaje | C# |
| Plataforma | Realidad virtual con **XR Interaction Toolkit 3.x** |
| Interfaz | TextMeshPro y UI de Unity |
| Escena principal | `AvionEpico` |
| Scripts | 14 archivos `.cs` y 4 prefabs |

La temática (avión, torreta, enemigos, oleadas) se deduce de los nombres de scripts y de la lógica del código. Es un juego de disparo en VR con partidas de tiempo limitado.

## 2. Flujo del juego

```
Menú principal (MainMenuController.PlayGame)
        │  carga la escena "AvionEpico"
        ▼
Inicio de la partida
  ├─ WaveManager.Start  → activa la oleada 1 y arranca el ciclo (cada 30 s una nueva)
  ├─ GameTimer.Start    → empieza la cuenta atrás (120 s por defecto)
  └─ ApagarAlInicii     → apaga los objetos configurados
        ▼
Jugador agarra la torreta (TurretController.OnGrabbed)
  └─ mientras esté agarrada: dispara cada fireRate segundos
        ▼
Cada disparo = Raycast desde el Fire Point
  └─ si golpea un collider con tag "enemigo" → EnemyHitReaction.Hit()
        ▼
Enemigo impactado: se suelta de su ruta, cae, gira, genera efecto y se destruye
        ▼
Tiempo agotado (GameTimer.TerminarJuego)
  ├─ muestra el panel "Fin del juego" frente a la mirada
  ├─ cambia a la cámara final y pausa el juego (Time.timeScale = 0)
  └─ detiene las oleadas (WaveManager.DetenerOleadas)
```

## 3. Relación entre scripts

```
TurretController ───────▶ EnemyHitReaction ───────▶ EnemyWaypointMover
 (dispara raycast)         (Hit(): reacciona)        (se desactiva al caer)

GameTimer ───────────────▶ WaveManager
 (al terminar)              (DetenerOleadas)

Activaraldisparar · RotationCopy · TurretAxisConstraint
 (apoyan visual y mecánicamente a la torreta, sin referencias directas entre sí)
```

Las dependencias directas en código son tres: `TurretController → EnemyHitReaction`, `EnemyHitReaction → EnemyWaypointMover` y `GameTimer → WaveManager`.

---

## 4. Scripts de la torreta

Ubicación: `Torreta/`

### 4.1 `TurretController`

Script principal de la torreta: gestiona el agarre en VR, el disparo, el láser y la detección de impactos.

**Requiere:** `Rigidbody` y `XRGrabInteractable` en el mismo objeto (si `grabInteractable` no se asigna, lo busca con `GetComponent`).

| Campo (Inspector) | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `firePoint` | `Transform` | — | Punto y dirección de origen del rayo. **Obligatorio.** |
| `grabInteractable` | `XRGrabInteractable` | auto | Componente de agarre. |
| `lineRenderer` | `LineRenderer` | auto | Línea del láser; si falta, se crea uno. |
| `objetoAlDisparar` | `GameObject` | — | Objeto que se activa al agarrar y se apaga al soltar. |
| `fireRate` | `float` | `0.15` | Segundos entre disparos. |
| `range` | `float` | `50` | Alcance del rayo en metros. |
| `hitLayers` | `LayerMask` | todas | Capas que puede golpear el rayo. |
| `laserDuration` | `float` | `0.08` | Tiempo que el láser permanece visible. |
| `showRay` | `bool` | `true` | Dibuja el rayo con `Debug.DrawRay`. |
| `freezeRotationX/Y/Z` | `bool` | `false`/`false`/`true` | Ejes de rotación congelados. |

**Comportamiento:**

- **`Awake`**: obtiene el `Rigidbody`, aplica restricciones, se suscribe a los eventos `selectEntered` y `selectExited`, prepara el láser y apaga `objetoAlDisparar`.
- **`ApplyConstraints`**: congela siempre la posición en X, Y y Z; la rotación se congela según los booleanos. Así la torreta solo gira, no se desplaza.
- **`OnGrabbed` / `OnReleased`**: activan o desactivan el estado de disparo (`isGrabbed`) y `objetoAlDisparar`.
- **`Update`**: si está agarrada y ya pasó `nextFireTime`, llama a `Shoot()`. El disparo es **automático** mientras se mantenga agarrada, sin botón.
- **`Shoot`**: lanza un `Physics.Raycast`. Si el collider tiene la etiqueta `enemigo`, busca `EnemyHitReaction` en el objeto o sus padres y llama a `Hit()`. Después dibuja el láser hasta el punto de impacto (o hasta el alcance máximo).
- **`LaserCoroutine`**: muestra el láser `laserDuration` segundos y lo apaga.
- **`OnDestroy`**: se desuscribe de los eventos para evitar fugas.

El color del láser es fijo: naranja-rojizo `(1, 0.35, 0.05)`.

### 4.2 `Activaraldisparar`

Controla el destello del cañón (*muzzle flash*).

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `muzzleFlash` | `GameObject` | auto | Objeto del destello. Si no se asigna, busca un hijo llamado `muzzleflash` (sin distinguir mayúsculas). |
| `desactivarAlDejarDeDisparar` | `bool` | `true` | Apaga el destello al soltar el botón. |
| `duracion` | `float` | `0` | Si es mayor que 0, apaga el destello solo tras esos segundos. |

- Se activa con `Input.GetButtonDown("Fire1")` y se apaga según `duracion` o al soltar el botón.
- Si no encuentra el destello, muestra una advertencia y se desactiva (`enabled = false`).

### 4.3 `TurretAxisConstraint`

Restringe la rotación local de un objeto a **un solo eje libre**.

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `freeAxis` | `enum Axis { X, Y, Z }` | `Y` | Eje en el que sí puede girar. |

Guarda la rotación local inicial en `Start` y, en `LateUpdate`, restablece los otros dos ejes a su valor original. Sirve para que partes de la torreta (por ejemplo, la base) solo giren horizontalmente.

### 4.4 `RotationCopy`

Copia **solo la rotación en Y** de otro objeto (`objetoPrincipal`) y deja X y Z en 0. Se ejecuta en `Update`.

| Campo | Tipo | Descripción |
|---|---|---|
| `objetoPrincipal` | `Transform` | Objeto del que se copia el giro horizontal. **Obligatorio** (si es nulo produce un error en tiempo de ejecución). |

---

## 5. Scripts de enemigos

### 5.1 `EnemyWaypointMover` (`Enemy/`)

Mueve al enemigo por una lista de puntos.

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `waypoints` | `Transform[]` | — | Puntos de la ruta, en orden. |
| `loop` | `bool` | `true` | Repite la ruta al llegar al final. |
| `moveSpeed` | `float` | `3` | Velocidad de avance. |
| `rotateSpeed` | `float` | `5` | Velocidad de giro hacia el destino. |
| `reachDistance` | `float` | `0.3` | Distancia a la que se considera que llegó al punto. |

**Funcionamiento (`Update`):**

1. Avanza hacia el punto actual con `Vector3.MoveTowards`.
2. Gira suavemente con `Quaternion.Slerp` hacia la dirección de movimiento.
3. Al llegar (distancia menor que `reachDistance`), pasa al siguiente punto. Si era el último: vuelve al primero (`loop`) o se queda en el último y se desactiva.

**`OnDrawGizmos`** dibuja en el editor la ruta (líneas cian) y los puntos (esferas amarillas), y une el último con el primero si hay `loop`.

### 5.2 `EnemyHitReaction`

Define qué pasa cuando el enemigo recibe un disparo. Se activa llamando al método público `Hit()`, que solo se ejecuta una vez.

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `prefabAInstanciar` | `GameObject` | — | Efecto u objeto que aparece al impactar. |
| `puntoDeAparicion` | `Transform` | — | Padre y posición del efecto; si falta, usa el propio enemigo. |
| `speedZ` | `float` | `5` | Impulso de avance tras el impacto. |
| `moveSpace` | `Space` | `World` | Dirección del impulso: eje Z del mundo o frente local. |
| `destroyDelay` | `float` | `5` | Segundos hasta destruir al enemigo. |
| `velocidadRotacion` | `Vector3` | `(180, 0, 90)` | Grados por segundo en cada eje. |
| `rotacionAleatoria` | `bool` | `true` | Multiplica cada eje por un valor aleatorio entre -1 y 1. |

**Secuencia de `Hit()`:**

1. Se separa de su padre (`SetParent(null)`).
2. Desactiva `EnemyWaypointMover`.
3. Obtiene o crea un `Rigidbody`, lo hace dinámico, activa la gravedad y quita las restricciones.
4. Aplica un impulso de velocidad (`ForceMode.VelocityChange`) y una velocidad angular.
5. Instancia el prefab del efecto como hijo del punto de aparición.
6. Se destruye tras `destroyDelay` segundos.

---

## 6. Scripts de partida y oleadas

### 6.1 `WaveManager`

Genera oleadas de enemigos a intervalos regulares.

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `oleadasPrefabs` | `GameObject[]` | tamaño 4 | Prefabs de cada oleada, en orden. |
| `tiempoEntreOleadas` | `float` | `30` | Segundos entre oleadas. |
| `puntoSpawn` | `Transform` | — | Posición y rotación de aparición; si falta, usa el origen del mundo. |
| `mostrarDebug` | `bool` | `true` | Escribe mensajes en la consola. |

**Funcionamiento:**

- **`Start`**: valida que haya prefabs, activa la oleada 1 inmediatamente y arranca la corrutina `CicloOleadas`.
- **`CicloOleadas`**: espera `tiempoEntreOleadas`, avanza el índice y activa la siguiente, hasta agotar la lista.
- **`ActivarOleada(indice)`**: instancia el prefab, lo activa y llama a `HacerVisible`. Si el prefab es nulo, registra una advertencia.
- **`HacerVisible`**: activa todos los `Renderer` de la instancia y sus objetos, incluso los que estaban inactivos.
- **`DetenerOleadas()`** (público): marca el sistema como inactivo y detiene las corrutinas.

### 6.2 `GameTimer`

Controla la duración de la partida y la pantalla final.

| Campo | Tipo | Valor por defecto | Descripción |
|---|---|---|---|
| `tiempoLimite` | `float` | `120` | Duración de la partida en segundos. |
| `panelFinJuego` | `GameObject` | — | Panel de fin de juego. |
| `textoFinJuego` | `TextMeshProUGUI` | — | Texto que muestra "Fin del juego". |
| `cabezaJugador` | `Transform` | `Camera.main` | Referencia a la cabeza del jugador. |
| `distanciaFrente` | `float` | `1.8` | Distancia del panel frente a la mirada. |
| `alturaOffset` | `float` | `0` | Ajuste de altura (0 = altura de los ojos). |
| `camaraJuego` | `Camera` | `Camera.main` | Cámara de la partida. |
| `camaraFinal` | `Camera` | — | Cámara a la que se cambia al terminar. |

**Funcionamiento:**

- **`Start`**: inicia la cuenta atrás, oculta el panel, resuelve las cámaras (usa `Camera.main` si no se asignan) y apaga la cámara final.
- **`Update`**: resta `Time.deltaTime`; al llegar a 0 llama a `TerminarJuego`. Una vez terminado, el panel sigue la mirada.
- **`TerminarJuego`**: muestra el panel y el texto, pone `Time.timeScale = 0`, cambia de cámara y detiene las oleadas con `WaveManager.DetenerOleadas()`.
- **`CambiarCamara`**: desactiva el `AudioListener` y la cámara de juego, activa la cámara final y la usa como nueva referencia de cabeza (evita dos `AudioListener` activos).
- **`SeguirMirada`**: coloca el panel delante de la cabeza y lo orienta hacia el jugador (*billboard*).
- **`ReiniciarJuego()`** (público): restablece `Time.timeScale = 1` y recarga la escena activa. Está pensado para un botón.

### 6.3 `MainMenuController`

Controla el menú principal. Sus métodos públicos se enlazan a botones de la UI.

| Método | Acción |
|---|---|
| `PlayGame()` | Carga la escena `AvionEpico`. |
| `QuitGame()` | Cierra la aplicación; en el editor detiene el modo *Play*. |

---

## 7. Scripts de utilidad

| Script | Qué hace | Campos |
|---|---|---|
| `ApagarAlInicii` | Desactiva una lista de objetos en `Awake` y de nuevo en `Start`. | `objetosAApagar` (`GameObject[]`) |
| `CaidaAleatoria` | Tras un tiempo aleatorio, activa gravedad, aplica una fuerza, muestra un objeto oculto y se destruye. | `tiempoMinimo` (2), `tiempoMaximo` (8), `direccionFuerza` (`forward`), `fuerza` (5), `modoFuerza` (`VelocityChange`), `objetoOculto`, `destruirDespues` (5) |
| `VisibleOnEnable` | Oculta los `Renderer` del objeto y sus hijos al iniciar y los muestra al activar el componente. | `startHidden` (`true`), `hideWhenDisabled` (`false`) |
| `ActivationDebugger` | Al activarse registra un error con el nombre del objeto y la traza de pila, para descubrir qué lo activó. | — |

**Detalles de `CaidaAleatoria`:** empieza con el `Rigidbody` en modo cinemático y sin gravedad (lo crea si no existe). Pasado el tiempo aleatorio (`Invoke`), pasa a dinámico, aplica la fuerza en la dirección normalizada, activa `objetoOculto` y llama a `Destroy` con `destruirDespues`.

---

## 8. Prefabs

Ubicación: `Elementos/`

| Prefab | Contenido |
|---|---|
| `GameManager` | Objeto con el script `GameTimer`. En el prefab, `tiempoLimite` está en **2**, valor que parece de pruebas (el valor por defecto del código es 120). |
| `WaveManager` | Objeto con el script `WaveManager`. `tiempoEntreOleadas` = 30 y la lista `oleadasPrefabs` viene **vacía**: hay que asignarla en la escena. |
| `Canvas` | Interfaz con un panel y un texto TextMeshPro, usada para el panel de fin de juego. |
| `EventSystem` | Sistema de eventos de la UI. |

## 9. Configuración necesaria en Unity

| Elemento | Requisito |
|---|---|
| Etiqueta (*Tag*) | Debe existir **`enemigo`** (en minúscula) y estar en los colliders de los enemigos. |
| Escena | **`AvionEpico`** debe estar en *Build Settings* para que el botón de jugar funcione. |
| Entrada | `Activaraldisparar` usa el botón **`Fire1`** del *Input Manager* clásico. Si el proyecto usa solo el nuevo Input System, habrá que habilitar ambos o adaptar el script. |
| Torreta | `Rigidbody` + `XRGrabInteractable` + `Fire Point` asignado. |
| Cámaras | Asignar `camaraFinal` y, preferiblemente, `cabezaJugador` en `GameTimer`. |
| Oleadas | Asignar los 4 prefabs en `WaveManager`. |

## 10. Puntos a mejorar

Observaciones surgidas al revisar el código. Ninguna impide ejecutar el proyecto, pero conviene tenerlas presentes.

| # | Observación | Script | Sugerencia |
|---|---|---|---|
| 1 | `FindObjectOfType` está obsoleto en Unity 6 y genera una advertencia. | `GameTimer` | Usar `FindFirstObjectByType<WaveManager>()` o una referencia asignada en el Inspector. |
| 2 | Si no hay `XRGrabInteractable` en el objeto ni asignado, `grabInteractable` es nulo y falla al suscribir los eventos. | `TurretController` | Validar con un mensaje claro y desactivar el script. |
| 3 | `HacerVisible` fuerza todos los hijos y renderers a estar activos, lo que anula objetos que se querían ocultar a propósito. | `WaveManager` | Limitar a los renderers realmente necesarios. |
| 4 | Con `startHidden`, `Awake` oculta y `OnEnable` vuelve a mostrar casi al instante si el objeto empieza activo. | `VisibleOnEnable` | Usarlo con el **componente** desactivado desde el inicio, o separar la lógica de ocultar y mostrar. |
| 5 | Nombres con errores tipográficos: carpeta `Sccripts`, clase `ApagarAlInicii`. | Varios | Renombrar desde el editor de Unity para conservar las referencias. |
| 6 | El prefab `GameManager` tiene `tiempoLimite = 2`. | `GameTimer` | Confirmar que el valor de partida real sea el deseado (por ejemplo, 120). |
| 7 | `Shader.Find("Sprites/Default")` puede no incluirse en una *build* si ningún material lo usa. | `TurretController` | Asignar un material al `LineRenderer` en el Inspector. |
| 8 | `EnemyWaypointMover.cs` está guardado en una codificación distinta (Windows-1252) a la del resto (UTF-8), por lo que los acentos de los comentarios pueden verse mal. | `EnemyWaypointMover` | Guardarlo como UTF-8. |
| 9 | Se registran mensajes de depuración en cada impacto y al agarrar/soltar la torreta. | `TurretController` | Condicionarlos a un booleano de depuración. |
