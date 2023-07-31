using UnityEngine;

namespace SpaceShooter
{
    public class LevelConditionGetToArea : MonoBehaviour, ILevelCondition
    {
        [SerializeField] private string m_AreaName;
        [SerializeField] private CircleArea m_Area;
        [SerializeField] private ImpactEffect m_AreaReachImpactEffect;

        private bool m_Reached;
        private bool m_IsEffectSpawned;

        LevelCondition ILevelCondition.Condition => LevelCondition.GetPosition;

        string ILevelCondition.Description
        {
            get 
            {
                if (m_Reached == true)
                    return "Get to the " + m_AreaName + " (✔)";

                return "Get to the " + m_AreaName;
            }
        }

        bool ILevelCondition.IsCompleted
        {
            get
            {
                if (Player.Instance != null && Player.Instance.ActiveShip != null)
                {
                    if (Vector2.Distance(Player.Instance.ActiveShip.transform.position, m_Area.transform.position) <= m_Area.Radius)
                    {
                        m_Reached = true;

                        if (m_AreaReachImpactEffect != null)
                        {
                            if (m_IsEffectSpawned == false)
                            {
                                ImpactEffect effect = Instantiate(m_AreaReachImpactEffect);
                                effect.transform.position = m_Area.transform.position;

                                m_IsEffectSpawned = true;
                            }
                        }
                    }
                }

                return m_Reached;
            }
        }

        private void Start()
        {
            if (m_AreaName == "")
                m_AreaName = "target area";
        }
    }
}
