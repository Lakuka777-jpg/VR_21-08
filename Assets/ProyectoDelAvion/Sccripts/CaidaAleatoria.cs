using UnityEngine;

public class CaidaAleatoria : MonoBehaviour
{
    [Header("Tiempo aleatorio antes de caer")]
    [SerializeField] private float tiempoMinimo = 2f;
    [SerializeField] private float tiempoMaximo = 8f;

    [Header("Fuerza")]
    [SerializeField] private Vector3 direccionFuerza = Vector3.forward; // Eje predeterminado (ej: 0,0,1 = Z)
    [SerializeField] private float fuerza = 5f;
    [SerializeField] private ForceMode modoFuerza = ForceMode.VelocityChange;

    [Header("Objeto oculto")]
    [SerializeField] private GameObject objetoOculto; // Debe ser hijo de este objeto para destruirse con él

    [Header("Destrucción")]
    [SerializeField] private float destruirDespues = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        // Empieza sin gravedad y sin moverse por física
        rb.useGravity = false;
        rb.isKinematic = true;

        if (objetoOculto != null)
            objetoOculto.SetActive(false);
    }

    private void Start()
    {
        float espera = Random.Range(tiempoMinimo, tiempoMaximo);
        Invoke(nameof(Activar), espera);
    }

    private void Activar()
    {
        // 1. Activa la gravedad
        rb.isKinematic = false;
        rb.useGravity = true;

        // 2. Aplica la fuerza en el eje elegido
        rb.AddForce(direccionFuerza.normalized * fuerza, modoFuerza);

        // 3. Activa el objeto oculto
        if (objetoOculto != null)
            objetoOculto.SetActive(true);

        // 4. Destruye el objeto tras X segundos
        Destroy(gameObject, destruirDespues);
    }
}