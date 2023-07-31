using UnityEngine;

namespace SpaceShooter
{
    public class LevelConditionTime : MonoBehaviour, ILevelCondition
    {
        [SerializeField] private int m_TimeInSeconds;

        private bool m_Reached = true;

        LevelCondition ILevelCondition.Condition => LevelCondition.Time;

        string ILevelCondition.Description
        {
            get 
            { 
                if (m_Reached == true)
                    return "Complete Level in " + TimeFormat.Format(m_TimeInSeconds) + "\n (" + TimeFormat.Format((int)LevelController.Instance.LevelTime) + ") (✔)";

                    return "Complete Level in " + TimeFormat.Format(m_TimeInSeconds) + "\n (" + TimeFormat.Format((int)LevelController.Instance.LevelTime) + ") (✖)";
            }
        }

        bool ILevelCondition.IsCompleted
        {
            get
            {
                if (LevelController.Instance.LevelTime < m_TimeInSeconds)
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
