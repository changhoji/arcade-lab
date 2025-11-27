using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour
{
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public virtual void Show()
    {
        var localPlayer = PlayerController.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.SetIsMovable(false);
        }
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerController.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.SetIsMovable(true);
        }
        gameObject.SetActive(false);
    }
}
