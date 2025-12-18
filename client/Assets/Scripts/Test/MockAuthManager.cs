using System;
using ArcadeLab.Data;
using ParrelSync;
using UnityEngine;
using Random = UnityEngine.Random;

public class MockAuthManager : MonoBehaviour, IAuthManager
{
    public string UserId { get; private set; }
    public bool IsAuthenticated => true;
    public PlayerBaseData Player { get; private set; }
    public event Action OnSignInSuccess;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (ClonesManager.IsClone())
        {
            var arg = ClonesManager.GetArgument();
            Player = new PlayerBaseData
            {
                userId = arg,
                nickname = $"Guest{arg}",
                skinIndex = Random.Range(0, 4),
            };
            UserId = arg;
        }
        else
        {
            Player = new PlayerBaseData
            {
                userId = "0",
                nickname = "Guest0",
                skinIndex = Random.Range(0, 4),
            };
            UserId = "0";
        }
    }

    public void SignInAnonymously()
    {
        
    }
}
