using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanelBase : MonoBehaviour
{
    public event Action OnShowPanel;
    public event Action OnHidePanel;

    [SerializeField] protected bool m_CloseOnEscape = true;
    [SerializeField] protected bool m_PreventMovement = true;
    [SerializeField] protected bool m_ModalBackground = true;

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

        if (m_CloseOnEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public virtual void Show()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (m_PreventMovement && localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = false;
        }
        m_Root.style.display = DisplayStyle.Flex;
        m_ShownTime = Time.time;

        if (m_ModalBackground)
        {
            m_Root.AddToClassList("dimmed");
        }
        else
        {
            m_Root.RemoveFromClassList("dimmed");
        }

        OnShowPanel?.Invoke();
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (m_PreventMovement && localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = true;
        }
        m_Root.style.display = DisplayStyle.None;
        OnHidePanel?.Invoke();
    }
}
