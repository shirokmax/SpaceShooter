using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, m_Speed * Time.fixedDeltaTime);
    }
}
