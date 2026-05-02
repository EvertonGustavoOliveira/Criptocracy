using UnityEngine;
using UnityEngine.InputSystem;

public class SomPassos : MonoBehaviour
{
    public AudioSource somPassos;

    //var keyboard = Keyboard.current;

    void Update()
    {
        if (Keyboard.current.dKey.isPressed || Keyboard.current.aKey.isPressed){
            somPassos.enabled = true;
        }
        else{
            somPassos.enabled = false;
        }
    }
}
