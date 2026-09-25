using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ElevarWheel : MonoBehaviour
{
    public Transform objeto;

    public XRKnob xRKnob;

    public BatterySocket batterySocket;

    [Header("Altura")]
    public float alturaMaxima = 5f;

    private float posicionInicialY;

    void Start()
    {
        // Guardar la posición actual como punto 0
        posicionInicialY = objeto.localPosition.z;
    }

    void Update()
    {
        // Si no hay batería, no hacer nada
        if (!batterySocket.bateriaConectada)
        {
            return;
        }

        // Obtener valor del Knob
        float valor = xRKnob.value;

        // Trabajar en décimas
        valor = Mathf.Round(valor * 10f) / 10f;

        // Convertir 0-1 a 0-alturaMaxima
        float altura = Mathf.Lerp(
            0f,
            alturaMaxima,
            valor
        );

        // Mantener X y Z y modificar solamente Y
        Vector3 posicion = objeto.localPosition;

        posicion.z = posicionInicialY + altura;

        objeto.localPosition = posicion;
    }
}