using UnityEngine;

namespace SpaceShooter
{
    public enum AsteroidSize
    {
        Small,
        Medium,
        Big,
    }

    public class Asteroid : Destructible
    {
        [SerializeField] private int m_StartScoreValue;

        [SerializeField] private Rigidbody2D m_Rigidbody;
        [SerializeField] private CircleCollider2D m_CircleCollider;

        [SerializeField] private AsteroidSize m_Size;
        public AsteroidSize Size => m_Size;

        [SerializeField] private float m_SmallSize;
        [SerializeField] private float m_MediumSize;
        [SerializeField] private float m_BigSize;

        [SerializeField] private float m_MinRandomSpeed;
        [SerializeField] private float m_MaxRandomSpeed;
        public float MinRandomSpeed => m_MinRandomSpeed;
        public float MaxRandomSpeed => m_MaxRandomSpeed;

        [SerializeField] private ImpactEffect m_SmallExplosionVFX;
        [SerializeField] private ImpactEffect m_MediumExplosionVFX;
        [SerializeField] private ImpactEffect m_BigExplosionVFX;

        private static int m_Count;
        public static int Count => m_Count;

        protected override void Awake()
        {
            SetSizeAndStats(m_Size);

            m_Count++;
        }

        public void SetSizeAndStats(AsteroidSize size)
        {
            if (size < 0) return;

            m_Size = size;

            if (size == AsteroidSize.Small)
            {
                base.Awake();
                m_ScorePerDamage = m_StartScoreValue;

                transform.localScale = Vector3.one * m_SmallSize;
            }

            if (size == AsteroidSize.Medium)
            {
                m_CurrentHitPoints = (int)(MaxHitPoints * m_MediumSize);
                m_ScorePerDamage = (int)(m_StartScoreValue * m_MediumSize);

                transform.localScale = Vector3.one * m_MediumSize;
            }

            if (size == AsteroidSize.Big)
            {
                m_CurrentHitPoints = (int)(MaxHitPoints * m_BigSize);
                m_ScorePerDamage = (int)(m_StartScoreValue * m_BigSize);

                transform.localScale = Vector3.one * m_BigSize;
            }
        }

        protected override void OnDeath()
        {
            if (m_Size == AsteroidSize.Small)
            {
                Instantiate(m_SmallExplosionVFX, transform.position, Quaternion.identity);
            }

            if (m_Size == AsteroidSize.Medium)
            {
                Instantiate(m_MediumExplosionVFX, transform.position, Quaternion.identity);

                SpawnFragments(AsteroidSize.Small);
            }

            if (m_Size == AsteroidSize.Big)
            {
                Instantiate(m_BigExplosionVFX, transform.position, Quaternion.identity);

                SpawnFragments(AsteroidSize.Medium);
            }

            m_Count--;

            base.OnDeath();
        }

        private void SpawnFragments(AsteroidSize size)
        {
            float speed = Random.Range(m_MinRandomSpeed, m_MaxRandomSpeed);
            Vector2 m_MovementDir = Random.insideUnitCircle.normalized;

            for (int i = 0; i < 2; i++)
            {
                Asteroid asteroid = Instantiate(this, transform.position, Quaternion.identity);

                asteroid.m_Size = size;
                asteroid.SetSizeAndStats(asteroid.m_Size);

                if (i == 0)
                {
                    asteroid.transform.position = (Vector2)transform.position + m_MovementDir * m_CircleCollider.radius * asteroid.transform.localScale.x * 1.2f;
                    asteroid.m_Rigidbody.velocity = m_MovementDir * speed;
                }
                if (i == 1)
                {
                    asteroid.transform.position = (Vector2)transform.position - m_MovementDir * m_CircleCollider.radius * asteroid.transform.localScale.x * 1.2f;
                    asteroid.m_Rigidbody.velocity = -m_MovementDir * speed;
                }
            }
        }
    }
}
