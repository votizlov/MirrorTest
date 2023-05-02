using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class WinScreen : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnWinScreenStateChanged))] public bool showWinner;
    [SyncVar(hook = nameof(OnWinScreenTextChanged))] public string winnerName;

    [SerializeField] private TMP_Text _winnerText;
    private GameObject _child;
    void Start()
    {
        GameStateManager.Instance.winScreen = this;
        _child = transform.GetChild(0).gameObject;
        _child.SetActive(false);
    }
    
    private void OnWinScreenStateChanged(bool old, bool newVal)
    {
        _child.SetActive(newVal);
    }

    private void OnWinScreenTextChanged(string old, string newText)
    {
        _winnerText.text = "Winner is " + newText;
    }
}
