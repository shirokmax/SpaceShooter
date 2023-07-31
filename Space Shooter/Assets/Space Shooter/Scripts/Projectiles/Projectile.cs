using UnityEngine;

namespace SpaceShooter
{
    public class Projectile : Entity
    {
        [SerializeField] protected float m_Speed;
        public float Speed => m_Speed;

        [SerializeField] protected float m_Lifetime;
        [SerializeField] protected int m_Damage;
        [SerializeField] protected ImpactEffect m_LaunchSFXPrefab;
        [SerializeField] protected ImpactEffect m_HitEffectPrefab;

        protected Destructible m_ParentDest;
        protected bool m_IsParentPlayer;

        protected float m_Timer;

        private void Start()
        {
            if (m_LaunchSFXPrefab != null)
                Instantiate(m_LaunchSFXPrefab, transform.position, Quaternion.identity);
        }

        private void FixedUpdate()
        {
            ProjectileMovement();
        }

        protected virtual void ProjectileMovement()
        {
            float stepLength = m_Speed * Time.fixedDeltaTime;
            Vector2 step = transform.up * stepLength;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLength);

            if (hit)
            {
                if (hit.collider.transform.root.TryGetComponent(out Destructible dest) && dest != m_ParentDest)
                {
                    bool isDamaged = dest.ApplyDamage(m_ParentDest, m_Damage);

                    if (isDamaged == true && m_IsParentPlayer == true)
                    {
                        if (m_ParentDest.TeamId != dest.TeamId)
                            Player.Instance.AddScore(m_Damage * dest.ScorePerDamage);
                        else
                            Player.Instance.AddScore((int)(-m_Damage * m_ParentDest.FriendlyFirePercentage) * dest.ScorePerDamage);
                    }
                }

                if (dest != m_ParentDest) OnProjectileHit(hit.collider, hit.point);
            }

            m_Timer += Time.fixedDeltaTime;

            if (m_Timer >= m_Lifetime)
                Destroy(gameObject);

            transform.position += new Vector3(step.x, step.y, 0);
        }

        protected void OnProjectileHit(Collider2D collider, Vector2 pos)
        {
            Destroy(gameObject);

            if (collider.transform.root.TryGetComponent(out Entity entity) &&
                entity.Type == EntityType.BlackHole)
                return;

            Instantiate(m_HitEffectPrefab, pos, Quaternion.identity);
        }

        public void SetParentShooter(Destructible dest)
        {
            m_ParentDest = dest;

            if (m_ParentDest == Player.Instance.ActiveShip)
                m_IsParentPlayer = true;
        }
    }
}