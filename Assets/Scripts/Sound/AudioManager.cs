using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static GameObject mixer = (GameObject)Resources.Load("Sounds/Mixer/AudioMixerSource");
    public static void PlaySound(string sound)
    {
        var tempAudio = (AudioClip)Resources.Load("Sounds/" + sound);
        if (tempAudio == null) return;
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.GetComponent<AudioSource>().outputAudioMixerGroup;
        audioSource.volume = .25f;
        audioSource.clip = tempAudio;
        audioSource.Play();
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }
}