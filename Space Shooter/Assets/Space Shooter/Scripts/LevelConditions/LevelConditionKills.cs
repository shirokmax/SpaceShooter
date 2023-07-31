using UnityEngine;

namespace SpaceShooter
{
    public class LevelConditionKills : MonoBehaviour, ILevelCondition
    {
        [SerializeField] private int m_NumKills;

        private bool m_Reached;

        LevelCondition ILevelCondition.Condition => LevelCondition.Kills;

        string ILevelCondition.Description
        {
            get 
            { 
                if (m_Reached == true)
                    return "Eliminate [" + m_NumKills + "/" + m_NumKills + "] enemy ships (✔)";

                return "Eliminate [" + Player.Instance.NumKills + "/" + m_NumKills + "] enemy ships";
            }
        }

        bool ILevelCondition.IsCompleted
        {
            get
            {
                if (Player.Instance != null && Player.Instance.ActiveShip != null)
                {
                    if (Player.Instance.NumKills >= m_NumKills)
                    {
                        m_Reached = true;
                    }
                }

                return m_Reached;
            }
        }
    }
}
