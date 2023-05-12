﻿using System;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerData : MonoBehaviour
{
    public UnityEvent<PlayerData> OnScoreUpdated = new UnityEvent<PlayerData>();

    private string _nickName = null;

    private void Start()
    {
        var count = FindObjectsOfType<PlayerData>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
            return;
        }

        ;

        DontDestroyOnLoad(gameObject);
    }

    public void SetNickName(string nickName)
    {
        _nickName = nickName;
    }

    public string GetNickName()
    {
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            _nickName = GetRandomNickName();
        }

        return _nickName;
    }

    public static string GetRandomNickName()
    {
        var rngPlayerNumber = Random.Range(0, 9999);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }

    void OnScoreChanged(uint oldScore, uint newScore)
    {
        OnScoreUpdated?.Invoke(this);
    }
    
}