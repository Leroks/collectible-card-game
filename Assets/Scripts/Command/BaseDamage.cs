using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseDamage : Command
{
    List<ParticleSystem> particleSystems;

    public BaseDamage(List<ParticleSystem> particleSystems)
    {
        this.particleSystems = particleSystems;
    }

    public override void Execute()
    {
        particleSystems.OrderBy(qu => Guid.NewGuid()).First().Play();
        Debug.Log("BaseDamage executed!");
    }

    public override void End()
    {
        throw new NotImplementedException();
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }

}
