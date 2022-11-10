using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackLine : MonoBehaviour {
    private LineRenderer lineRenderer;
    private Vector3 startingPosition;
    private Vector3 position;
    private Camera cam;
    private float timer;
    private float delay = 0.05f;

    // Start is called before the first frame update
    void Start () {
        cam = Camera.main;
        lineRenderer = this.GetComponent<LineRenderer> ();
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material (Shader.Find ("Sprites/Default"));
        lineRenderer.startWidth = 0.25f;
        lineRenderer.endWidth = 0.25f;
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        timer = delay;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            lineRenderer.positionCount = 2;
            startingPosition = GetMousePosition ();
        }

        timer -= Time.deltaTime;

        if (Input.GetMouseButton (0)) {

            if (timer <= 0) {
                timer = delay;
                position = GetMousePosition ();

                lineRenderer.SetPosition (0, startingPosition);
                lineRenderer.SetPosition (1, position);
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            StartCoroutine (ClearLine ());
        }
    }

    private Vector3 GetMousePosition () {
        Ray ray = cam.ScreenPointToRay (Input.mousePosition);
        return ray.origin + (ray.direction * 15);
    }

    private IEnumerator ClearLine () {
        yield return new WaitForSeconds (0.5f);
        lineRenderer.positionCount = 0;
    }
}