using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    public enum EntityType
    {
        Spaceship,
        Planet,
        BlackHole,
        Asteroid,
        Projectile,
        Powerup
    }

    /// <summary>
    /// Базовый класс для всех интерактивных объектов на сцене.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private string m_Nickname;
        public string Nickname => m_Nickname;

        [SerializeField] private EntityType m_Type;
        public EntityType Type => m_Type;

        private UnityEvent m_EventOndestroy = new UnityEvent();
        public UnityEvent EventOnDestroy => m_EventOndestroy;

        private void OnDestroy()
        {
            m_EventOndestroy.Invoke();
        }
    }
}