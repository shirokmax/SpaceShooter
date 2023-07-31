using UnityEngine;

namespace SpaceShooter
{
    public enum LevelCondition
    {
        Score,
        Kills,
        Time,
        SurviveTime,
        GetPosition
    }

    public interface ILevelCondition
    {
        LevelCondition Condition { get; }
        string Description { get; }
        bool IsCompleted { get; }
    }

    public class LevelController : MonoSingleton<LevelController>
    {
        [SerializeField] private float m_ReferenceTime;
        public float ReferenceTime => m_ReferenceTime;

        private ILevelCondition[] m_Conditions;
        public ILevelCondition[] Conditions => m_Conditions;

        private bool m_IsLevelCompleted;

        private float m_LevelTime;
        public float LevelTime => m_LevelTime;

        protected override void Awake()
        {
            base.Awake();

            m_Conditions = GetComponentsInChildren<ILevelCondition>();
        }

        private void Update()
        {
            if (m_IsLevelCompleted == false)
            {
                m_LevelTime += Time.deltaTime;

                CheckLevelConditions();
            }
        }

        private void CheckLevelConditions()
        {
            if (m_Conditions.Length == 0)
                return;

            int numCompleted = 0;

            foreach(var condition in m_Conditions)
            {
                if (condition.IsCompleted)
                    numCompleted++;

                if (condition.Condition == LevelCondition.Time && condition.IsCompleted == false)
                {
                    m_IsLevelCompleted = true;

                    LevelSequenceController.Instance?.FinishCurrentLevel(false);
                }
            }

            if (numCompleted == m_Conditions.Length)
            {
                m_IsLevelCompleted = true;

                LevelSequenceController.Instance?.FinishCurrentLevel(true);
            }
        }
    }
}
