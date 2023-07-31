using UnityEngine;

namespace SpaceShooter
{
    public class CircleArea : MonoBehaviour
    {
        public enum ColorStyle
        {
            Neutral,
            Danger,
            Friendly
        }

        [SerializeField] private float m_Radius;
        public float Radius
        {
            set 
            { 
                if (value >= 0) m_Radius = value;
                else m_Radius = 0;
            }
            get { return m_Radius; } 
        }

        [SerializeField] private ColorStyle m_ColorStyle;

        public Vector2 GetRandomInsideZone()
        {
            return (Vector2)transform.position + Random.insideUnitCircle * m_Radius;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            switch (m_ColorStyle)
            {
                case ColorStyle.Danger:
                    UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
                    break;
                case ColorStyle.Neutral:
                    UnityEditor.Handles.color = new Color(1, 1, 0, 0.1f);
                    break;
                case ColorStyle.Friendly:
                    UnityEditor.Handles.color = new Color(0, 1, 0, 0.1f);
                    break;
                default:
                    UnityEditor.Handles.color = new Color(1, 1, 0, 0.1f);
                    break;
            }

            UnityEditor.Handles.DrawSolidDisc(transform.position, transform.forward, m_Radius);
        }

#endif

    }
}
