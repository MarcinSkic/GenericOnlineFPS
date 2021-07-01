using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    Transform playerBody;
    [Header("Sensitivity values")]
    [SerializeField]
    float defaultMouseSensitivity = 400f;
    [SerializeField]
    float mouseSensitivityMin = 100f;
    [SerializeField]
    float mouseSensitivityMax = 1000f;
    [SerializeField]
    float mouseSensitivityChangeMultiplier = 0.9f;
    [Header("Display of current sensitivity")]
    [SerializeField]
    float mouseSensitivity;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        mouseSensitivity = defaultMouseSensitivity;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("p") || Input.GetKeyDown("[") || Input.GetKeyDown("]"))
        {
            if (Input.GetKeyDown("p"))
            {
                mouseSensitivity = defaultMouseSensitivity;
            }
            if (Input.GetKeyDown("["))
            {
                mouseSensitivity *= mouseSensitivityChangeMultiplier;
            }
            if (Input.GetKeyDown("]"))
            {
                mouseSensitivity /= mouseSensitivityChangeMultiplier;
            }
            mouseSensitivity = Mathf.Clamp(mouseSensitivity, mouseSensitivityMin, mouseSensitivityMax);
        }
        

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // * Values.Namespace.Y_AXIS_ROTATION_MULTIPLIER ;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
