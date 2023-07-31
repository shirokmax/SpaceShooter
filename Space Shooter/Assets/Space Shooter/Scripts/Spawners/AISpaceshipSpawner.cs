using UnityEngine;

namespace SpaceShooter
{
    public class AISpaceshipSpawner : Spawner
    {
        [SerializeField] private AIController[] AISpaceshipPrefabs;

        /// <summary>
        /// Свойства для каждого префаба. Элементы массива префабов должны соответствовать элементам свойств.
        /// </summary>
        [SerializeField] private AIControllerProperties[] m_AIPrefabsProperties;

        [Space]
        [SerializeField] private int m_TeamId;

        [Space]
        [SerializeField] AIBehaviour m_AIBehaviour;

        [Space]
        [SerializeField] private CircleArea m_PatrolArea;
        [SerializeField] private Route m_PatrolRoute;

        [Space]
        [SerializeField][Min(0.0f)] private float m_PatrolRouteZoneRadius;
        [SerializeField] private bool m_RandomFirstRoutePoint;

        [Space]
        [SerializeField] private bool m_ResetAgro;

        /// <summary>
        /// Радиус зоны сброса агра. Если корабль вылетает за эту зону, он включает неуязвимость и хилится на фул хп, и возвращается в свою зону патруля.
        /// После возвращения в границы своей зоны патруля, корабль выключает неуязвимость.
        /// В случае с поведением "RoutePatrol", эти зоны должны соприкасаться друг с другом, чтобы небыло пустых окон между ними, 
        /// иначе корабль будет включать неуязвимость во время перелета между зонами.
        /// </summary>
        [SerializeField][Min(0.0f)] private float m_AgroResetRadius;

        protected override void Spawn()
        {
            if (AISpaceshipPrefabs.Length == 0) return;

            if (m_AIPrefabsProperties.Length < AISpaceshipPrefabs.Length)
            {
                Debug.LogError("AISpaceshipSpawner: Properties count is less than Prefabs count " + "(" + transform.root.name + ")");
                return;
            }

            for (int i = 0; i < m_SpawnCount; i++)
            {
                if (m_SpawnCountLimit == 0 || m_CurrentSpawnedCount < m_SpawnCountLimit)
                {
                    int index = Random.Range(0, AISpaceshipPrefabs.Length);

                    AIController AI = Instantiate(AISpaceshipPrefabs[index]);
                    AI.transform.position = m_SpawnArea.GetRandomInsideZone();

                    if (AI.TryGetComponent(out Destructible dest))
                    {
                        dest.SetTeamId(m_TeamId);
                        dest.EventOnDeath.AddListener(OnDestroyEntity);
                    }

                    switch (m_AIBehaviour)
                    {
                        case AIBehaviour.None:
                            {
                                AI.SetNoneBehaviour();
                            }
                            break;
                        case AIBehaviour.AreaPatrol:
                            {
                                AI.SetAreaPatrolBehaviour(m_PatrolArea);
                            }
                            break;
                        case AIBehaviour.RoutePatrol:
                            {
                                AI.SetRoutePatrolBehaviour(m_PatrolRoute);

                                AI.SetPatrolRouteZoneRadius(m_PatrolRouteZoneRadius);
                                AI.SetBoolRandomFirstRoutePoint(m_RandomFirstRoutePoint);
                            }
                            break;
                        default:
                            {
                                AI.SetNoneBehaviour();
                            }
                            break;
                    }

                    AI.SetAgroParams(m_ResetAgro, m_AgroResetRadius);

                    AI.AssignAIProperties(m_AIPrefabsProperties[index]);

                    m_CurrentSpawnedCount++;

                    m_Timer = m_RespawnTime;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_AIBehaviour == AIBehaviour.AreaPatrol && m_PatrolArea != null)
            {
                // Радиус зоны патруля
                UnityEditor.Handles.color = new Color(1, 1, 0, 0.02f);
                UnityEditor.Handles.DrawSolidDisc(m_PatrolArea.transform.position, Vector3.forward, m_PatrolArea.Radius);

                // Радиус сброса агра
                if (m_ResetAgro == true)
                {
                    UnityEditor.Handles.color = new Color(0, 1, 0, 0.02f);
                    UnityEditor.Handles.DrawSolidDisc(m_PatrolArea.transform.position, Vector3.forward, m_AgroResetRadius);
                }
            }
            if (m_AIBehaviour == AIBehaviour.RoutePatrol && m_PatrolRoute != null)
            {
                if (m_ResetAgro == true)
                {
                    // Радиус зоны route
                    UnityEditor.Handles.color = new Color(1, 1, 0, 0.02f);
                    for (int i = 0; i < m_PatrolRoute.transform.childCount; i++)
                        UnityEditor.Handles.DrawSolidDisc(m_PatrolRoute.transform.GetChild(i).position, Vector3.forward, m_PatrolRouteZoneRadius);

                    // Радиус сброса агра
                    UnityEditor.Handles.color = new Color(0, 1, 0, 0.02f);
                    for (int i = 0; i < m_PatrolRoute.transform.childCount; i++)
                        UnityEditor.Handles.DrawSolidDisc(m_PatrolRoute.transform.GetChild(i).position, Vector3.forward, m_AgroResetRadius);
                }
            }
        }
#endif
    }
}
