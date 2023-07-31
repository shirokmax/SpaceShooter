using UnityEngine;

namespace SpaceShooter
{
    public class LevelConditionSurviveTime : MonoBehaviour, ILevelCondition
    {
        [SerializeField] private int m_TimeInSeconds;

        private bool m_Reached = true;

        LevelCondition ILevelCondition.Condition => LevelCondition.SurviveTime;

        string ILevelCondition.Description
        {
            get
            {
                if (m_Reached == true)
                    return "Survive " + TimeFormat.Format(m_TimeInSeconds) + " time\n (" + TimeFormat.Format((int)LevelController.Instance.LevelTime) + ") (✔)";

                return "Survive " + TimeFormat.Format(m_TimeInSeconds) + " time\n (" + TimeFormat.Format((int)LevelController.Instance.LevelTime) + ")";
            }
        }

        bool ILevelCondition.IsCompleted
        {
            get
            {
                if (LevelController.Instance.LevelTime >= m_TimeInSeconds)
                {
                    m_Reached = true;
                }
                else
                    m_Reached = false;

                return m_Reached;
            }
        }
    }
}
