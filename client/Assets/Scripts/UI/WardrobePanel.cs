using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class WardrobePanel : UIPanelBase
{
    const int k_NumberOfSkin = 4;
    [SerializeField] PlayerLibrary m_PlayerLibrary;

    VisualElement[] m_SkinPreviews;
    TextField m_NicknameInput;
    Button m_PrevButton;
    Button m_NextButton;

    int m_SelectedIndex = 0;
    int m_PreviousIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        m_SkinPreviews = new VisualElement[k_NumberOfSkin];
        for (int i = 0; i < k_NumberOfSkin; i++)
        {
            m_SkinPreviews[i] = m_Root.Q<VisualElement>($"skin-{i}");
        }

        m_NicknameInput = m_Root.Q<TextField>("nickname-input");
        m_PrevButton = m_Root.Q<Button>("prev-button");
        m_NextButton = m_Root.Q<Button>("next-button");
    }

    void Start()
    {
        for (int i = 0; i < k_NumberOfSkin; i++)
        {
            var sprite = m_PlayerLibrary.Library[i].GetSprite("Idle", "Idle_00");
            m_SkinPreviews[i].style.backgroundImage = new StyleBackground(sprite);
        }

        m_NicknameInput.RegisterCallback<FocusOutEvent>(evt => ChangeNickname(m_NicknameInput.text));
        m_PrevButton.clicked += () => ChangeSkin(-1);
        m_NextButton.clicked += () => ChangeSkin(1);
        
        Hide();
    }

    protected override void Update()
    {
        base.Update();

        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     m_SelectedIndex = (m_SelectedIndex + k_NumberOfSkin - 1) % k_NumberOfSkin;
        // }
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     m_SelectedIndex = (m_SelectedIndex + 1) % 4;
        // }
    }

    public override void Show()
    {
        base.Show();
        
        Debug.Log("wardrobepanel.show");
    }

    public void ChangeNickname(string nickname)
    {
        PlayerBase.LocalPlayer.SetNickname(nickname);
    }

    void ChangeSkin(int direction)
    {
        m_SelectedIndex = (m_SelectedIndex + k_NumberOfSkin + direction) % k_NumberOfSkin;

        Debug.Log($"prev = {m_PreviousIndex} / selected = {m_SelectedIndex}");

        if (m_SelectedIndex != m_PreviousIndex)
        {
            PlayerBase.LocalPlayer.SetSkinIndex(m_SelectedIndex);

            m_SkinPreviews[m_PreviousIndex].RemoveFromClassList("skin-active");
            m_SkinPreviews[m_PreviousIndex].AddToClassList("skin-inactive");

            m_SkinPreviews[m_SelectedIndex].RemoveFromClassList("skin-inactive");
            m_SkinPreviews[m_SelectedIndex].AddToClassList("skin-active");

            m_PreviousIndex = m_SelectedIndex;
        }
    }
}
