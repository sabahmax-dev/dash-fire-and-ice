using UnityEngine;
using TMPro;

/// <summary>
/// 分数UI控制器
/// 显示当前分数和最高分
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("样式设置")]
    [SerializeField] private Color scoreColor = Color.white;
    [SerializeField] private Color highScoreColor = Color.yellow;
    [SerializeField] private int baseFontSize = 48;

    private int lastScore = -1;
    private int lastHighScore = -1;

    private void Awake()
    {
        // 自动获取UI组件（如果未手动赋值）
        if (currentScoreText == null)
        {
            currentScoreText = transform.Find("CurrentScore")?.GetComponent<TextMeshProUGUI>();
        }
        if (highScoreText == null)
        {
            highScoreText = transform.Find("HighScore")?.GetComponent<TextMeshProUGUI>();
        }

        // 设置文本样式
        if (currentScoreText != null)
        {
            currentScoreText.color = scoreColor;
            currentScoreText.fontSize = baseFontSize;
            currentScoreText.alignment = TextAlignmentOptions.Right;
        }

        if (highScoreText != null)
        {
            highScoreText.color = highScoreColor;
            highScoreText.fontSize = baseFontSize * 0.7f;
            highScoreText.alignment = TextAlignmentOptions.Right;
        }

        Debug.Log("ScoreUI: 初始化完成");
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        // 只在分数变化时更新UI
        if (lastScore != GameManager.Instance.CurrentScore)
        {
            UpdateScoreText();
            lastScore = GameManager.Instance.CurrentScore;
        }

        if (lastHighScore != GameManager.Instance.HighScore)
        {
            UpdateHighScoreText();
            lastHighScore = GameManager.Instance.HighScore;
        }
    }

    private void UpdateScoreText()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = $"SCORE: {GameManager.Instance.CurrentScore}";
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"HIGH SCORE: {GameManager.Instance.HighScore}";
        }
    }

    /// <summary>
    /// 强制刷新UI（游戏重开时调用）
    /// </summary>
    public void RefreshUI()
    {
        lastScore = -1;
        lastHighScore = -1;
    }
}
