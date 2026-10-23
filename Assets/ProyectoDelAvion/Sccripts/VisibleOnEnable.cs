using UnityEngine;

public class VisibleOnEnable : MonoBehaviour
{
    [Header("Visibilidad")]
    [SerializeField] private bool startHidden = true;       // Oculto hasta que se active
    [SerializeField] private bool hideWhenDisabled = false; // Se oculta al desactivar el script

    private Renderer[] renderers;

    private void Awake()
    {
        // true = incluye hijos que estén inactivos
        renderers = GetComponentsInChildren<Renderer>(true);

        if (startHidden)
            SetVisible(false);
    }

    private void OnEnable()
    {
        SetVisible(true);
    }

    private void OnDisable()
    {
        if (hideWhenDisabled)
            SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null) return;

        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }
}