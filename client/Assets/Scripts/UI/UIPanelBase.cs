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
        localPlayer.SetIsMovable(false);
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerController.LocalPlayer;
        localPlayer.SetIsMovable(true);
        gameObject.SetActive(false);
    }
}
