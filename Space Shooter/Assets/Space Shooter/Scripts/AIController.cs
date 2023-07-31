using UnityEngine;

namespace SpaceShooter
{
    public enum AIBehaviour
    {
        None,
        Stay,
        AreaPatrol,
        RoutePatrol
    }

    [RequireComponent(typeof(SpaceShip))]
    public class AIController : MonoBehaviour
    {
        [SerializeField] private AIControllerProperties m_AIProperties;

        [SerializeField] private AIBehaviour m_AIBehaviour;

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

        [Space]
        [SerializeField] private bool m_PreemptiveShootingMove;
        [SerializeField] private Turret PreemptiveShootingTurret;

        [Space]
        [SerializeField] private Vector2 m_EvageBoxSize;
        [SerializeField] private float m_EvadeRayLength;

        private const float ALIGN_TORQUE_MAX_ANGLE = 45.0f;
        private const float MOVE_POSITION_THRESHOLD = 1.5f;

        private SpaceShip m_Spaceship;

        private Vector3 m_MovePosition;
        private Destructible m_SelectedTarget;

        private Timer m_RandomizeDirectionTimer;
        private Timer m_FindNewTargetTimer;

        private int m_RandomRotationDir;

        private int m_CurrentRoutePointIndex;

        private void Awake()
        {
            m_Spaceship = GetComponent<SpaceShip>();

            if (m_AIProperties != null)
                AssignAIProperties(m_AIProperties);
            else
                SetNoneBehaviour();

            if (PreemptiveShootingTurret == null && m_Spaceship.Turrets.Length > 0)
                PreemptiveShootingTurret = m_Spaceship.Turrets[0];
        }

        private void Update()
        {
            if (m_AIBehaviour == AIBehaviour.None) return;

            if (m_AIBehaviour == AIBehaviour.AreaPatrol && m_PatrolArea == null)
            {
                Debug.LogError("AIController: PatrolArea = null! AIBehaviour has been set to None." + "(" + transform.root.name + ")");
                SetNoneBehaviour();
                return;
            }
            if (m_AIBehaviour == AIBehaviour.RoutePatrol)
            {
                if (m_PatrolRoute == null || m_PatrolRoute.IsComplete == false)
                {
                    Debug.LogError("Route: Route = Null or Points count < 2! AIBehaviour has been set to None." + "(" + transform.root.name + ")");
                    SetNoneBehaviour();
                    return;
                }
            }

            UpdateTimers();

            UpdateAI();
        }

        private void UpdateAI()
        {
            ActionFindNewMovePosition();
            ActionControlShip();
            ActionFindNewAttackTarget();
            ActionFire();
            ActionEvadeCollision();
        }

        public void AssignAIProperties(AIControllerProperties props)
        {
            if (props == null)
            {
                Debug.LogError("AIController: AIProperties = null! AIBehaviour has been set to None." + "(" + transform.root.name + ")");
                SetNoneBehaviour();
                return;
            }

            m_AIProperties = props;

            InitTimers();

            if (m_RandomFirstRoutePoint)
            {
                if (m_AIBehaviour == AIBehaviour.RoutePatrol && m_PatrolRoute != null)
                    m_CurrentRoutePointIndex = Random.Range(0, m_PatrolRoute.GetPointsCount);
            }
        }

        #region Actions
        private void ActionFindNewMovePosition()
        {
            if (m_AIBehaviour == AIBehaviour.Stay)
            {
                if (m_SelectedTarget != null)
                    SelectTarget();
            }

            if (m_AIBehaviour == AIBehaviour.AreaPatrol)
            {
                if (m_SelectedTarget != null)
                {
                    SelectTarget();
                }
                else
                {
                    if (IsInsidePatrolArea() == true)
                    {
                        if (m_ResetAgro == true)
                            m_Spaceship.InvincibleOff();

                        if (m_RandomizeDirectionTimer.IsFinished == true ||
                            (m_MovePosition - transform.position).sqrMagnitude <= MOVE_POSITION_THRESHOLD * MOVE_POSITION_THRESHOLD)
                        {
                            m_MovePosition = m_PatrolArea.GetRandomInsideZone();

                            m_RandomRotationDir = Random.Range(0, 2);

                            m_RandomizeDirectionTimer.Restart();
                        }
                    }
                    else
                    {
                        m_MovePosition = m_PatrolArea.transform.position;

                        if (m_ResetAgro == true)
                        {
                            if (m_Spaceship.IsIndestructible == false)
                            {
                                m_Spaceship.InvincibleOn();
                                m_Spaceship.Heal(m_Spaceship.MaxHitPoints);
                            }
                        }
                    }
                }
            }

            if (m_AIBehaviour == AIBehaviour.RoutePatrol)
            {
                if (m_SelectedTarget != null)
                {
                    SelectTarget();
                }
                else
                {
                    m_MovePosition = m_PatrolRoute.GetPointPosition(m_CurrentRoutePointIndex);

                    if ((m_MovePosition - transform.position).sqrMagnitude <= MOVE_POSITION_THRESHOLD * MOVE_POSITION_THRESHOLD)
                    {
                        m_PatrolRoute.GetNextPointIndex(ref m_CurrentRoutePointIndex);

                        m_RandomRotationDir = Random.Range(0, 2);
                    }

                    if (m_ResetAgro == true)
                    {
                        if (IsInsidePatrolRouteZone() == true)
                        {
                            m_Spaceship.InvincibleOff();
                        }
                        else
                        {
                            if (m_Spaceship.IsIndestructible == false)
                            {
                                m_Spaceship.InvincibleOn();
                                m_Spaceship.Heal(m_Spaceship.MaxHitPoints);
                            }
                        }
                    }
                }
            }
        }

        private void ActionControlShip()
        {
            if (m_AIBehaviour == AIBehaviour.Stay && m_SelectedTarget == null)
            {
                m_Spaceship.TorqueControl = 0;
                m_Spaceship.ThrustControl = 0;

                return;
            }

            m_Spaceship.TorqueControl = ComputeAlignTorqueNormalized(m_MovePosition, m_Spaceship.transform) * m_AIProperties.NavigationAngular;
            m_Spaceship.ThrustControl = m_AIProperties.NavigationLinear;

            if (m_SelectedTarget != null && IsInStopMoveToEnemyRadius() == true)
                m_Spaceship.ThrustControl = 0;
        }

        private void ActionFindNewAttackTarget()
        {
            if (m_SelectedTarget == null)
            {
                if (m_ResetAgro == true)
                {
                    if (m_AIBehaviour == AIBehaviour.AreaPatrol && IsInsidePatrolArea() == false)
                        return;

                    if (m_AIBehaviour == AIBehaviour.RoutePatrol && IsInsidePatrolRouteZone() == false)
                        return;
                }
            }

            if (m_FindNewTargetTimer.IsFinished == true)
            {
                m_SelectedTarget = FindNearestDestructibleTarget();

                m_FindNewTargetTimer.Restart();
            }

            if (m_ResetAgro == true)
            {
                if (m_AIBehaviour == AIBehaviour.AreaPatrol)
                {
                    if (m_AgroResetRadius > m_PatrolArea.Radius)
                    {
                        if (IsInsideAgroResetRadius(AIBehaviour.AreaPatrol) == false)
                            m_SelectedTarget = null;
                    }
                    else
                    {
                        if (IsInsidePatrolArea() == false)
                            m_SelectedTarget = null;
                    }
                }
                if (m_AIBehaviour == AIBehaviour.RoutePatrol)
                {
                    if (m_AgroResetRadius > m_PatrolRouteZoneRadius)
                    {
                        if (IsInsideAgroResetRadius(AIBehaviour.RoutePatrol) == false)
                            m_SelectedTarget = null;
                    }
                    else
                    {
                        if (IsInsidePatrolRouteZone() == false)
                            m_SelectedTarget = null;
                    }
                }
            }
        }

        private void ActionFire()
        {
            if (m_SelectedTarget != null)
                if (IsInFireRadius() == true)
                    m_Spaceship.Fire(TurretMode.Primary);
        }

        private void ActionEvadeCollision()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, m_EvageBoxSize, transform.eulerAngles.z, transform.up, m_EvadeRayLength);

            if (hit)
            {
                if (m_RandomRotationDir == 0) m_MovePosition = transform.position + transform.right * 100.0f;
                if (m_RandomRotationDir == 1) m_MovePosition = transform.position - transform.right * 100.0f;
            }
        }

        #endregion

        #region Helper Methods

        private void SelectTarget()
        {
            if (m_PreemptiveShootingMove == true)
            {
                if (PreemptiveShootingTurret != null)
                {
                    m_MovePosition = MakeLeadTurret(m_SelectedTarget.transform);
                }
                else
                {
                    Debug.LogError("AIController: PreemptiveShootingTurret = null! PreemptiveShootingMove bool has been set to false." + "(" + transform.root.name + ")");
                    m_PreemptiveShootingMove = false;
                }
            }
            else
            {
                m_MovePosition = m_SelectedTarget.transform.position;
            }
        }

        private Vector2 MakeLeadTurret(Transform target)
        {
            if (target.TryGetComponent(out Rigidbody2D targetRb) == false)
                return target.transform.position;

            float projectileSpeed = 1 / (PreemptiveShootingTurret.TurretProperties.ProjectilePrefab.Speed * Time.fixedDeltaTime);
            float dist = Vector2.Distance(target.transform.position, transform.position);

            float time = dist / projectileSpeed;

            Vector2 targetProbablyPos = (Vector2)target.transform.position + targetRb.velocity * time;

            dist = Vector2.Distance(targetProbablyPos, transform.position);
            time = dist / projectileSpeed;

            return (Vector2)target.transform.position + targetRb.velocity * time;
        }

        private static float ComputeAlignTorqueNormalized(Vector3 targetPosition, Transform ship)
        {
            Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);
            
            float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);

            angle = Mathf.Clamp(angle, -ALIGN_TORQUE_MAX_ANGLE, ALIGN_TORQUE_MAX_ANGLE) / ALIGN_TORQUE_MAX_ANGLE;
            
            return -angle;
        }

        private Destructible FindNearestDestructibleTarget()
        {
            Destructible potentialTarget = null;

            float minDist = float.MaxValue;

            foreach (Destructible dest in Destructible.AllDestructibles)
            {
                if (dest.GetComponent<SpaceShip>() == m_Spaceship) continue;

                if (dest.TeamId == Destructible.TeamIdNeutral) continue;
                if (dest.TeamId == m_Spaceship.TeamId) continue;

                float dist = Vector2.Distance(m_Spaceship.transform.position, dest.transform.position);

                if (dist < minDist && dist <= m_AIProperties.AgressionRadius)
                {
                    potentialTarget = dest;
                    minDist = dist;
                }
            }

            return potentialTarget;
        }

        #endregion

        #region Set Behaviours And Params
        public bool SetAreaPatrolBehaviour(CircleArea area)
        {
            if (area == null)
            {
                Debug.LogError("AIController: PatrolArea = null! AIBehaviour has been set to None." + "(" + transform.root.name + ")");
                SetNoneBehaviour();
                return false;
            }

            m_AIBehaviour = AIBehaviour.AreaPatrol;
            m_PatrolArea = area;

            return true;
        }

        public bool SetRoutePatrolBehaviour(Route route)
        {
            if (route == null || route.IsComplete == false)
            {
                Debug.LogError("Route: Route = Null or Points count < 2! AIBehaviour has been set to None." + "(" + transform.root.name + ")");
                SetNoneBehaviour();
                return false;
            }

            m_AIBehaviour = AIBehaviour.RoutePatrol;
            m_PatrolRoute = route;

            return true;
        }

        public void SetNoneBehaviour()
        {
            m_AIBehaviour = AIBehaviour.None;
        }

        public void SetAgroParams(bool resetAgro, float radius)
        {
            m_ResetAgro = resetAgro;
            m_AgroResetRadius = radius;
        }

        public void SetPatrolRouteZoneRadius(float radius)
        {
            m_PatrolRouteZoneRadius = radius;
        }

        public void SetBoolRandomFirstRoutePoint(bool isRandomPoint)
        {
            m_RandomFirstRoutePoint = isRandomPoint;
        }

        #endregion

        #region Zone Checks
        private bool IsInsidePatrolArea()
        {
            float dist = (m_PatrolArea.transform.position - transform.position).sqrMagnitude;

            return dist <= m_PatrolArea.Radius * m_PatrolArea.Radius;
        }

        private bool IsInsidePatrolRouteZone()
        {
            float dist = (m_PatrolRoute.GetNearestPointPosition(transform.position) - transform.position).sqrMagnitude;

            return dist <= m_PatrolRouteZoneRadius * m_PatrolRouteZoneRadius;
        }

        private bool IsInsideAgroResetRadius(AIBehaviour behaviour)
        {
            float dist = float.MaxValue;

            if (behaviour == AIBehaviour.AreaPatrol)
                dist = (m_PatrolArea.transform.position - transform.position).sqrMagnitude;

            if (behaviour == AIBehaviour.RoutePatrol)
                dist = (m_PatrolRoute.GetNearestPointPosition(transform.position) - transform.position).sqrMagnitude;

            return dist <= m_AgroResetRadius * m_AgroResetRadius;
        }

        private bool IsInFireRadius()
        {
            float dist = (m_SelectedTarget.transform.position - transform.position).sqrMagnitude;

            return dist <= m_AIProperties.MaxFireRadius * m_AIProperties.MaxFireRadius;
        }

        private bool IsInStopMoveToEnemyRadius()
        {
            float dist = (m_SelectedTarget.transform.position - transform.position).sqrMagnitude;

            return dist <= m_AIProperties.StopMoveToEnemyRadius * m_AIProperties.StopMoveToEnemyRadius;
        }

        #endregion

        #region Timers
        private void InitTimers()
        {
            m_RandomizeDirectionTimer = new Timer(m_AIProperties.RandomSelectMovePointTime);
            m_RandomizeDirectionTimer.Reset();

            m_FindNewTargetTimer = new Timer(m_AIProperties.FindNewTargetTime);
            m_FindNewTargetTimer.Reset();
        }

        private void UpdateTimers()
        {
            m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);
            m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
        }

        #endregion

        #region Unity Editor

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_SelectedTarget != null)
                UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
            else
                UnityEditor.Handles.color = new Color(1, 1, 0, 0.1f);

            // Точка движения
            UnityEditor.Handles.DrawSolidDisc(m_MovePosition, Vector3.forward, MOVE_POSITION_THRESHOLD);

            if (m_AIProperties != null)
            {
                // Радиус агра
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, m_AIProperties.AgressionRadius);

                if (m_AIBehaviour == AIBehaviour.RoutePatrol && m_PatrolRoute != null)
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
                if (m_AIBehaviour == AIBehaviour.AreaPatrol && m_PatrolArea != null)
                {
                    // Радиус зоны патруля
                    UnityEditor.Handles.color = new Color(1, 1, 0, 0.02f);
                    UnityEditor.Handles.DrawSolidDisc(m_PatrolArea.transform.position, Vector3.forward, m_PatrolArea.Radius);

                    // Радиус сброса агра
                    UnityEditor.Handles.color = new Color(0, 1, 0, 0.02f);
                    UnityEditor.Handles.DrawSolidDisc(m_PatrolArea.transform.position, Vector3.forward, m_AgroResetRadius);
                }

                // Радиус стрельбы
                UnityEditor.Handles.color = Color.magenta;
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, m_AIProperties.MaxFireRadius);

                // Радиус остановки движения к врагу
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, m_AIProperties.StopMoveToEnemyRadius);
            }

            // Бокс каст уворота от препятствий
            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(new Vector3(0, m_EvadeRayLength, 0), m_EvageBoxSize);
            Gizmos.DrawLine(Vector3.zero, Vector3.up * m_EvadeRayLength);
        }
#endif

        #endregion
    }
}
