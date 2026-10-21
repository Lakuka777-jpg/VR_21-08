using UnityEngine;

public class Movimiento : MonoBehaviour
{
    [Header("Configuracion")]
    public Vector3 direccion = Vector3.forward;

    public float distanciaMinima = 1f;
    public float distanciaMaxima = 3f;

    public float velocidad = 2f;

    private Vector3 posicionInicial;
    private Vector3 destino;
    private float distanciaActual;

    void Start()
    {
        posicionInicial = transform.position;
        NuevaDistancia();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destino) < 0.01f)
        {
            NuevaDistancia();
        }
    }

    void NuevaDistancia()
    {
        distanciaActual = Random.Range(
            distanciaMinima,
            distanciaMaxima
        );

        // Alterna entre adelante y atras
        if (destino == posicionInicial)
        {
            destino = posicionInicial
                + direccion.normalized * distanciaActual;
        }
        else
        {
            destino = posicionInicial;
        }
    }
}