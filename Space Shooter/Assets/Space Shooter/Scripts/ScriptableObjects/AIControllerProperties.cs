using UnityEngine;

namespace SpaceShooter
{
    [CreateAssetMenu]
    public class AIControllerProperties : ScriptableObject
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_NavigationLinear;
        public float NavigationLinear => m_NavigationLinear;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_NavigationAngular;
        public float NavigationAngular => m_NavigationAngular;

        [Space]
        [SerializeField][Min(0.0f)] private float m_RandomSelectMovePointTime;
        public float RandomSelectMovePointTime => m_RandomSelectMovePointTime;

        [SerializeField][Min(0.0f)] private float m_FindNewTargetTime;
        public float FindNewTargetTime => m_FindNewTargetTime;

        [Space]
        [SerializeField][Min(0.0f)] private float m_MaxFireRadius;
        public float MaxFireRadius => m_MaxFireRadius;

        [SerializeField][Min(0.0f)] private float m_AgressionRadius;
        public float AgressionRadius => m_AgressionRadius;

        [SerializeField][Min(0.0f)] private float m_StopMoveToEnemyRadius;
        public float StopMoveToEnemyRadius => m_StopMoveToEnemyRadius;
    }
}
