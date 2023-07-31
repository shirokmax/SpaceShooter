using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(Destructible))]
    public class CollisionDamageApplicator : MonoBehaviour
    {
        public static string IgnoreTag = "WorldBoundary";

        [SerializeField] private int m_DamageConstant;
        [SerializeField] private float m_VelocityDamageModifier;

        private Destructible destructible;

        private void Awake()
        {
            destructible = GetComponent<Destructible>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.CompareTag(IgnoreTag)) return;

            if (collision.collider.transform.root.TryGetComponent(out Entity entity) && entity.Type == EntityType.BlackHole)
            {
                destructible.ApplyDamage(destructible.CurrentHitPoints);
            }
            else
            {
                if (collision.collider.transform.root.TryGetComponent(out Destructible dest))
                    destructible.ApplyDamage(dest, m_DamageConstant + (int)(m_VelocityDamageModifier * collision.relativeVelocity.magnitude));
                else
                    destructible.ApplyDamage(m_DamageConstant + (int)(m_VelocityDamageModifier * collision.relativeVelocity.magnitude));
            }
        }
    }
}
