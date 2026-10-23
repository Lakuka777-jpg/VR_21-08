using UnityEngine;

public class ActivationDebugger : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.LogError($"'{name}' se ACTIVÓ", this);
        Debug.Log(System.Environment.StackTrace);
    }
}