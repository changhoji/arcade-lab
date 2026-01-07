using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class ColorLabPanel : UIPanelBase
{
    [Inject] ColorLabManager m_Manager;
    [Inject] ColorTileManager m_TileManager;
    
    Label m_LeftPlayerScore;
    Label m_RightPlayerScore;
    Label m_TimerLabel;
    VisualElement m_TimerContainer;
    VisualElement m_CountdownOverlay;
    Label m_CountdownLabel;

    protected override void Awake()
    {
        base.Awake();

        m_LeftPlayerScore = m_Root.Q<Label>("left-player-score");
        m_RightPlayerScore = m_Root.Q<Label>("right-player-score");
        m_TimerLabel = m_Root.Q<Label>("timer-label");
        m_TimerContainer = m_Root.Q<VisualElement>("timer-label").parent;
        m_CountdownOverlay = m_Root.Q<VisualElement>("countdown-overlay");
        m_CountdownLabel = m_Root.Q<Label>("countdown-label");
    }

    void Start()
    {
        m_Manager.OnCountdown += ShowCountdown;
        m_Manager.OnStartGame += HideCountdown;
        m_Manager.OnGameTimerTick += UpdateTimer;

        m_TileManager.OnScoreUpdated += UpdateScore;
    }

    void OnDestroy()
    {
        m_Manager.OnCountdown -= ShowCountdown;
        m_Manager.OnStartGame -= HideCountdown;
        m_Manager.OnGameTimerTick -= UpdateTimer;

        m_TileManager.OnScoreUpdated -= UpdateScore;
    }

    public void UpdateLeftPlayerScore(int score)
    {
        m_LeftPlayerScore.text = score.ToString();
    }

    public void UpdateRightPlayerScore(int score)
    {
        m_RightPlayerScore.text = score.ToString();
    }

    public void UpdateTimer(int timer)
    {
        Debug.Log("in panel");
        m_TimerLabel.text = timer.ToString();

        m_TimerContainer.RemoveFromClassList("warning");
        m_TimerContainer.RemoveFromClassList("danger");
        
        if (timer <= 10)
        {
            m_TimerContainer.AddToClassList("danger");
        }
        else if (timer <= 30)
        {
            m_TimerContainer.AddToClassList("warning");
        }
    }

    public void ShowCountdown(int count)
    {
        m_CountdownOverlay.style.display = DisplayStyle.Flex;
        m_CountdownLabel.text = count.ToString();
    }

    public void HideCountdown()
    {
        m_CountdownOverlay.style.display = DisplayStyle.None;
    }

    void UpdateScore(string userId, int score)
    {
        if (userId == "0")
        {
            UpdateLeftPlayerScore(score);
        }
        else
        {
            UpdateRightPlayerScore(score);
        }
    }
}
