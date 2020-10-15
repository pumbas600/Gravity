using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CelestialBody target;
    [SerializeField] private float moveSpeed = 25F;
    [SerializeField] private float zoomSpeed = 30F;
    [SerializeField] private float zoomStepCoefficient = 3F;
    [SerializeField] private float minZoom = 10F;

    [SerializeField] private KeyCode zoomInKey = KeyCode.UpArrow;
    [SerializeField] private KeyCode zoomOutKey = KeyCode.DownArrow;


    private Vector3 initialPosition;
    private Camera cam;
    private float targetZoom;
    [SerializeField, ReadOnly] private bool movingToTarget = false;

    public static event System.EventHandler<CelestialBody> OnSetCameraTarget;

    // Start is called before the first frame update
    private void Start()
    {
        cam = GetComponent<Camera>();
        initialPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    public bool SetTarget(CelestialBody newTarget)
    {
        if (target == newTarget) return false;

        target = newTarget;
        OnSetCameraTarget?.Invoke(this, target);

        StopCoroutine("MoveToTarget");
        StartCoroutine("MoveToTarget");
        return true;
    }


    // Update is called once per frame
    private void Update()
    {
        if (target == null)
        {
            SetTarget(CelestialManager.CentralBody);
        }

        if (Input.GetKeyDown(zoomInKey))
        {
            float step = zoomStepCoefficient * Mathf.Sqrt(cam.orthographicSize);
            targetZoom -= step;
            if (targetZoom < minZoom)
            {
                targetZoom = minZoom;
            }
        }
        else if (Input.GetKeyDown(zoomOutKey))
        {
            float step = zoomStepCoefficient * Mathf.Sqrt(cam.orthographicSize);
            targetZoom += step;
        }

        float zoomVelocity = targetZoom - cam.orthographicSize;
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, 1F, zoomSpeed * Time.deltaTime);

        if (!movingToTarget)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, initialPosition.z);
        }
    }

    private IEnumerator MoveToTarget()
    {
        float elapsedTime = 0;
        movingToTarget = true;

        Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, initialPosition.z);

        float d = (targetPosition - transform.position).magnitude;
        float moveDuration = d / moveSpeed;

        while (elapsedTime < moveDuration)
        {
            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, initialPosition.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime / moveDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, initialPosition.z);
        movingToTarget = false;
    }
}
