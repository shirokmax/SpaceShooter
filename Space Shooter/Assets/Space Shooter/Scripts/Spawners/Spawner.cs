using UnityEngine;

namespace SpaceShooter
{
    public abstract class Spawner : MonoBehaviour
    {
        public enum SpawnMode
        {
            Start,
            Loop
        }

        [SerializeField] protected SpawnMode m_SpawnMode;

        [SerializeField][Min(0)] protected int m_SpawnCount;
        [SerializeField][Min(0.0f)] protected float m_RespawnTime;

        /// <summary>
        /// Лимит заспавненных объектов. Если лимит равен нулю, то объекты спавнятся без лимита.
        /// </summary>
        [SerializeField][Min(0)] protected int m_SpawnCountLimit;

        protected CircleArea m_SpawnArea;

        protected int m_CurrentSpawnedCount;

        protected float m_Timer;
        protected bool m_CanSpawn => m_Timer <= 0;

        private void Awake()
        {
            m_SpawnArea = GetComponent<CircleArea>();
        }

        private void Start()
        {
            if (m_SpawnMode == SpawnMode.Start)
                Spawn();
        }

        private void Update()
        {
            if (m_SpawnMode == SpawnMode.Loop)
            {
                if (m_Timer > 0)
                    m_Timer -= Time.deltaTime;

                if (m_CanSpawn)
                    Spawn();
            }
        }

        protected abstract void Spawn();

        protected void OnDestroyEntity()
        {
            m_CurrentSpawnedCount--;
        }
    }
}
