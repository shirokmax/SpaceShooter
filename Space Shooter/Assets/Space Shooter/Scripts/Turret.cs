using UnityEngine;

namespace SpaceShooter
{
    public class Turret : MonoBehaviour
    {
        #region Properties
        [SerializeField] private TurretMode m_Mode;
        public TurretMode Mode => m_Mode;

        [SerializeField] private TurretProperties m_StartTurretProperties;

        private TurretProperties m_TurretProperties;
        public TurretProperties TurretProperties => m_TurretProperties;

        private float m_RefireTimer;

        private float m_AssignLoadoutTimer;
        public float AssignLoadoutTimer => m_AssignLoadoutTimer;
        public float AssignLoadoutLastDurationTime { get; private set; }
        public bool CanFire => m_RefireTimer <= 0;
        public bool AssignLoadoutEnd => m_AssignLoadoutTimer <= 0;

        private SpaceShip m_Ship;

        #endregion

        #region UnityEvents
        private void Start()
        {
            m_Ship = transform.root.GetComponent<SpaceShip>();
            m_TurretProperties = m_StartTurretProperties;
        }

        private void Update()
        {
            if (m_RefireTimer > 0)
                m_RefireTimer -= Time.deltaTime;

            if (m_AssignLoadoutTimer > 0)
                m_AssignLoadoutTimer -= Time.deltaTime;

            if (AssignLoadoutEnd == true && m_TurretProperties != m_StartTurretProperties)
            {
                AssignLoadout(m_StartTurretProperties, 0);
            }
        }

        #endregion

        #region Public API
        public void Fire()
        {
            if (m_TurretProperties == null) return;

            if (CanFire == false) return;

            if (m_Ship.DrawEnergy(m_TurretProperties.EnergyUsage) == false) 
                return;

            if (m_Ship.DrawAmmo(m_TurretProperties.AmmoUsage) == false) 
                return;

            Projectile projectile = Instantiate(m_TurretProperties.ProjectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.up = transform.up;

            projectile.SetParentShooter(m_Ship);

            m_RefireTimer = m_TurretProperties.FireRate;
        }

        public void AssignLoadout(TurretProperties props, float assignTime)
        {
            if (props == null) return;
            if (m_Mode != props.Mode) return;

            m_RefireTimer = 0;
            m_AssignLoadoutTimer = assignTime;

            AssignLoadoutLastDurationTime = assignTime;

            m_TurretProperties = props;
        }

        #endregion
    }
}
