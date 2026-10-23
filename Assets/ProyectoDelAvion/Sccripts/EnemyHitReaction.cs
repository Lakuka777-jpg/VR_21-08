using UnityEngine;

public class EnemyHitReaction : MonoBehaviour
{
    [Header("Objeto que se crea al impacto")]
    [SerializeField] private GameObject prefabAInstanciar;
    [SerializeField] private Transform puntoDeAparicion;

    [Header("Movimiento tras el impacto")]
    [SerializeField] private float speedZ = 5f;
    [SerializeField] private Space moveSpace = Space.World;
    [SerializeField] private float destroyDelay = 5f;

    [Header("Rotación al caer")]
    [SerializeField] private Vector3 velocidadRotacion = new Vector3(180f, 0f, 90f); // Grados por segundo en cada eje
    [SerializeField] private bool rotacionAleatoria = true; // Varía el giro en cada enemigo

    private bool isHit = false;

    public void Hit()
    {
        if (isHit) return;
        isHit = true;

        // 1. Deja de ser hijo
        transform.SetParent(null);

        // 2. Detiene el movimiento por waypoints
        EnemyWaypointMover mover = GetComponent<EnemyWaypointMover>();
        if (mover != null) mover.enabled = false;

        // 3. Activa la física: gravedad + avance en Z + giro
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None; // Permite rotar libremente
        rb.angularDamping = 0f; // Si usas Unity 2022 o anterior, cambia a: rb.angularDrag = 0f;

        Vector3 direccion = (moveSpace == Space.World) ? Vector3.forward : transform.forward;
        rb.AddForce(direccion * speedZ, ForceMode.VelocityChange);

        Vector3 giro = velocidadRotacion * Mathf.Deg2Rad; // La velocidad angular se da en radianes por segundo
        if (rotacionAleatoria)
        {
            giro = new Vector3(
                giro.x * Random.Range(-1f, 1f),
                giro.y * Random.Range(-1f, 1f),
                giro.z * Random.Range(-1f, 1f)
            );
        }
        rb.angularVelocity = giro;

        // 4. Crea el objeto como hijo del objeto vacío
        if (prefabAInstanciar != null)
        {
            Transform padre = puntoDeAparicion != null ? puntoDeAparicion : transform;
            Instantiate(prefabAInstanciar, padre.position, padre.rotation, padre);
        }

        // 5. Destruye el enemigo (y sus hijos) tras X segundos
        Destroy(gameObject, destroyDelay);
    }
}