using AxesCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] CamMode mode;
    [SerializeField] GameObject pivot;
    [SerializeField] float rotFactor = 50f;

    [Header("Origins")]
    //Pivot details
    [SerializeField] Vector3 ogPivotPos;
    [SerializeField] Quaternion ogPivotRot;

    //Camera details
    [SerializeField] Vector3 ogCameraPos;
    [SerializeField] Quaternion ogCameraRot;

    [SerializeField] Camera cam;

    [Header("ToolBar UI")]
    [SerializeField] Image panImage;
    [SerializeField] Image rotImage;
    [SerializeField] Image resetImage;

    void Start()
    {
        mode = CamMode.Pan;

        //Store the original position and rotation of the pivot
        ogPivotPos = pivot.transform.position;
        ogCameraPos = this.transform.position;

        ogPivotRot = pivot.transform.rotation;
        ogCameraRot = this.transform.rotation;
        ResetUI();
        panImage.color = Color.green;
    }

    void Update()
    {
        if (Input.mouseScrollDelta.sqrMagnitude > 0)
        {
            ZoomCamera(-Input.mouseScrollDelta.y);
        }

        if (mode == CamMode.Pan)
        {
            float xMov = Input.GetAxis("Horizontal");
            float yMov = Input.GetAxis("Vertical");

            transform.position += (transform.right * xMov + transform.forward * yMov) * (15f * Time.deltaTime);
        }
        else if (mode == CamMode.Rotate) //TODO: Add mouse rotation
        {
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
    }

    public void ZoomCamera(float _factor)
    {
        if (cam.fieldOfView >= 10 && cam.fieldOfView <= 120)
        {
            cam.fieldOfView += _factor;
        }

        if (cam.fieldOfView < 10) cam.fieldOfView = 10;
        if (cam.fieldOfView > 120) cam.fieldOfView = 120;
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
