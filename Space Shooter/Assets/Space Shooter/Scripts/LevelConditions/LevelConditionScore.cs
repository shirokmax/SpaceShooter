using UnityEngine;

namespace SpaceShooter
{
    public class LevelConditionScore : MonoBehaviour, ILevelCondition
    {
        [SerializeField] private int m_Score;

        private bool m_Reached;

        LevelCondition ILevelCondition.Condition => LevelCondition.Score;

        string ILevelCondition.Description
        {
            get
            {
                if (m_Reached == true)
                    return "Reach " + m_Score + " scores (✔)";

                return "Reach " + m_Score + " scores";
            }
        }

        bool ILevelCondition.IsCompleted
        {
            get
            {
                if (Player.Instance != null && Player.Instance.ActiveShip != null)
                {
                    if (Player.Instance.Score >= m_Score)
                        m_Reached = true;
                }

                return m_Reached;
            }
        }
    }
}
