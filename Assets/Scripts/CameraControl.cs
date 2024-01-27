using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public CamMode mode;
    [SerializeField] GameObject pivot;
    [SerializeField] float rotFactor = 50f;

    [Header("Origins")]
    //Store the original position and rotation of the pivot
    [SerializeField] Vector3 ogPivotPos;
    [SerializeField] Quaternion ogPivotRot;
    //Store the original positon and rotation of the camera
    [SerializeField] Vector3 ogCameraPos;
    [SerializeField] Quaternion ogCameraRot;

    [Header("ToolBar UI")]
    [SerializeField] Image panImage;
    [SerializeField] Image rotImage;
    [SerializeField] Image resetImage;

    void Start()
    {
        Debug.Log("Camera Script Active");
        ogPivotPos = pivot.transform.position;
        ogCameraPos = this.transform.position;

        ogPivotRot = pivot.transform.rotation;
        ogCameraRot = this.transform.rotation;
        ResetUI();
        panImage.color = Color.green;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCamera();
        }

        if (Input.mouseScrollDelta.sqrMagnitude > 0)
        {
            ZoomCamera(Input.mouseScrollDelta.y);
        }
        if (mode == CamMode.Pan)
        {
            float xMov = Input.GetAxis("Horizontal");
            float yMov = Input.GetAxis("Vertical");

            transform.position += (transform.right * xMov + transform.up * yMov) * 2 * Time.deltaTime;
        }
        else if (mode == CamMode.Rotate)
        {
            //TODO: Add mouse rotation
            float xRot = Input.GetAxis("Horizontal");
            pivot.transform.Rotate(Vector3.up, -rotFactor * xRot * Time.deltaTime);
        }
    }

    public void SetCamMode(int _mode)
    {
        mode = (CamMode)_mode;

        ResetUI();
        if (_mode == 0) panImage.color = Color.green;
        if (_mode == 1) rotImage.color = Color.green;

        Debug.Log("Camera Mode Set To: " + (CamMode)_mode);
    }

    public void ZoomCamera(float _factor)
    {
        Camera cam = GetComponent<Camera>();
        if (cam.fieldOfView > 30 && cam.fieldOfView < 90)
        {
            cam.fieldOfView += _factor;
        }
    }

    public void ResetCamera()
    {
        //Reset the pivot to its original
        pivot.transform.position = ogPivotPos;
        pivot.transform.rotation = ogPivotRot;

        //Reset the camera to its original
        transform.position = ogCameraPos;
        transform.rotation = ogCameraRot;

        ResetUI();
        panImage.color = Color.green;
        mode = CamMode.Pan;
        Debug.Log("Reset The Camera");
    }

    public enum CamMode : int
    {
        Pan = 0,
        Rotate = 1
    }

    #region UI Handling
    public void ResetUI()
    {
        panImage.color = Color.white;
        rotImage.color = Color.white;
        resetImage.color = Color.white;
    }
    #endregion
}
