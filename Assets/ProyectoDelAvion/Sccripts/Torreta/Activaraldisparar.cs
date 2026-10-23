using UnityEngine;

public class Activaraldisparar : MonoBehaviour
{
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private bool desactivarAlDejarDeDisparar = true;

    [Header("Opcional: apagar automáticamente tras X segundos (0 = desactivado)")]
    [SerializeField] private float duracion = 0f;

    void Start()
    {
        // Si no lo asignaste en el Inspector, lo busca como hijo por nombre
        if (muzzleFlash == null)
        {
            Transform hijo = FindChildByName(transform, "muzzleflash");
            if (hijo != null) muzzleFlash = hijo.gameObject;
        }

        if (muzzleFlash == null)
        {
            Debug.LogWarning("No se encontró el objeto muzzleflash en " + name, this);
            enabled = false;
            return;
        }

        muzzleFlash.SetActive(false); // empieza apagado
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            muzzleFlash.SetActive(true);

            if (duracion > 0f)
            {
                CancelInvoke(nameof(Apagar));
                Invoke(nameof(Apagar), duracion);
            }
        }

        if (desactivarAlDejarDeDisparar && duracion <= 0f && Input.GetButtonUp("Fire1"))
        {
            Apagar();
        }
    }

    void Apagar()
    {
        muzzleFlash.SetActive(false);
    }

    // Busca entre todos los hijos (incluso desactivados), sin distinguir mayúsculas
    Transform FindChildByName(Transform padre, string nombre)
    {
        foreach (Transform t in padre.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.ToLower() == nombre.ToLower())
                return t;
        }
        return null;
    }
}