using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDowngrade : Command {

    ParticleSystem particleSystem;

    public HealthDowngrade (ParticleSystem particleSystem) {
        this.particleSystem = particleSystem;
    }

    public override void Execute () {
        particleSystem.Play ();
    }

    public override void End () {
        throw new NotImplementedException ();
    }

    public override void Undo () {
        throw new NotImplementedException ();
    }
}