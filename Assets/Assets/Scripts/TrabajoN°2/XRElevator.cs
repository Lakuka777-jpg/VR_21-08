using UnityEngine;

public class XRElevator : MonoBehaviour
{
    public Transform xrOrigin;

    public float alturaMinima = 0f;
    public float alturaMaxima = 5f;

    private float alturaAnterior;

    void Start()
    {
        alturaAnterior = transform.position.y;
    }

    public void MoverAscensor(float valor)
    {
        // Convertimos el valor del Knob (0-1) a altura
        float nuevaAltura = Mathf.Lerp(alturaMinima, alturaMaxima, valor);

        Vector3 posicion = transform.position;
        posicion.y = nuevaAltura;

        transform.position = posicion;

        // Cuánto se movió la plataforma
        float deltaY = nuevaAltura - alturaAnterior;

        // Mover al jugador exactamente lo mismo
        if (xrOrigin != null)
        {
            xrOrigin.position += new Vector3(0, deltaY, 0);
        }

        alturaAnterior = nuevaAltura;
    }
}