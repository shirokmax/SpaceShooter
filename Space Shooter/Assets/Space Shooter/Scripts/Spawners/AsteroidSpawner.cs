using UnityEngine;

namespace SpaceShooter
{
    public class AsteroidSpawner : Spawner
    {
        [SerializeField] private Asteroid[] m_AsteroidPrefabs;

        protected override void Spawn()
        {
            if (m_AsteroidPrefabs.Length == 0) return;

            for (int i = 0; i < m_SpawnCount; i++)
            {
                if (m_SpawnCountLimit == 0 || Asteroid.Count < m_SpawnCountLimit)
                {
                    int index = Random.Range(0, m_AsteroidPrefabs.Length);

                    Asteroid asteroid = Instantiate(m_AsteroidPrefabs[index]);
                    asteroid.transform.position = m_SpawnArea.GetRandomInsideZone();

                    AsteroidSize randomSize = (AsteroidSize)Random.Range(0, System.Enum.GetNames(typeof(AsteroidSize)).Length);

                    asteroid.SetSizeAndStats(randomSize);

                    float speed = Random.Range(asteroid.MinRandomSpeed, asteroid.MaxRandomSpeed);

                    Vector2 m_MovementDir = Random.insideUnitCircle.normalized;
                    asteroid.gameObject.GetComponent<Rigidbody2D>().velocity = m_MovementDir * speed;

                    asteroid.transform.rotation = Quaternion.Euler(0, 0, Random.rotation.eulerAngles.z);

                    m_Timer = m_RespawnTime;
                }
            }
        }
    }
}
