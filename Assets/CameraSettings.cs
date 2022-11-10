using UnityEngine;

[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour
{
    Camera _cam;
    void Awake()
    { 
        _cam = Camera.main;
        if (_cam == null)
        {
            Debug.LogError("MAIN TAG CAMERA IS NULL !!");
            return;
        }
        _cam.orthographic = false;
        _cam.usePhysicalProperties = true;
        if(_cam.transform.parent == null)
            _cam.transform.position = new Vector3 { x = 0, y = 3871, z = 701 };
        else _cam.transform.parent.transform.position = new Vector3 { x = 0, y = 3871, z = 701 };
        _cam.transform.rotation = Quaternion.Euler(80, 180, 0);
        _cam.sensorSize = new Vector2 {x = 36, y = 24 };
        _cam.focalLength = 60f;
        _cam.farClipPlane = 5000f;
        _cam.nearClipPlane = 0.5f;
        _cam.fieldOfView = 22.62f;
    }
}