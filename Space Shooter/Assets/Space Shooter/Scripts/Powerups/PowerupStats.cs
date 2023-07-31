using UnityEngine;

namespace SpaceShooter
{
    public class PowerupStats : Powerup
    {
        public enum EffectType
        {
            AddEnergy,
            AddAmmo,
            SpeedUp,
            Invincible
        }

        [SerializeField] private EffectType m_EffectType;

        [SerializeField] private float m_Value;
        [SerializeField][Min(0)] private float m_Time;

        protected override void OnPickedUp(SpaceShip ship)
        {
            switch (m_EffectType)
            {
                case EffectType.AddEnergy:
                    ship.AddEnergy(m_Value);
                    break;

                case EffectType.AddAmmo:
                    ship.AddAmmo((int)m_Value);
                    break;

                case EffectType.SpeedUp:
                    ship.SpeedBoostOn(m_Value, m_Time);
                    break;

                case EffectType.Invincible:
                    ship.InvincibleOn(m_Time);
                    break;

                default: return;
            }
        }
    }
}
