using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanelBase : MonoBehaviour
{
    public event Action OnShowPanel;
    public event Action OnHidePanel;

    protected UIDocument m_UIDocument;
    protected VisualElement m_Root;
    protected float m_ShownTime;

    protected virtual void Awake()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_Root = m_UIDocument.rootVisualElement;
        m_ShownTime = Time.time;
    }

    protected virtual void Update()
    {
        if (Time.time - m_ShownTime < 0.3f)
        {
            return;
        }

        if (m_Root.style.display == DisplayStyle.None)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public virtual void Show()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = false;
        }
        m_Root.style.display = DisplayStyle.Flex;
        m_ShownTime = Time.time;
        OnShowPanel?.Invoke();
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = true;
        }
        m_Root.style.display = DisplayStyle.None;
        OnHidePanel?.Invoke();
    }
}
