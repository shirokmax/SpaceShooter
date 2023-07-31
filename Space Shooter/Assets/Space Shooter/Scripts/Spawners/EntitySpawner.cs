using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleArea))]
    public class EntitySpawner : Spawner
    {
        [SerializeField] private Entity[] m_EntityPrefabs;

        protected override void Spawn()
        {
            if (m_EntityPrefabs.Length == 0) return;

            for (int i = 0; i < m_SpawnCount; i++)
            {
                if (m_SpawnCountLimit == 0 || m_CurrentSpawnedCount < m_SpawnCountLimit)
                {
                    int index = Random.Range(0, m_EntityPrefabs.Length);

                    Entity entity = Instantiate(m_EntityPrefabs[index]);
                    entity.transform.position = m_SpawnArea.GetRandomInsideZone();

                    entity.EventOnDestroy.AddListener(OnDestroyEntity);

                    m_CurrentSpawnedCount++;

                    m_Timer = m_RespawnTime;
                }
            }
        }
    }
}