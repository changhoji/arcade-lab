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
        var localPlayer = PlayerBase.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = false;
        }
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        var localPlayer = PlayerBase.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.GetComponent<PlayerMovement>().IsMovable = true;
        }
        gameObject.SetActive(false);
    }
}
