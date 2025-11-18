using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WardrobePanel : MonoBehaviour
{
    const int k_NumberOfSkin = 4;
    [SerializeField] PlayerLibrary m_PlayerLibrary;
    [SerializeField] Image[] m_Images;
    [SerializeField] TMP_InputField m_NicknameInput;
    int m_SelectedIndex = 0;
    int m_PreviousIndex = 0;
    PlayerController m_Player;

    void Start()
    {
        for (int i = 0; i < k_NumberOfSkin; i++)
        {
            Debug.Log($"i = {i}");
            m_Images[i].sprite = m_PlayerLibrary.Library[i].GetSprite("Idle", "Idle_00");
            m_Images[i].color = i == 0 ? Color.white : Color.grey;
        }
        m_NicknameInput.onEndEdit.AddListener(ChangeNickname);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_SelectedIndex = (m_SelectedIndex + k_NumberOfSkin - 1) % k_NumberOfSkin;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_SelectedIndex = (m_SelectedIndex + 1) % 4;
        }

        if (m_SelectedIndex != m_PreviousIndex)
        {
            if (m_Player)
            {
                m_Player.SetSkinIndex(m_SelectedIndex);
            }
            m_Images[m_PreviousIndex].color = Color.grey;
            m_Images[m_SelectedIndex].color = Color.white;
            m_PreviousIndex = m_SelectedIndex;
        }
        
    }

    public void Show(PlayerController player)
    {
        m_Player = player;
        m_Player.SetIsMovable(false);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (m_Player)
        {
            m_Player.SetIsMovable(true);
            m_Player = null;
        }
        gameObject.SetActive(false);
    }

    public void ChangeNickname(string nickname)
    {
        Debug.Log($"value = {nickname}");
        if (m_Player)
        {
            m_Player.SetNickname(nickname);
        }
    }
}
