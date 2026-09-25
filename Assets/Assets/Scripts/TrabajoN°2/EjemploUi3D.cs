using UnityEngine;
using UnityEngine.XR.Content.Interaction;
public class EjemploUi3D : MonoBehaviour
{
    public XRJoystick joystick;
    public XRLever xRLever;
    public XRSlider xRSlider;
    public XRKnob xRKnob;

    void Update()
    {
        print(joystick.value); 
        print(xRLever.value);   
        print(xRSlider.value);
        print(xRKnob.value);
    }
}
