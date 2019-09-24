using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput {
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        public enum AxisOption {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public int MovementRange = 100;
        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

        Vector3 m_StartPos;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        void OnEnable () {
            CreateVirtualAxes ();
        }
        RectTransform rectTransform;
        void Start () {
            scaler = GetComponentInParent<CanvasScaler> ();
            rectTransform = GetComponent<RectTransform> ();
            m_StartPos = rectTransform.anchoredPosition; // transform.position;
        }

        void UpdateVirtualAxes (Vector3 value) {
            var delta = m_StartPos - value;
            delta.y = -delta.y;
            delta /= MovementRange;

            if (m_UseX) {
                m_HorizontalVirtualAxis.Update (-delta.x);
            }

            if (m_UseY) {
                m_VerticalVirtualAxis.Update (delta.y);
            }
        }

        void CreateVirtualAxes () {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX) {
                m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis (horizontalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis (m_HorizontalVirtualAxis);
            }
            if (m_UseY) {
                m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis (verticalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis (m_VerticalVirtualAxis);
            }
        }

        CanvasScaler scaler;
        public void OnDrag (PointerEventData data) {

            Vector3 newPos = Vector3.zero;
            Vector2 pos = new Vector2 (data.position.x * scaler.referenceResolution.x / Screen.width, data.position.y * scaler.referenceResolution.y / Screen.height);

            if (m_UseX) {
                int delta = (int) (pos.x - m_StartPos.x);
                delta = Mathf.Clamp (delta, -MovementRange, MovementRange);
                newPos.x = delta;
            }

            if (m_UseY) {
                int delta = (int) (pos.y - m_StartPos.y);
                delta = Mathf.Clamp (delta, -MovementRange, MovementRange);
                newPos.y = delta;
            }

            Vector2 normalizedPos = newPos.normalized * MovementRange;

            if (newPos.x > normalizedPos.x && newPos.x > 0 || newPos.x < normalizedPos.x && newPos.x < 0)
                newPos.x = normalizedPos.x;
            if (newPos.y > normalizedPos.y && newPos.y > 0 || newPos.y < normalizedPos.y && newPos.y < 0)
                newPos.y = normalizedPos.y;

            rectTransform.anchoredPosition = new Vector3 (m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);

            //transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
            UpdateVirtualAxes (rectTransform.anchoredPosition);
        }

        public void OnPointerUp (PointerEventData data) {
            rectTransform.anchoredPosition = m_StartPos;
            //transform.position = m_StartPos;
            UpdateVirtualAxes (m_StartPos);
        }

        public void OnPointerDown (PointerEventData data) { }

        void OnDisable () {
            // remove the joysticks from the cross platform input
            if (m_UseX) {
                m_HorizontalVirtualAxis.Remove ();
            }
            if (m_UseY) {
                m_VerticalVirtualAxis.Remove ();
            }
        }
    }
}