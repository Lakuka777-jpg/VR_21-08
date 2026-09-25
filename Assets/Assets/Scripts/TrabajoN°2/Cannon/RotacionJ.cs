using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RotacionJ : MonoBehaviour
{
    public XRJoystick joystick;

    public float velocidadHorizontal = 50f;
    public float velocidadVertical = 50f;

    public float limiteArriba = 30f;
    public float limiteAbajo = -30f;

    private float rotacionX;
    private float rotacionY;

    [Header("DISPARO")]

    public GameObject objetoDisparo;
    public GameObject objetoNuevo;

    public string tagObjetivo = "ObjetoPeligroso";

    public float cooldown = 4f;
    private bool puedeDisparar = true;

    void Start()
    {
        Vector3 rotacionInicial = transform.localEulerAngles;

        rotacionX = rotacionInicial.x;
        rotacionY = rotacionInicial.y;

        if (rotacionX > 90f)
            rotacionX -= 180f;

        if (rotacionY > 90f)
            rotacionY -= 180f;

        if (objetoDisparo != null)
            objetoDisparo.SetActive(false);
    }

    void Update()
    {
        float horizontal = joystick.value.x;
        float vertical = joystick.value.y;

        rotacionY += horizontal * velocidadHorizontal * Time.deltaTime;
        rotacionX -= vertical * velocidadVertical * Time.deltaTime;

        rotacionX = Mathf.Clamp(rotacionX, limiteAbajo, limiteArriba);

        transform.localRotation = Quaternion.Euler(
            rotacionX,
            rotacionY,
            0f
        );
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