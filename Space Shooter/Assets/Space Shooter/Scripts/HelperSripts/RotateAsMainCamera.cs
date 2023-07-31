using UnityEngine;

public class RotateAsMainCamera : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
    }
}