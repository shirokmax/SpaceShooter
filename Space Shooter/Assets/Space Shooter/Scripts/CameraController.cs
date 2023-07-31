using UnityEngine;

namespace SpaceShooter
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private Transform m_Target;

        [SerializeField] private float m_Speed;
        [SerializeField] private float m_AngularSpeed;

        [SerializeField] private float m_ZOffset;
        [SerializeField] private float m_ForwardOffset;

        private void Start()
        {
            if (m_Target != null) 
                m_camera.transform.position = m_Target.position + m_Target.up * m_ForwardOffset;
        }

        private void FixedUpdate()
        {
            if (m_camera == null || m_Target == null) return;

            Vector2 camPos = m_camera.transform.position;

            Vector2 targetPos = m_Target.position + m_Target.up * m_ForwardOffset;

            Vector2 camNewPos = Vector2.Lerp(camPos, targetPos, m_Speed * Time.fixedDeltaTime);

            m_camera.transform.position = new Vector3(camNewPos.x, camNewPos.y, m_ZOffset);

            if (m_AngularSpeed > 0)
            {
                m_camera.transform.rotation = Quaternion.Slerp(m_camera.transform.rotation, m_Target.rotation, m_AngularSpeed * Time.fixedDeltaTime);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            m_Target = newTarget;
        }
    }
}
