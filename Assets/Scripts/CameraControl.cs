using AxesCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] CamMode mode;
    //[SerializeField] GameObject pivot;
    [SerializeField] float rotFactor = 50f;

    [Header("Origins")]
    //Pivot details
    [SerializeField] Vector3 ogPivotPos;
    [SerializeField] Quaternion ogPivotRot;

    //Camera details
    [SerializeField] Vector3 ogCameraPos;
    [SerializeField] Quaternion ogCameraRot;

    [SerializeField] Camera cam; //Active Camera
    [SerializeField] Camera regCam; //Regular camera
    [SerializeField] Camera topCam; //Topdown Camera

    [Header("ToolBar UI")]
    [SerializeField] Image panImage;
    [SerializeField] Image rotImage;
    [SerializeField] Image resetImage;
    [SerializeField] Text cameraModeText;

    [Header("Scale")]
    [SerializeField] GameObject scaleUI;

    void Start()
    {
        mode = CamMode.Pan;

        //Store the original position and rotation of the pivot
        ogCameraPos = this.transform.position;
        ogCameraRot = this.transform.rotation;
        ResetUI();
        panImage.color = Color.green;
    }

    void Update()
    {
        float h = -2f * Input.GetAxis("Mouse X");
        float v = -2f * Input.GetAxis("Mouse Y");

        if(Input.GetMouseButton(0)) //Drag
        {
            transform.position += (transform.right * h + transform.forward * v) * (100f * Time.deltaTime);
        }

        if(Input.GetMouseButton(1) && cam == regCam) //Rotate
        {
            transform.Rotate(Vector3.up, -rotFactor * 5f * h * Time.deltaTime);
        }

        if (Input.mouseScrollDelta.sqrMagnitude > 0)
        {
            ZoomCamera(-Input.mouseScrollDelta.y * 5f);
        }

        if (mode == CamMode.Pan)
        {
            float xMov = Input.GetAxis("Horizontal");
            float yMov = Input.GetAxis("Vertical");

            transform.position += (transform.right * xMov + transform.forward * yMov) * (30f * Time.deltaTime);
        }
        else if (mode == CamMode.Rotate)
        {
            float xRot = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up, -rotFactor * xRot * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.X)) //Put the camera in orthographic mode
        {
            SwitchCameras();
        }
    }

    public void SetCamMode(int _mode)
    {
        mode = (CamMode)_mode;

        ResetUI();
        if (_mode == 0) panImage.color = Color.green;
        if (_mode == 1) rotImage.color = Color.green;
    }

    public void ZoomCamera(float _factor)
    {
        if (!cam.orthographic)
        {
            if (cam.fieldOfView >= 10 && cam.fieldOfView <= 120)
            {
                cam.fieldOfView += (_factor * 60f * Time.deltaTime);
            }

            if (cam.fieldOfView < 10) cam.fieldOfView = 10;
            if (cam.fieldOfView > 120) cam.fieldOfView = 120;
        }
        else
        {
            if(cam.orthographicSize >= 2 && cam.orthographicSize <= 120)
            {
                cam.orthographicSize += _factor;
                scaleUI.GetComponent<ScaleManager>().GenerateScaleNumbers();
            }

            if(cam.orthographicSize < 2)    cam.orthographicSize = 2;
            if(cam.orthographicSize > 120)  cam.orthographicSize = 120;
        }
    }

    public void SwitchCameras()
    {
        if(cam == regCam)
        {
            topCam.gameObject.SetActive(true);
            regCam.gameObject.SetActive(false);
            cam = topCam;
            ResetCamera();
            cameraModeText.text = "Orthographic";
            scaleUI.SetActive(true);
            scaleUI.GetComponent<ScaleManager>().GenerateScaleNumbers();
        }
        else
        {
            regCam.gameObject.SetActive(true);
            topCam.gameObject.SetActive(false);
            cam = regCam;
            cameraModeText.text = "Perspective";
            scaleUI.SetActive(false);
        }
    }

    public void ResetCamera()
    {
        //Reset the pivot to its original
        transform.SetPositionAndRotation(ogCameraPos, ogCameraRot);
        ResetUI();
        panImage.color = Color.green;
        mode = CamMode.Pan;
    }

    public void Reload()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }

    public enum CamMode : int
    {
        Pan = 0,
        Rotate = 1
    }

    public void ResetUI()
    {
        float val = 133f/255f;

        panImage.color = new Color(val, val, val);
        rotImage.color = new Color(val, val, val);
        resetImage.color = new Color(val, val, val);
    }
}
