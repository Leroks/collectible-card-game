using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMechanism : Command
{
    Animator animator;
    string animatorKey;

    public BaseMechanism(Animator animator, string animatorKey)
    {
        this.animator = animator;
        this.animatorKey = animatorKey;
    }

    public override void Execute()
    {
        animator.Play(animatorKey);
    }

    public override void End()
    {
        animator.StopPlayback();
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
