using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceShip : Destructible
    {
        #region Properties      
        /// <summary>
        /// Превью изображение корабля.
        /// </summary>
        [Header("Spaceship")]
        [SerializeField] private Sprite m_PreviewSprite;
        public Sprite PreviewSprite => m_PreviewSprite;

        /// <summary>
        /// Масса для автоматической установки в Rigidbody.
        /// </summary>
        [SerializeField] private float m_Mass;

        /// <summary>
        /// Толкающая вперед сила.
        /// </summary>
        [Space]
        [SerializeField] private float m_Thrust;
        public float Thrust => m_Thrust;

        /// <summary>
        /// Вращающая сила.
        /// </summary>
        [SerializeField] private float m_Mobility;
        public float Mobility => m_Mobility;

        /// <summary>
        /// Максимальная линейная скорость
        /// </summary>
        [Space]
        [SerializeField] private float m_MaxLinearVelocity;

        /// <summary>
        /// Максимальная вращательная скорость. В градусах/сек.
        /// </summary>
        [SerializeField] private float m_MaxAngularVelocity;

        /// <summary>
        /// Множитель замедления движения при нулевой тяге.
        /// </summary>
        [Space]
        [SerializeField] private float m_VelocityDecreaseMult;

        /// <summary>
        /// Множитель замедления вращения при нулевой вращательной тяге.
        /// </summary>
        [SerializeField] private float m_TorqueDecreaseMult;

        /// <summary>
        /// Сохраненная ссылка на rigidbody.
        /// </summary>
        private Rigidbody2D m_Rigidbody;
        public Rigidbody2D RigidBd => m_Rigidbody;

        /// <summary>
        /// Пушки, из которых будет стрелять корабль.
        /// </summary>
        [Space]
        [SerializeField] private Turret[] m_Turrets;
        public Turret[] Turrets => m_Turrets;

        [Space]
        [SerializeField] private int m_MaxEnergy;
        public int MaxEnergy => m_MaxEnergy;

        [SerializeField] private int m_MaxAmmo;
        public int MaxAmmo => m_MaxAmmo;

        [SerializeField] private int m_EnergyRegenPerSecond;

        [Space]
        [SerializeField] private bool m_CanUsePowerups;
        public bool CanUsePowerups => m_CanUsePowerups;

        private float m_Energy;
        public float Energy => m_Energy;

        private int m_Ammo;
        public int Ammo => m_Ammo;

        private UnityEvent m_EventChangeEnergy = new UnityEvent();
        public UnityEvent EventChangeEnergy => m_EventChangeEnergy;

        private UnityEvent m_EventChangeAmmo = new UnityEvent();
        public UnityEvent EventChangeAmmo => m_EventChangeAmmo;

        private float m_SpeedBoostMult;
        private float m_SpeedBoostTimer;
        public float SpeedBoostTimer => m_SpeedBoostTimer;

        private float m_InvincibleTimer;
        public float InvincibleTimer => m_InvincibleTimer;

        public float SpeedBoostLastDurationTime { get; private set; }
        public float InvincibleLastDurationTime { get; private set; }

        [HideInInspector] public bool isInvincibleWasOn;
        [HideInInspector] public bool isSpeedBoostWasOn;

        public bool IsSpeedBoostActive => m_SpeedBoostTimer > 0;
        public bool IsInvincibleActive => m_InvincibleTimer > 0;

        /// <summary>
        /// Управление линейной тягой. (от -1.0 до 1.0)
        /// </summary>
        public float ThrustControl { get; set; }
        /// <summary>
        /// Управление вращательной тягой. (от -1.0 до 1.0)
        /// </summary>
        public float TorqueControl { get; set; }

        #endregion

        #region Unity Events
        protected override void Awake()
        {
            base.Awake();

            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Rigidbody.mass = m_Mass;           
            m_Rigidbody.inertia = 1;

            m_SpeedBoostMult = 1;

            InitResources();
        }

        private void FixedUpdate()
        {
            UpdateRigidbody();
            EnergyRegeneration();

            if (m_CanUsePowerups == true)
            {
                CheckInvincible();
                CheckSpeedBoost();
            }    
        }

        #endregion

        #region Private/Protected API
        /// <summary>
        /// Метод добавления сил кораблю для движения.
        /// </summary>
        private void UpdateRigidbody()
        {
            m_Rigidbody.AddForce(ThrustControl * m_SpeedBoostMult * m_Thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);

            if (ThrustControl == 0)
                m_Rigidbody.AddForce(-m_Rigidbody.velocity * (m_Thrust * m_VelocityDecreaseMult / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
            else
                m_Rigidbody.AddForce(-m_Rigidbody.velocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            m_Rigidbody.AddTorque(TorqueControl * m_Mobility * Time.fixedDeltaTime, ForceMode2D.Force);

            if (TorqueControl == 0)
                m_Rigidbody.AddTorque(-m_Rigidbody.angularVelocity * (m_Mobility * m_TorqueDecreaseMult / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
            else
                m_Rigidbody.AddTorque(-m_Rigidbody.angularVelocity * (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        /// <summary>
        /// Задание стартовых значений статов.
        /// </summary>
        private void InitResources()
        {
            m_Energy = m_MaxEnergy;
            m_Ammo = m_MaxAmmo;

            m_EventChangeEnergy.Invoke();
            m_EventChangeAmmo.Invoke();
        }

        /// <summary>
        /// Пассивный реген энергии.
        /// </summary>
        private void EnergyRegeneration()
        {
            AddEnergy(m_EnergyRegenPerSecond * Time.fixedDeltaTime);
        }

        private void CheckInvincible()
        {
            if (IsInvincibleActive == true)
                m_InvincibleTimer -= Time.fixedDeltaTime;

            if (IsInvincibleActive == false)
                InvincibleOff();
        }

        private void CheckSpeedBoost()
        {
            if (IsSpeedBoostActive == true)
                m_SpeedBoostTimer -= Time.fixedDeltaTime;

            if (IsSpeedBoostActive == false)
                m_SpeedBoostMult = 1;
        }

        #endregion

        #region Public API
        public void Fire(TurretMode mode)
        {
            foreach (Turret turret in m_Turrets)
            {
                if (turret.Mode == mode)
                    turret.Fire();
            }
        }

        public void SpeedBoostOn(float boostMult, float boostTime)
        {
            isSpeedBoostWasOn = true;

            m_SpeedBoostMult = boostMult;
            m_SpeedBoostTimer = boostTime;

            SpeedBoostLastDurationTime = boostTime;
        }

        /// <summary>
        /// Включает неуязвимость.
        /// </summary>
        public void InvincibleOn()
        {
            isInvincibleWasOn = true;

            if (m_Indestructible == false)
                m_Indestructible = true;
        }

        /// <summary>
        /// Включает неуязвимость по таймеру.
        /// </summary>
        /// <param name="time">Время неуязвимости.</param>
        public void InvincibleOn(float time)
        {
            InvincibleOn();
            m_InvincibleTimer = time;

            InvincibleLastDurationTime = time;
        }

        /// <summary>
        /// Выключает неуязвимость.
        /// </summary>
        public void InvincibleOff()
        {
            if (m_Indestructible == true)
                m_Indestructible = false;
        }

        public void AddEnergy(float value)
        {
            if (m_Energy == m_MaxEnergy) return;

            m_Energy = Mathf.Clamp(m_Energy + value, 0, m_MaxEnergy);

            m_EventChangeEnergy.Invoke();
        }

        public void AddAmmo(int value)
        {
            if (m_Ammo == m_MaxAmmo) return;

            m_Ammo = Mathf.Clamp(m_Ammo + value, 0, m_MaxAmmo);

            m_EventChangeAmmo.Invoke();
        }

        public bool DrawEnergy(float value)
        {
            if (value == 0)
                return true;

            if (m_Energy >= value)
            {
                m_Energy -= value;
                m_EventChangeEnergy.Invoke();

                return true;
            }

            return false;
        }

        public bool DrawAmmo(int value)
        {
            if (value == 0)
                return true;

            if (m_Ammo >= value)
            {
                m_Ammo -= value;
                m_EventChangeAmmo.Invoke();

                return true;
            }

            return false;
        }

        public void AssignWeapon(TurretProperties props, float assignTime)
        {
            foreach(Turret turret in m_Turrets)
            {
                turret.AssignLoadout(props, assignTime);
            }
        }

        #endregion
    }
}
