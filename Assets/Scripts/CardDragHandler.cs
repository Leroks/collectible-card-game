using System.Collections;
using System.Collections.Generic;
using StormWarfare.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour {

    private GameObject selectedCard;
    private Camera cam;
    private Quaternion rotation;
    private Vector3 initialPosition;
    private float oldX;
    public GameObject baseContainer;
    private BaseContainer container;

    // Start is called before the first frame update
    void Start () {
        cam = Camera.main;
        container = baseContainer.GetComponent<BaseContainer> ();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            if (selectedCard == null) {
                RaycastHit hit = CastRay ();
                if (hit.collider != null && hit.collider.CompareTag (Constants.TagNames.card)) {
                    selectedCard = hit.collider.gameObject;
                    oldX = Input.mousePosition.x;
                    rotation = selectedCard.transform.rotation;
                    initialPosition = selectedCard.transform.position;
                    selectedCard.transform.rotation = Quaternion.Euler (0, 0, 0);
                }
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            if (selectedCard != null) {
                Vector3 position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint (selectedCard.transform.position).z);
                Vector3 worldPosition = cam.ScreenToWorldPoint (position);
                // TODO: replace 2350 with upper bound of card container area
                if (position.z > 2350) {
                    selectedCard.transform.position = initialPosition;
                    selectedCard.transform.rotation = rotation;
                } else {
                    //container.OrderHand ();
                }

                selectedCard = null;
            }
        }

        if (selectedCard != null) {
            Vector3 position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint (selectedCard.transform.position).z);
            Vector3 worldPosition = cam.ScreenToWorldPoint (position);
            selectedCard.transform.position = new Vector3 (worldPosition.x, Constants.GameSettings.cardDragYPosition, worldPosition.z);

            if (System.Math.Abs (oldX - position.x) > 1) {
                // TODO: replace 400 with center of screen
                selectedCard.transform.rotation = Quaternion.Euler (0, -1 * (400 - position.x) / 10, 0);
            } else {
                selectedCard.transform.rotation = Quaternion.Euler (0, 0, 0);
            }

            selectedCard.transform.SetAsLastSibling ();
        }
    }

    private RaycastHit CastRay () {

        Vector3 screenMousePosNear = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
        Vector3 screenMousePosFar = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, cam.farClipPlane);

        Vector3 worldMousePosNear = cam.ScreenToWorldPoint (screenMousePosNear);
        Vector3 worldMousePosFar = cam.ScreenToWorldPoint (screenMousePosFar);

        RaycastHit hit;
        Physics.Raycast (worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}