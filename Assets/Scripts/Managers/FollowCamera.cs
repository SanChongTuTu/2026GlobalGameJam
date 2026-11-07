using UnityEngine;

public class FollowCamera: MonoBehaviour
{
    [Header("Target")]
    public Transform target;      

    [Header("Offset")]
    public Vector3 offset;

    [Header("Smooth")]
    public float smoothSpeed;

    [Header("Boundary")]
    public bool useBound = false;
    public Vector2 minBound;
    public Vector2 maxBound;

    [Header("拉远镜头 Size")]
    public float normalSize = 5f;    // 正常大小
    public float farSize = 8f;       // 拉远后的大小
    public float sizeSmooth = 8f;    // 平滑速度
    private bool isZoomOut = false;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        normalSize = cam.orthographicSize; // 自动记录原始size
    }

    public void ZoomOutFar()
    {
        isZoomOut = true;
    }

    // 调用这个：恢复正常镜头
    public void ZoomNormal()
    {
        isZoomOut = false;
    }

    void Update()
    {
        if (cam == null) return;

        if (isZoomOut)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, farSize, sizeSmooth * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, normalSize, sizeSmooth * Time.deltaTime);
        }
    }

    void LateUpdate()   
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        if (useBound)
        {
            smoothedPos.x = Mathf.Clamp(smoothedPos.x, minBound.x, maxBound.x);
            smoothedPos.y = Mathf.Clamp(smoothedPos.y, minBound.y, maxBound.y);
        }

        transform.position = smoothedPos;
    }
}