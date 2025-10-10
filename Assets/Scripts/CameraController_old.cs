using UnityEngine;

public class CameraController_old : MonoBehaviour
{
    public Transform target;   // プレイヤー
    public float distance = 5.0f;       // プレイヤーからの距離
    public float minDistance = 2.0f;    // ズームインの限界距離
    public float maxDistance = 10.0f;   // ズームアウトの限界距離
    public float zoomSpeed = 2.0f;      // ズーム速度

    public float xSpeed = 120.0f; // 水平方向の回転速度
    public float ySpeed = 80.0f;  // 垂直方向の回転速度
    public float yMinLimit = -20f; // 下を向く限界角
    public float yMaxLimit = 60f;  // 上を向く限界角
    public float smoothTime = 0.1f; // 追従スムーズさ

    float x = 0.0f;
    float y = 0.0f;
    Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        if (!target)
            return;

        // マウスによるカメラ回転
        x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        // マウスホイールによるズーム
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // カメラの回転と位置を計算
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 desiredPosition = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        // スムーズに追従
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, smoothTime);
        transform.rotation = rotation;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
