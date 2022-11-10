using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    private Renderer _renderer;

    public CameraShake cameraShake;

    private Animator _animator;

    [SerializeField] GameObject _gameObject;

    [SerializeField] List<GameObject> muzzle = new List<GameObject>();

    [SerializeField] float cameraShakeMagnitude = 4f;

    [SerializeField] string animationString;

    [SerializeField] Animator otherAnimator;

    [SerializeField] string otherAnimationString;

    Command shipFire, otherShipFire;

    [SerializeField] float waitForSeconds;
    [SerializeField] float colliderDelayTime = 6.3f;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _animator = GetComponent<Animator>();
        shipFire = new ShipFire(_animator, animationString);
        otherShipFire = new ShipFire(otherAnimator, otherAnimationString);
    }

    private void OnMouseDown()
    {
        StartCoroutine(Delay());
        StartCoroutine(ColliderDelay());
    }

    private void Fire()
    {
        foreach (GameObject m in muzzle)
        {
            m.GetComponent<Explode>().PlayParticleSystem();
        }
        if (animationString == "Dido_Ship")
        {
            StartCoroutine(AnimationDelay());
        }

        StartCoroutine(cameraShake.Shake(0.15f, cameraShakeMagnitude));
    }

    IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(.5f);
        AudioManager.PlaySound("Church Fire");
    }

    IEnumerator Delay()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        shipFire.Execute();
        if (otherAnimator && otherAnimationString != null)
        {
            otherShipFire.Execute();
            GetComponent<BoxCollider2D>().enabled = false;
        }
        yield return new WaitForSeconds(waitForSeconds);
        AudioManager.PlaySound(animationString);
    }

    IEnumerator ColliderDelay()
    {
        yield return new WaitForSeconds(colliderDelayTime);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
