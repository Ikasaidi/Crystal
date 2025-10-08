using UnityEngine;
// Résultat : quand le joueur passe dans un Checkpoint, 
//on mémorise la position.
// Quand il perd une vie, on le TP au dernier checkpoint 
//(ou au start s’il n’y en a pas encore).


public static class CheckpointManager
{
    public static bool HasCheckpoint { get; private set; }
    public static Vector3 LastCheckpoint { get; private set; }

    public static void SetCheckpoint(Vector3 pos)
    {
        LastCheckpoint = pos;
        HasCheckpoint = true;
    }

    public static void Clear()
    {
        HasCheckpoint = false;
    }
}
