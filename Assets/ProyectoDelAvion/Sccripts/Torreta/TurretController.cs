using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class TurretController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject objetoAlDisparar; // <- NUEVO

    [Header("Configuración de disparo")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private float laserDuration = 0.08f;

    [Header("Debug")]
    [SerializeField] private bool showRay = true;

    [Header("Restricciones de rotación")]
    [SerializeField] private bool freezeRotationX = false;
    [SerializeField] private bool freezeRotationY = false;
    [SerializeField] private bool freezeRotationZ = true;

    // Color naranja-rojizo constante
    private readonly Color laserColor = new Color(1f, 0.35f, 0.05f); // Naranja rojizo intenso

    private Rigidbody rb;
    private bool isGrabbed = false;
    private float nextFireTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ApplyConstraints();

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            SetupLineRenderer();
        }
        else
        {
            // Aseguramos el color aunque lo hayas asignado manualmente
            ApplyLaserColor();
        }

        lineRenderer.enabled = false;

        if (objetoAlDisparar != null)
            objetoAlDisparar.SetActive(false); // <- NUEVO
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = 0.025f;
        lineRenderer.endWidth = 0.012f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        ApplyLaserColor();
    }

    private void ApplyLaserColor()
    {
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
    }

    private void ApplyConstraints()
    {
        if (rb == null) return;

        RigidbodyConstraints constraints = RigidbodyConstraints.None;

        constraints |= RigidbodyConstraints.FreezePositionX;
        constraints |= RigidbodyConstraints.FreezePositionY;
        constraints |= RigidbodyConstraints.FreezePositionZ;

        if (freezeRotationX) constraints |= RigidbodyConstraints.FreezeRotationX;
        if (freezeRotationY) constraints |= RigidbodyConstraints.FreezeRotationY;
        if (freezeRotationZ) constraints |= RigidbodyConstraints.FreezeRotationZ;

        rb.constraints = constraints;
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
        if (isGrabbed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        if (objetoAlDisparar != null)
            objetoAlDisparar.SetActive(true); // <- NUEVO

        Debug.Log("Torreta agarrada → Disparo activado");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;

        if (objetoAlDisparar != null)
            objetoAlDisparar.SetActive(false); // <- NUEVO

        Debug.Log("Torreta soltada → Disparo desactivado");
    }

    private void Shoot()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("Falta asignar el Fire Point en la torreta");
            return;
        }

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        Vector3 endPoint = firePoint.position + firePoint.forward * range;

        if (Physics.Raycast(ray, out hit, range, hitLayers))
        {
            endPoint = hit.point;

            if (hit.collider.CompareTag("enemigo"))
            {
                Debug.Log($"¡IMPACTO en enemigo! → {hit.collider.gameObject.name}  (distancia: {hit.distance:F2}m)");

                EnemyHitReaction reaction = hit.collider.GetComponentInParent<EnemyHitReaction>();
                if (reaction != null)
                    reaction.Hit();
            }
        }

        ShowLaser(firePoint.position, endPoint);

        if (showRay)
        {
            Debug.DrawRay(firePoint.position, firePoint.forward * range, laserColor, fireRate);
        }
    }

    private void ShowLaser(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null) return;

        StopAllCoroutines();
        StartCoroutine(LaserCoroutine(start, end));
    }

    private IEnumerator LaserCoroutine(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(laserDuration);

        lineRenderer.enabled = false;
    }
}