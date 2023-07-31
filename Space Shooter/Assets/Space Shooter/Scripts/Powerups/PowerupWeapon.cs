using UnityEngine;

namespace SpaceShooter
{
    public class PowerupWeapon : Powerup
    {
        [SerializeField] private TurretProperties m_Properties;
        [SerializeField] private float m_Time;

        protected override void OnPickedUp(SpaceShip ship)
        {
            ship.AssignWeapon(m_Properties, m_Time);
        }
    }
}