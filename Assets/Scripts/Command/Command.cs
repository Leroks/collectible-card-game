using UnityEngine;

public abstract class Command {
    public abstract void Execute ();
    public abstract void End ();
    public abstract void Undo ();
}