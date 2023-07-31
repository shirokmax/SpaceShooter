using UnityEngine;

namespace SpaceShooter
{
    public class SpaceshipController : MonoBehaviour
    {
        public enum ControlMode
        {
            Keyboard,
            VirtualJoystick
        }

        [SerializeField] private SpaceShip m_TargetShip;

        [SerializeField] private VirtualJoystick m_VirtualJoystick;

        [SerializeField] private ControlMode m_ControlMode;

        [SerializeField] private PointerClickHold m_MobileFirePrimary;
        [SerializeField] private PointerClickHold m_MobileFireSecondary;

        private void Start()
        {
            if (Application.isMobilePlatform)
                m_ControlMode = ControlMode.VirtualJoystick;

            if (m_ControlMode == ControlMode.Keyboard)
            {
                m_VirtualJoystick.gameObject.SetActive(false);

                m_MobileFirePrimary.gameObject.SetActive(false);
                m_MobileFireSecondary.gameObject.SetActive(false);
            }
            else
            {
                m_VirtualJoystick.gameObject.SetActive(true);

                m_MobileFirePrimary.gameObject.SetActive(true);
                m_MobileFireSecondary.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (m_TargetShip == null) return;

            if (m_ControlMode == ControlMode.Keyboard) ControlKeyboard();
            if (m_ControlMode == ControlMode.VirtualJoystick) ControlVirtualJoystick();
        }

        public void SetTargetShip(SpaceShip ship)
        {
            m_TargetShip = ship;
        }

        private void ControlVirtualJoystick()
        {
            if (Time.timeScale == 0) return;

            Vector2 dir = m_VirtualJoystick.Value;

            m_TargetShip.ThrustControl = dir.y;
            m_TargetShip.TorqueControl = -dir.x;

            if (m_MobileFirePrimary.IsHold)
                m_TargetShip.Fire(TurretMode.Primary);

            if (m_MobileFireSecondary.IsHold)
                m_TargetShip.Fire(TurretMode.Secondary);

            //var dot = Vector2.Dot(dir, m_TargetShip.transform.up);
            //var dot2 = Vector2.Dot(dir, m_TargetShip.transform.right);

            //m_TargetShip.ThrustControl = Mathf.Max(0, dot);
            //m_TargetShip.TorqueControl = -dot2;
        }

        private void ControlKeyboard()
        {
            if (Time.timeScale == 0) return;

            float thrust = 0;
            float torque = 0;
            
            if (Input.GetKey(KeyCode.UpArrow))
                thrust = 1.0f;

            if (Input.GetKey(KeyCode.DownArrow))
                thrust = -1.0f;

            if (Input.GetKey(KeyCode.LeftArrow))
                torque = 1.0f;

            if (Input.GetKey(KeyCode.RightArrow))
                torque = -1.0f;

            if (Input.GetKey(KeyCode.Space))
                m_TargetShip.Fire(TurretMode.Primary);

            if (Input.GetKey(KeyCode.X))
                m_TargetShip.Fire(TurretMode.Secondary);

            m_TargetShip.ThrustControl = thrust;
            m_TargetShip.TorqueControl = torque;
        }
    }
}
