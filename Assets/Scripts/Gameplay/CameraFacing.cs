using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CameraFacing : MonoBehaviour
{
    internal Camera main;
    internal BoxCollider2D _col;

    private void Awake()
    {
        if (main == null)
            main = Camera.main;
        if (_col == null)
            _col = GetComponent<BoxCollider2D>();
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    public void LateUpdate()
    {
        var cam = Camera.main.transform.rotation;
        transform.LookAt(transform.position + cam * Vector3.forward,
            cam * Vector3.up);
        Debug.Log("x " + transform.localRotation.x + " y " + transform.localRotation.y);
        //_col.size = new Vector2(transform.localRotation.x, transform.localRotation.y);

    }
}
