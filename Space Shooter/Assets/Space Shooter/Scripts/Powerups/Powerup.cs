using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Powerup : Entity
    {
        [SerializeField] private ImpactEffect m_PickupImpactSFX;
        [SerializeField] private CircleCollider2D m_PowerupCollider;

        private void Start()
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, m_PowerupCollider.radius);

            if (col != null && col.transform.root.TryGetComponent(out Entity entity))
            {
                if (entity.Type == EntityType.Planet ||
                    entity.Type == EntityType.BlackHole)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.root.TryGetComponent(out SpaceShip ship) && ship.CanUsePowerups == true)
            {
                OnPickedUp(ship);

                if (m_PickupImpactSFX != null)
                    Instantiate(m_PickupImpactSFX, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }

        protected abstract void OnPickedUp(SpaceShip ship);
    }
}
