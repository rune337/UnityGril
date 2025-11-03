using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public GameObject shieldPrefab;
    public float forwardOffset = 0.3f;
    public float duration = 3.0f;

    private GameObject currentShield;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ActivateShield();
        }
    }
    
    void ActivateShield()
    {
        if (shieldPrefab == null)
            return;

        Vector3 position = transform.position + transform.forward * forwardOffset;

        currentShield = Instantiate(shieldPrefab, position, transform.rotation);
        // currentShield.transform.parent = this.transform;
        Destroy(currentShield, duration);
    }
}
