using UnityEngine;

namespace SpaceShooter
{
    public class LevelBoundary : MonoSingleton<LevelBoundary>
    {
        [SerializeField] private float m_Radius;
        public float Radius => m_Radius;

        [SerializeField] private Camera m_MapCamera;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, m_Radius);
        }

        private void OnValidate()
        {
            if (gameObject.activeInHierarchy == false) return;

            transform.localScale = Vector3.one * m_Radius * 5.7f;

            if (m_MapCamera != null)
                m_MapCamera.orthographicSize = m_Radius;
        }
#endif
    }
}
