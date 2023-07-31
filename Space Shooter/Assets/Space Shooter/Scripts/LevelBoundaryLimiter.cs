using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Ограничитель позиции. Работает в связке со скриптом LevelBoundary, если таковой есть на сцене.
    /// Кидается на объект, который надо ограничить.
    /// </summary>
    public class LevelBoundaryLimiter : MonoBehaviour
    {
        public enum Mode
        {
            Limit,
            Teleport
        }

        [SerializeField] private Mode m_LimitMode;
        public Mode LimitMode => m_LimitMode;

        private void Update()
        {
            if (LevelBoundary.Instance == null) return;

            var radius = LevelBoundary.Instance.Radius;

            if (transform.position.magnitude > radius)
            {
                if (m_LimitMode == Mode.Limit)
                {
                    transform.position = transform.position.normalized * radius;
                }
                if (m_LimitMode == Mode.Teleport)
                {
                    transform.position = -transform.position.normalized * radius;
                }
            }
        }
    }
}
