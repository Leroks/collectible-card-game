using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StormWarfare.Gameplay;
using UnityEngine;

public class BaseWeopen : Command {

    List<ParticleGroup> particleGroups;
    System.Random rand = new System.Random ();
    MonoBehaviour mono;

    public BaseWeopen (List<ParticleGroup> particleGroups, MonoBehaviour mono) {
        this.particleGroups = particleGroups;
        this.mono = mono;
    }

    public override void Execute () {
        var item = particleGroups.OrderBy (qu => Guid.NewGuid ()).First ();
        mono.StartCoroutine (RunAnimations (item));
    }

    public override void End () {
        throw new NotImplementedException ();
    }

    public override void Undo () {
        throw new NotImplementedException ();
    }

    private IEnumerator RunAnimations (ParticleGroup group) {
        foreach (var item in group.particleSystems) {
            item.Play ();
            yield return new WaitForSeconds ((float) rand.NextDouble () / 2);
        }
    }
}