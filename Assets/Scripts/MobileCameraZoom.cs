using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class MobileCameraZoom : MonoBehaviour
{
    public float minFov = 35f;
    public float maxFov = 75f;
    public float pinchSensitivity = 0.02f;
    public float mouseWheelSensitivity = 4f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandlePinchZoom();
        HandleMouseWheelZoom();

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFov, maxFov);
    }

    private void HandlePinchZoom()
    {
        if (Input.touchCount != 2)
        {
            return;
        }

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (IsTouchOverUI(touch0) || IsTouchOverUI(touch1))
        {
            return;
        }

        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        float prevDistance = (touch0PrevPos - touch1PrevPos).magnitude;
        float currentDistance = (touch0.position - touch1.position).magnitude;

        float distanceDelta = currentDistance - prevDistance;

        cam.fieldOfView -= distanceDelta * pinchSensitivity;
    }

    private void HandleMouseWheelZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.fieldOfView -= scroll * mouseWheelSensitivity;
        }
    }

    private bool IsTouchOverUI(Touch touch)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
}