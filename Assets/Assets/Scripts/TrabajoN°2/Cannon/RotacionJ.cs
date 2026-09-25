using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RotacionJ : MonoBehaviour
{
    public XRJoystick joystick;

    [Header("OBJETOS DE ROTACIÓN")]
    public Transform objetoHorizontal;
    public Transform objetoVertical;

    [Header("VELOCIDAD")]
    public float velocidadHorizontal = 50f;
    public float velocidadVertical = 50f;

    [Header("LÍMITES HORIZONTALES")]
    public float limiteHorizontalIzquierda = -90f;
    public float limiteHorizontalDerecha = 90f;

    [Header("LÍMITES VERTICALES")]
    public float limiteArriba = 30f;
    public float limiteAbajo = -30f;

    // Rotación relativa a la posición inicial
    private float rotacionHorizontal = 0f;
    private float rotacionVertical = 0f;

    // Rotación inicial de cada objeto
    private Quaternion rotacionInicialHorizontal;
    private Quaternion rotacionInicialVertical;

    [Header("DISPARO")]
    public GameObject objetoDisparo;
    public GameObject objetoNuevo;

    public string tagObjetivo = "ObjetoPeligroso";

    private bool puedeDisparar = true;

    void Start()
    {
        // La rotación actual se considera 0
        if (objetoHorizontal != null)
            rotacionInicialHorizontal = objetoHorizontal.localRotation;

        if (objetoVertical != null)
            rotacionInicialVertical = objetoVertical.localRotation;

        if (objetoDisparo != null)
            objetoDisparo.SetActive(false);
    }

    void Update()
    {
        float horizontal = joystick.value.x;
        float vertical = joystick.value.y;

        // =========================
        // HORIZONTAL → ROTACIÓN Z
        // =========================

        rotacionHorizontal -= horizontal *
                              velocidadHorizontal *
                              Time.deltaTime;

        rotacionHorizontal = Mathf.Clamp(
            rotacionHorizontal,
            limiteHorizontalIzquierda,
            limiteHorizontalDerecha
        );

        if (objetoHorizontal != null)
        {
            objetoHorizontal.localRotation =
                rotacionInicialHorizontal *
                Quaternion.Euler(
                    0f,
                    0f,
                    rotacionHorizontal
                );
        }

        // =========================
        // VERTICAL → ROTACIÓN X
        // =========================

        rotacionVertical -= vertical *
                            velocidadVertical *
                            Time.deltaTime;

        rotacionVertical = Mathf.Clamp(
            rotacionVertical,
            limiteAbajo,
            limiteArriba
        );

        if (objetoVertical != null)
        {
            objetoVertical.localRotation =
                rotacionInicialVertical *
                Quaternion.Euler(
                    rotacionVertical,
                    0f,
                    0f
                );
        }
    }

    public void Disparar()
    {
        if (!puedeDisparar)
            return;

        puedeDisparar = false;

        if (objetoDisparo != null)
            objetoDisparo.SetActive(true);

        GameObject objetivo = GameObject.FindGameObjectWithTag(tagObjetivo);

        if (objetivo != null)
        {
            objetivo.SetActive(false);

            if (objetoNuevo != null)
                objetoNuevo.SetActive(true);
        }

        Invoke(nameof(TerminarDisparo), 4f);
    }

    void TerminarDisparo()
    {
        if (objetoDisparo != null)
            objetoDisparo.SetActive(false);

        puedeDisparar = true;
    }
}
