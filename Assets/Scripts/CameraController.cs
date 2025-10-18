using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
   
    GameObject player; 
    Vector3 playerPos; 
    Vector3 defaultPos = new Vector3(0,4,-4); 

    
    float pitch = 0f; 
    float yaw = 0f;   
    public float sensitivity = 2f;

    
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    float currentZoom = 5f;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 
        playerPos = player.transform.position; 
        transform.position = playerPos;
        transform.position = playerPos + defaultPos;
        Debug.Log("カメラ位置 " + transform.position);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; //カーソルを非表示
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        yaw += mouseX;  
        pitch -= mouseY; 
        pitch = Mathf.Clamp(pitch, -60f, 60f); 


        //transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        //transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        
        player.transform.Rotate(Vector3.up * mouseX);


        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        
        //transform.localPosition = new Vector3(0, 0, -currentZoom);


    }


    void LateUpdate()
    {
       
        Vector3 offset = Quaternion.Euler(pitch, yaw, 0f) * new Vector3(0, 0, -currentZoom);
        transform.position = player.transform.position + offset;

       
        transform.LookAt(player.transform.position + Vector3.up * 1.5f); 
    }

}
