using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    GameObject player;
    Vector3 playerPos;
    Vector3 defaultPos = new Vector3(0, 4, -4);

    float pitch = 0f; //y座標
    float yaw = 0f; //x座標
    public float sensitivity = 2f; //はやく動かすための係数

    public float zoomSpeed = 5f; //拡大する速さ
    public float minZoom = 2f; //ズームインの限界値(カメラが一番近づける距離)
    public float maxZoom = 10f; //ズームアウトの限界値(カメラが一番遠ざかる距離)
    float currentZoom = 5f; //現在のカメラ位置

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform.position;
        transform.position = playerPos;
        transform.position = playerPos + defaultPos;
        Debug.Log("カメラ位置 " + transform.position);

        Cursor.lockState = CursorLockMode.Locked; //マウスカーソルを画面中央に固定して見えなくする
        Cursor.visible = false; //カーソルを非表示
    }

    void Update()
    {
        //プレイヤーいなくなった時
        if (player == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity; //マウスの横方向の動きの量を変数に格納
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity; //マウスの縦方向の動きの量を変数に格納

        float scroll = Input.GetAxis("Mouse ScrollWheel"); //マウスホイールを回した量を変数に格納

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -60f, 60f);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f); //現在の角度に関係なく自身のオブジェクトをxとyの角度に回転させる
        player.transform.Rotate(Vector3.up * mouseX); //プレイヤーの現在の角度に回転をマウスの横移動だけ回転させる

        //ホイールを上に回すとscroll が+なのでcurrentZoomが減る（ズームイン）
        //ホイールを下に回すとscroll が-なのでcurrentZoomが増える（ズームアウト）
        currentZoom -= scroll * zoomSpeed; 
        
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); //ズームの限界値の幅に制限する
    }


    void LateUpdate()
    {
        //プレイヤーいなくなった時
        if (player == null)
        {
            return;
        }

        Vector3 offset = Quaternion.Euler(pitch, yaw, 0f) * new Vector3(0, 0, -currentZoom); //カメラの向き * カメラの位置
        Vector3 targetPos = player.transform.position + offset; //カメラの位置をオフセット分ずらす値を変数に代入

        // 地面より下に潜らないように制限
        float groundY = player.transform.position.y + 0.3f; // プレイヤーの足元より少し上

        //カメラが設定値より下になったら設定値に戻して設定値より下にカメラが行かないようにする
        if (targetPos.y < groundY)
        {
            targetPos.y = groundY;
        }

        transform.position = targetPos; //カメラの位置を変更する
        transform.LookAt(player.transform.position + Vector3.up * 1.5f); //カメラを常にプレイヤーに向かせる
    }

}
