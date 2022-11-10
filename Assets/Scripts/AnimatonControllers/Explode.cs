using UnityEngine;

public class Explode : MonoBehaviour
{
    ParticleSystem system;

    void Start()
    {
        system = gameObject.GetComponent<ParticleSystem>();
    }

    public void PlayParticleSystem()
    {
        system.Play();
    }

    public void StopParticleSystem()
    {
        system.Stop();
    }

    private void OnCollisionEnter(Collision other)
    {
        PlayParticleSystem();
    }
}
