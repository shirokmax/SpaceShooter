using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// Уничтожаемый объект на сцене. То, что может иметь хитпоинты.
    /// </summary>
    public class Destructible : Entity
    {
        #region Properties
        /// <summary>
        /// Объект игнорирует повреждения.
        /// </summary>
        [SerializeField] protected bool m_Indestructible;
        public bool IsIndestructible => m_Indestructible;

        /// <summary>
        /// Стартовое кол-во хитпоинтов.
        /// </summary>
        [SerializeField] private int m_MaxHitPoints;
        public int MaxHitPoints => m_MaxHitPoints;

        /// <summary>
        /// Текущие хитпоинты.
        /// </summary>
        protected int m_CurrentHitPoints;
        public int CurrentHitPoints => m_CurrentHitPoints;

        /// <summary>
        /// Событие, вызываемое при "смерти" destructible.
        /// </summary>
        [SerializeField] private UnityEvent m_EventOnDeath;
        public UnityEvent EventOnDeath => m_EventOnDeath;

        private UnityEvent m_EventChangeHitPoints = new UnityEvent();
        public UnityEvent EventChangeHitPoints => m_EventChangeHitPoints;

        private UnityEvent<Destructible, int> m_EventDamageTaken = new UnityEvent<Destructible, int>();
        public UnityEvent<Destructible, int> EventDamageTaken => m_EventDamageTaken;

        private static HashSet<Destructible> m_AllDestructibles;
        public static IReadOnlyCollection<Destructible> AllDestructibles => m_AllDestructibles;

        public const int TeamIdNeutral = 0;

        [SerializeField] private int m_TeamId;
        public int TeamId => m_TeamId;

        [SerializeField][Range(0f, 1f)] private float m_FriendlyFirePercentage;
        public float FriendlyFirePercentage => m_FriendlyFirePercentage;

        /// <summary>
        /// Очки за единицу урона.
        /// </summary>
        [SerializeField] protected int m_ScorePerDamage;
        public int ScorePerDamage => m_ScorePerDamage;

        #endregion

        #region Unity Events
        protected virtual void Awake()
        {
            m_CurrentHitPoints = m_MaxHitPoints;
        }

        #endregion

        #region Public API
        /// <summary>
        /// Применение урона к объекту.
        /// </summary>
        public virtual bool ApplyDamage(int damage)
        {
            return ApplyDamage(null, damage);
        }

        /// <summary>
        /// Применение урона к объекту с учитыванием "урона по своим".
        /// </summary>
        /// <param name="damage">Урон, наносимый объекту.</param>
        /// <param name="teamId">Id команды объекта, наносящего урон.</param>
        /// <param name="friendlyFirePercentage">Процент "урона по своим" объекта, наносящего урон.</param>
        /// <returns>Возвращает результат операции.</returns>
        public virtual bool ApplyDamage(Destructible fromDest, int damage)
        {
            if (IsIndestructible) return false;

            if (damage == 0) return false;

            if (fromDest != null)
            {
                if (fromDest.TeamId == m_TeamId && fromDest.FriendlyFirePercentage == 0) return false;

                if (fromDest.TeamId == m_TeamId && fromDest.FriendlyFirePercentage > 0.0f)
                {
                    int dmg = (int)(damage * fromDest.FriendlyFirePercentage);

                    m_CurrentHitPoints -= dmg;
                    m_EventDamageTaken.Invoke(fromDest, dmg);
                }
                else
                {
                    m_CurrentHitPoints -= damage;
                    m_EventDamageTaken.Invoke(fromDest, damage);
                }
            }
            else
            {
                m_CurrentHitPoints -= damage;
                m_EventDamageTaken.Invoke(fromDest, damage);
            }

            m_EventChangeHitPoints.Invoke();

            if (m_CurrentHitPoints <= 0)
            {
                m_CurrentHitPoints = 0;

                if (fromDest != null)
                {
                    if (fromDest == Player.Instance.ActiveShip)
                    {
                        if (gameObject.GetComponent<SpaceShip>() != null)
                        {
                            if (fromDest.TeamId != m_TeamId)
                                Player.Instance.AddKill();
                            else
                                Player.Instance.RemoveKill();
                        }
                        else if (gameObject.GetComponent<Asteroid>() != null)
                        {
                            Player.Instance.AddAsteroidKill();
                        }
                    }
                }

                OnDeath();
            }

            return true;
        }

        /// <summary>
        /// Прибавление здоровья к объекту. Здоровье объекта не может стать больше максимального.
        /// </summary>
        /// <param name="healAmount">Кол-во прибавляемого здоровья.</param>
        public bool Heal(int healAmount)
        {
            if (m_CurrentHitPoints < m_MaxHitPoints)
            {
                if (m_CurrentHitPoints + healAmount > m_MaxHitPoints)
                    m_CurrentHitPoints = m_MaxHitPoints;
                else
                    m_CurrentHitPoints += healAmount;

                m_EventChangeHitPoints.Invoke();

                return true;
            }

            return false;
        }

        public void SetTeamId(int id)
        {
            m_TeamId = id;
        }

        #endregion

        #region Protected API
        /// <summary>
        /// Переопределяемое событие уничтожения объекта, когда хитпоинты ниже нуля.
        /// </summary>
        protected virtual void OnDeath()
        {
            Destroy(gameObject);

            m_EventOnDeath?.Invoke();
        }

        protected virtual void OnEnable()
        {
            if (m_AllDestructibles == null)
                m_AllDestructibles = new HashSet<Destructible>();

            m_AllDestructibles.Add(this);
        }

        protected virtual void OnDestroy()
        {
            m_AllDestructibles.Remove(this);
        }

        #endregion
    }
}
