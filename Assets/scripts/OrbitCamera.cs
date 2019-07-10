using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UIWidgets;

public class OrbitCamera : MonoBehaviour
{
    public bool isRotating = false;

    public GameObject grid;
    public Transform target;
    public Vector3 targetOrigonalPosition;
    public float distance = 2.0f;
    private float xSpeed = 20.0f;
    private float ySpeed = 20.0f;
    public float xSpeedSensitivity = 8f;
    public float ySpeedSensitivity = 8f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float scrollSensitivity = 3f;
    public float distanceMin = 1f;
    public float distanceMax = 10f;
    public float smoothTime = 2f;
    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;
    public float velocityX = 0.0f;
    public float velocityY = 0.0f;

    public float initialDistance = 10.0f;
    public float initialRotationYAxis = 135.0f;
    public float initialRotationXAxis = 15.0f;

    private bool isMouseDrag;
    private Vector3 screenPosition;
    private Vector3 offset;

    private Splitter[] splitters;

    void Start()
    {
        rotationYAxis = initialRotationYAxis;
        rotationXAxis = initialRotationXAxis;
        distance = initialDistance;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        splitters = FindObjectsOfType<Splitter>();
    }

    public void SetTarget(GameObject targetGameObject, Vector3 center, Vector3 size)
    {
        float magnitude = size.magnitude/* * targetGameObject.transform.localScale.x*/;
        if (target)
            target.localPosition = Vector3.zero;
        target = targetGameObject.transform;
        targetOrigonalPosition = center;
        distance = magnitude * 1.25f;
        distanceMax = distance * 2f;
        scrollSensitivity = magnitude / 5f;
        xSpeed = xSpeedSensitivity / magnitude;
        ySpeed = ySpeedSensitivity / magnitude;
        rotationYAxis = initialRotationYAxis;
        rotationXAxis = initialRotationXAxis;

        Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
        Quaternion rotation = toRotation;
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + targetOrigonalPosition;
        transform.localRotation = rotation;
        transform.localPosition = position;

        grid.SetActive(true);
        grid.transform.SetParent(targetGameObject.transform);
        grid.transform.localPosition = new Vector3(center.x, targetGameObject.GetComponent<SubMeshInfo>().MinY, center.z);
        grid.transform.localScale = new Vector3(magnitude / 5, 1, magnitude / 5);
    }

    void OnGUI()
    {
        if (target && (GetComponent<Camera>().pixelRect.Contains(Input.mousePosition) || (!GetComponent<Camera>().pixelRect.Contains(Input.mousePosition) && isRotating))
            && !CanvasCtrl.Instance.IsMainImageDragging && !CanvasCtrl.Instance.IsGalleryDragging)
        {
            foreach (var splitter in splitters)
                if (splitter.processDrag)
                    return;

            if (Input.GetMouseButton(0))
            {
                isRotating = true;
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * Time.deltaTime;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * distance * Time.deltaTime;
            }
            else
            {
                isRotating = false;
            }
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, distanceMin, distanceMax);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + targetOrigonalPosition;

            transform.localRotation = rotation;
            transform.localPosition = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            //中键鼠标跟踪
            if (Input.GetMouseButtonDown(2))
            {
                isMouseDrag = true;
                screenPosition = Camera.main.WorldToScreenPoint(target.position + targetOrigonalPosition);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
                velocityX = 0;
                velocityY = 0;
            }
            if (Input.GetMouseButtonUp(2))
            {
                isMouseDrag = false;
            }
            if (isMouseDrag)
            {
                Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
                target.transform.position = currentPosition;
            }
        }

        //右键恢复原位
        if (target && GetComponent<Camera>().pixelRect.Contains(Input.mousePosition)
            && Event.current.isMouse && Event.current.button == 1 && Event.current.clickCount == 2)
        {
            target.localPosition = Vector3.zero;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}