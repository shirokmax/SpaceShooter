using UnityEngine;

namespace SpaceShooter
{
    public class Missile : Projectile
    {
        enum HomingMode
        {
            All,
            EnemySpaceships,
            None
        }

        /// <summary>
        /// Границы касания ракеты других коллайдеров.
        /// </summary>
        [SerializeField] private Vector2 m_HitBounds;
        /// <summary>
        /// Радиус сплеш урона.
        /// </summary>
        [SerializeField] private float m_DamageRadius;

        /// <summary>
        /// Цели для самонаведения.
        /// </summary>
        [SerializeField] private HomingMode m_HomingMode;

        /// <summary>
        /// Максимальный радиус, в котором ракета может захватить первую попавшуюся цель.
        /// </summary>
        [SerializeField] private float m_HomingRadius;

        /// <summary>
        /// Максимальное расстояние, при котором ракета будет преследовать цель.
        /// </summary>
        [SerializeField] private float m_MaxChasingDistance;

        /// <summary>
        /// Скорость плавного поворота к цели.
        /// </summary>
        [SerializeField] private float m_HomingRotateInterpolation;

        /// <summary>
        /// Звук при захвате цели ракетой.
        /// </summary>
        [SerializeField] private AudioSource m_TargetLockSound;

        /// <summary>
        /// Множитель размера эффекта взрыва ракеты в зависимости от радиуса урона взрыва.
        /// </summary>
        private float m_HitEffectScaleMult = 3.4f;

        private Transform m_HomingTarget;

        protected override void ProjectileMovement()
        {
            m_Timer += Time.fixedDeltaTime;

            if (m_Timer >= m_Lifetime)
                Destroy(gameObject);

            if (m_HomingMode != HomingMode.None && m_HomingTarget == null)
                TargetSearch();

            if (m_HomingTarget != null)
            {
                Vector2 dir = m_HomingTarget.position - transform.position;

                if (dir.magnitude > m_MaxChasingDistance)
                    m_HomingTarget = null;
                else
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, dir), m_HomingRotateInterpolation * Time.fixedDeltaTime);
            }

            transform.Translate(Vector3.up * m_Speed * Time.fixedDeltaTime);

            Collider2D col = Physics2D.OverlapBox(transform.position, m_HitBounds, transform.eulerAngles.z);

            if (col != null)
            {
                if (col.transform.root.TryGetComponent(out Entity entity) && entity.Type == EntityType.BlackHole)
                {
                    Destroy(gameObject);
                    return;
                }

                if (col.transform.root.GetComponent<Destructible>() != m_ParentDest) 
                    OnMissileHit();
            }
        }

        private void OnMissileHit()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, m_DamageRadius);

            if (hitColliders.Length != 0)
            {
                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.transform.root.TryGetComponent(out Destructible dest) && dest != m_ParentDest)
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
                }

                OnMissileLifeEnd();
            }
        }

        private void TargetSearch()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, m_HomingRadius);

            if (hitColliders.Length != 0)
            {
                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.transform.root.TryGetComponent(out Destructible dest))
                    {
                        if (dest != m_ParentDest && dest.TeamId != m_ParentDest.TeamId)
                        {
                            if (m_HomingMode == HomingMode.All)
                            {
                                TargetLock(dest);
                                return;
                            }

                            if (m_HomingMode == HomingMode.EnemySpaceships && dest.Type == EntityType.Spaceship)
                            {
                                TargetLock(dest);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void TargetLock(Destructible destructible)
        {
            m_HomingTarget = destructible.transform;
            m_TargetLockSound.Play();
        }

        private void OnMissileLifeEnd()
        {
            ImpactEffect hitEffect = Instantiate(m_HitEffectPrefab, transform.position, Quaternion.identity);

            hitEffect.transform.localScale = Vector3.one * (m_DamageRadius * m_HitEffectScaleMult);

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = new Color(1, 1, 0, 0.05f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, transform.forward, m_HomingRadius);

            UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, transform.forward, m_DamageRadius);

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, m_HitBounds);
        }
#endif
    }
}