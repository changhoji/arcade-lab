using System;
using ArcadeLab.Data;
using UnityEngine;

public interface IAuthManager
{
    string UserId { get; }
    bool IsAuthenticated { get; }
    PlayerBaseData Player { get; }
    event Action OnSignInSuccess;
    void SignInAnonymously();
}
