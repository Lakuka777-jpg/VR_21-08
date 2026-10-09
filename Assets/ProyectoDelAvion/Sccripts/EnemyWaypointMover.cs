using UnityEngine;

public class EnemyWaypointMover : MonoBehaviour
{
    [Header("Puntos de ruta")]
    [SerializeField] private Transform[] waypoints;   // Los puntos por los que va a pasar
    [SerializeField] private bool loop = true;        // ¿Se repite la ruta?

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float reachDistance = 0.3f; // Distancia para considerar que llegó al punto

    private int currentWaypointIndex = 0;

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        // Mover hacia el punto actual
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Rotar suavemente hacia la dirección del movimiento
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        // ¿Llegó al punto?
        if (Vector3.Distance(transform.position, target.position) < reachDistance)
        {
            currentWaypointIndex++;

            // Si llegó al final
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                {
                    currentWaypointIndex = 0; // Vuelve al principio
                }
                else
                {
                    // Se queda en el último punto
                    currentWaypointIndex = waypoints.Length - 1;
                    enabled = false; // Deja de moverse
                }
            }
        }
    }

    // Dibuja la ruta en el editor para que se vea fácil
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        // Si es loop, une el último con el primero
        if (loop && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }

        // Dibuja esferas en cada punto
        Gizmos.color = Color.yellow;
        foreach (var point in waypoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.25f);
        }
    }
}
