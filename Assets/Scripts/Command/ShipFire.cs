using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFire : Command {

    Animator animator;
    string animatorKey;

    public ShipFire (Animator animator, string animatorKey) {
        this.animator = animator;
        this.animatorKey = animatorKey;
    }

    public override void Execute () {
        animator.Play (animatorKey);
    }

    public override void End () {
        throw new NotImplementedException ();
    }

    public override void Undo () {
        throw new NotImplementedException ();
    }
}