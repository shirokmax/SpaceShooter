using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class GravityWell : Entity
    {
        [SerializeField] private float m_Force;
        [SerializeField] private float m_Radius;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.attachedRigidbody == null) return;

            Vector2 dir = transform.position - collision.transform.position;

            float dist = dir.magnitude;

            if (dist <= m_Radius)
                collision.attachedRigidbody.AddForce(dir.normalized * m_Force * (dist / m_Radius), ForceMode2D.Force);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.activeInHierarchy == false) return;

            GetComponent<CircleCollider2D>().radius = m_Radius;
        }
#endif

    }
}
