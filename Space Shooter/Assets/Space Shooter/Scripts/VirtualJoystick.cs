using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpaceShooter
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image m_JoyBack;
        [SerializeField] private Image m_Joystick;

        public Vector3 Value { get; private set; }
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_JoyBack.rectTransform, eventData.position, eventData.pressEventCamera, out position);

            position.x /= (m_JoyBack.rectTransform.sizeDelta.x / 2);
            position.y /= (m_JoyBack.rectTransform.sizeDelta.y / 2);

            Value = new Vector3(position.x, position.y, 0);

            if (Value.magnitude > 1)
                Value = Value.normalized;

            float offsetX = (m_JoyBack.rectTransform.sizeDelta.x / 2) - (m_Joystick.rectTransform.sizeDelta.x / 2);
            float offsetY = (m_JoyBack.rectTransform.sizeDelta.y / 2) - (m_Joystick.rectTransform.sizeDelta.y / 2);

            m_Joystick.rectTransform.anchoredPosition = new Vector2(Value.x * offsetX, Value.y * offsetY);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector3.zero;

            m_Joystick.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
