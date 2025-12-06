using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanelBase : MonoBehaviour
{
    protected UIDocument m_UIDocument;
    protected VisualElement m_Root;

    protected virtual void Awake()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_Root = m_UIDocument.rootVisualElement;
    }

    protected virtual void Update()
    {
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
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = true;
        }
        m_Root.style.display = DisplayStyle.None;
    }
}
