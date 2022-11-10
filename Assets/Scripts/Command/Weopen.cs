using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weopen : Command {

    List<ParticleSystem> particleSystems;

    public Weopen (List<ParticleSystem> particleSystems) {
        this.particleSystems = particleSystems;
    }

    public override void Execute () {
        particleSystems.OrderBy (qu => Guid.NewGuid ()).First ().Play ();
    }

    public override void End () {
        throw new NotImplementedException ();
    }

    public override void Undo () {
        throw new NotImplementedException ();
    }
}