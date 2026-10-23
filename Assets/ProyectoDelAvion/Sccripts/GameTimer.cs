using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Tiempo")]
    public float tiempoLimite = 120f; // 2 minutos

    [Header("UI")]
    public GameObject panelFinJuego;
    public TextMeshProUGUI textoFinJuego;

    [Header("Seguimiento de la cabeza")]
    public Transform cabezaJugador;          // Main Camera (o XR Origin → Camera Offset → Main Camera)
    public float distanciaFrente = 1.8f;     // Distancia a la que aparece el panel
    public float alturaOffset = 0f;          // Ajuste de altura (0 = a la altura de los ojos)

    [Header("Cambio de cámara")]
    public Camera camaraJuego;   // Cámara actual (si la dejas vacía, usa la Main Camera)
    public Camera camaraFinal;   // Cámara a la que se cambia al terminar el timer

    private float tiempoRestante;
    private bool juegoTerminado = false;

    void Start()
    {
        tiempoRestante = tiempoLimite;

        if (panelFinJuego != null)
            panelFinJuego.SetActive(false);

        // Si no asignaste la cámara, la buscamos automáticamente
        if (cabezaJugador == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                cabezaJugador = cam.transform;
        }

        if (camaraJuego == null)
            camaraJuego = Camera.main;

        if (camaraFinal != null)
            camaraFinal.gameObject.SetActive(false); // Empieza apagada
    }

    void Update()
    {
        if (juegoTerminado)
        {
            // Mientras el juego está terminado, el panel sigue la mirada
            SeguirMirada();
            return;
        }

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            TerminarJuego();
        }
    }

    void TerminarJuego()
    {
        juegoTerminado = true;

        if (panelFinJuego != null)
            panelFinJuego.SetActive(true);

        if (textoFinJuego != null)
            textoFinJuego.text = "Fin del juego";

        // Opcional: pausar el juego
        Time.timeScale = 0f;

        // Cambia a la cámara final
        CambiarCamara();

        // Posicionar inmediatamente
        SeguirMirada();

        FindObjectOfType<WaveManager>()?.DetenerOleadas();
    }

    void CambiarCamara()
    {
        if (camaraFinal == null) return;

        // Evita tener dos AudioListener activos
        AudioListener listenerViejo = camaraJuego != null ? camaraJuego.GetComponent<AudioListener>() : null;
        if (listenerViejo != null) listenerViejo.enabled = false;

        if (camaraJuego != null)
            camaraJuego.enabled = false;

        camaraFinal.gameObject.SetActive(true);

        // El panel ahora sigue a la cámara nueva
        cabezaJugador = camaraFinal.transform;
    }

    void SeguirMirada()
    {
        if (cabezaJugador == null || panelFinJuego == null) return;

        // Posición: delante de la cabeza + offset de altura
        Vector3 posicion = cabezaJugador.position
                         + cabezaJugador.forward * distanciaFrente
                         + Vector3.up * alturaOffset;

        panelFinJuego.transform.position = posicion;

        // Rotación: mirando hacia el jugador (billboard)
        panelFinJuego.transform.rotation = Quaternion.LookRotation(
            panelFinJuego.transform.position - cabezaJugador.position
        );
    }

    // Botón de reiniciar (opcional)
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}