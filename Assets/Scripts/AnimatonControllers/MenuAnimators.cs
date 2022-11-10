using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimators : MonoBehaviour
{
    private Renderer _renderer;

    [SerializeField] Animator otherAnimator;
    [SerializeField] Animator menuAnimator;
    [SerializeField] Animator fireAnimator;
    [SerializeField] Animator subButtonCloseAnimator;
    [SerializeField] string otherAnimationString;
    [SerializeField] string menuAnimatorString;
    [SerializeField] string otherFireString;
    [SerializeField] string subButtonCloseString;
    Command shipFire, otherShipFire, otherFire, subButton;

    [SerializeField] GameObject toActivate;
    [SerializeField] Collider2D toDeactivateCollider;
    [SerializeField] float colliderDelayTime = 1.0f;
    [SerializeField] bool colliderDelayBool = false;
    [SerializeField] float toActivateDelayTime = 0;
    [SerializeField] GameObject toMinimize;

    bool subButtonCheck = true;
    [SerializeField] GameObject toActivateOther;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        otherShipFire = new ShipFire(otherAnimator, otherAnimationString);
        shipFire = new ShipFire(menuAnimator, menuAnimatorString);
        otherFire = new ShipFire(fireAnimator, otherFireString);
        subButton = new ShipFire(subButtonCloseAnimator, subButtonCloseString);
    }

    public void OnMouseDown()
    {
        if (colliderDelayBool)
        {
            StartCoroutine(ColliderDelay());
        }
        if (otherAnimator && otherAnimationString != null)
        {
            otherShipFire.Execute();
        }

        if (menuAnimator && menuAnimatorString != null)
        {
            if (subButtonCheck)
            {
                Debug.Log("SubButton Open");
                shipFire.Execute();
            }
            if (subButtonCloseAnimator && subButtonCloseString != null)
            {
                subButtonCheck = !subButtonCheck;
                if (subButtonCheck)
                {
                    Debug.Log("SubButton Close");
                    subButton.Execute();
                    toActivate.SetActive(false);
                }
            }
        }

        if (fireAnimator && otherFireString != null)
        {
            otherFire.Execute();
        }

        if (toActivate != null)
        {
            if (!subButtonCheck)
            {
                StartCoroutine(ActivationDelay());
            }
        }
        if (toDeactivateCollider != null)
        {
            toDeactivateCollider.enabled = false;
        }

        if (toMinimize != null)
        {
            toMinimize.transform.localScale = new Vector3(transform.localScale.x * 0.8f, transform.localScale.x * 0.8f, 1);
            StartCoroutine(DeActivationDelay());
        }

        if (toActivateOther != null)
        {

            toActivateOther.SetActive(true);
        }



        AudioManager.PlaySound(menuAnimatorString);
    }

    IEnumerator ActivationDelay()
    {
        yield return new WaitForSeconds(toActivateDelayTime);
        toActivate.SetActive(true);
    }

    IEnumerator ColliderDelay()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(colliderDelayTime);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    IEnumerator DeActivationDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (toMinimize.name != "Back_Tus")
        {
            toMinimize.SetActive(false);
        }
        toMinimize.transform.localScale = new Vector3(transform.localScale.x * 10 / 8, transform.localScale.x * 10 / 8, 1);
    }
}
