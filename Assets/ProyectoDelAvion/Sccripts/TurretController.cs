using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TurretController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform firePoint;          // Punto desde donde sale el raycast (cañón)
    [SerializeField] private XRGrabInteractable grabInteractable;

    [Header("Configuración de disparo")]
    [SerializeField] private float fireRate = 0.15f;       // Tiempo entre disparos
    [SerializeField] private float range = 50f;            // Alcance del raycast
    [SerializeField] private LayerMask hitLayers = ~0;     // Capas que puede detectar (todo por defecto)

    [Header("Debug")]
    [SerializeField] private bool showRay = true;
    [SerializeField] private Color rayColor = Color.red;

    private bool isGrabbed = false;
    private float nextFireTime = 0f;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // Suscribirse a los eventos de agarre
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void Update()
    {
        // Solo dispara mientras esté agarrada
        if (isGrabbed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        Debug.Log("Torreta agarrada ? Disparo activado");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        Debug.Log("Torreta soltada ? Disparo desactivado");
    }

    private void Shoot()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("Falta asignar el Fire Point en la torreta");
            return;
        }

        // Raycast
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitLayers))
        {
            // Detectamos si el objeto tiene el tag "enemigo"
            if (hit.collider.CompareTag("enemigo"))
            {
                Debug.Log($"¡IMPACTO en enemigo! ? {hit.collider.gameObject.name}  (distancia: {hit.distance:F2}m)");

                // Aquí puedes añadir daño, efectos, etc.
                // Ejemplo:
                // hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(10);
            }
            else
            {
                // Opcional: ver qué más está tocando el raycast
                // Debug.Log($"Raycast tocó: {hit.collider.name}");
            }
        }

        // Visualización del raycast en el editor
        if (showRay)
        {
            Debug.DrawRay(firePoint.position, firePoint.forward * range, rayColor, fireRate);
        }
    }
}