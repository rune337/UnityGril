using UnityEngine;
using UnityEngine.InputSystem;

public class WinCameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 2f, -5f);

  
    void Start()
    {
         if (player != null)
        {
            // カメラの位置をプレイヤーの位置 + オフセットに設定
            transform.position = player.position + offset;

            // カメラがプレイヤーの方向を向くように設定
            transform.LookAt(player);
        }
  
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}