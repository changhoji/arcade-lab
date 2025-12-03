using System;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerBase : MonoBehaviour
{    
    static public PlayerBase LocalPlayer { get; private set; }

    public event Action<int> OnChangeSkin;
    public event Action<string> OnChangeNickname;

    public string UserId { get; private set; }
    public string Nickname { get; private set; }
    public int SkinIndex { get; private set; }
    public bool IsOwner { get; private set; }

    [SerializeField] PlayerLibrary m_PlayerLibrary;
    [SerializeField] TextMeshPro m_NicknameText;

    AuthManager m_AuthManager;
    SpriteLibrary m_SpriteLibrary;

    public void Init(string userId, string nickname, int skinIndex, bool isOwner)
    {
        UserId = userId;
        SetNickname(nickname);
        SetSkinIndex(skinIndex);
        IsOwner = isOwner;
        if (IsOwner)
        {
            LocalPlayer = this;
            m_AuthManager = FindAnyObjectByType<AuthManager>();
        }
    }

    void Awake()
    {
        m_SpriteLibrary = GetComponent<SpriteLibrary>();   
    }

    public void SetNickname(string nickname)
    {
        Nickname = nickname;
        if (m_AuthManager)
        {
            m_AuthManager.Player.nickname = nickname;    
        }
        m_NicknameText.text = nickname;
        OnChangeNickname?.Invoke(nickname);
    }

    public void SetSkinIndex(int skinIndex)
    {
        SkinIndex = skinIndex;
        if (m_AuthManager)
        {
            m_AuthManager.Player.skinIndex = skinIndex;    
        }
        m_SpriteLibrary.spriteLibraryAsset = m_PlayerLibrary.Library[skinIndex];
        OnChangeSkin?.Invoke(skinIndex);
    }
}
