using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : NetworkBehaviour
{
    [SyncVar] public int index;
    
    [SyncVar(hook = nameof(OnScoreChanged))] public uint score;
    public UnityEvent<PlayerScore> OnScoreUpdated = new UnityEvent<PlayerScore>();

    void OnGUI()
    {
        GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
    }

    void OnScoreChanged(uint oldScore, uint newScore)
    {
        OnScoreUpdated?.Invoke(this);
    }
    
}