using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// 游戏结束界面控制器
/// 显示最终分数、最高分，提供重新开始按钮
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private Button restartButton;

    [Header("样式设置")]
    [SerializeField] private Color titleColor = Color.red;
    [SerializeField] private Color scoreColor = Color.white;
    [SerializeField] private Color newRecordColor = Color.yellow;

    private bool isNewRecord = false;

    private void Awake()
    {
        // 自动获取UI组件（如果未手动赋值）
        if (gameOverPanel != null)
        {
            gameOverTitleText = gameOverPanel.transform.Find("GameOverTitle")?.GetComponent<TextMeshProUGUI>();
            finalScoreText = gameOverPanel.transform.Find("FinalScore")?.GetComponent<TextMeshProUGUI>();
            highScoreText = gameOverPanel.transform.Find("HighScore")?.GetComponent<TextMeshProUGUI>();
            newRecordText = gameOverPanel.transform.Find("NewRecord")?.GetComponent<TextMeshProUGUI>();
            restartButton = gameOverPanel.transform.Find("RestartButton")?.GetComponent<Button>();
        }

        // 隐藏游戏结束面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 设置重新开始按钮事件
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }

        Debug.Log("GameOverUI: 初始化完成");
    }

    private void Update()
    {
        // 游戏结束且面板显示时，按空格或点击可重新开始
        if (gameOverPanel != null && gameOverPanel.activeSelf &&
            GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            bool spacePressed = Input.GetKeyDown(KeyCode.Space);
            bool tapPressed = Input.GetMouseButtonDown(0);

            if (spacePressed || tapPressed)
            {
                RestartGame();
            }
        }
    }

    /// <summary>
    /// 显示游戏结束界面
    /// </summary>
    public void ShowGameOver()
    {
        if (GameManager.Instance == null) return;

        isNewRecord = GameManager.Instance.CurrentScore >= GameManager.Instance.HighScore;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 设置标题文本
        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = "GAME OVER";
            gameOverTitleText.color = titleColor;
            gameOverTitleText.fontSize = Screen.height * 0.08f;
            gameOverTitleText.alignment = TextAlignmentOptions.Center;
        }

        // 设置最终分数
        if (finalScoreText != null)
        {
            finalScoreText.text = $"SCORE: {GameManager.Instance.CurrentScore}";
            finalScoreText.color = scoreColor;
            finalScoreText.fontSize = Screen.height * 0.05f;
            finalScoreText.alignment = TextAlignmentOptions.Center;
        }

        // 设置最高分
        if (highScoreText != null)
        {
            highScoreText.text = $"HIGH SCORE: {GameManager.Instance.HighScore}";
            highScoreText.color = scoreColor;
            highScoreText.fontSize = Screen.height * 0.04f;
            highScoreText.alignment = TextAlignmentOptions.Center;
        }

        // 显示新纪录提示
        if (newRecordText != null)
        {
            newRecordText.gameObject.SetActive(isNewRecord);
            if (isNewRecord)
            {
                newRecordText.text = "NEW RECORD!";
                newRecordText.color = newRecordColor;
                newRecordText.fontSize = Screen.height * 0.04f;
                newRecordText.alignment = TextAlignmentOptions.Center;
            }
        }

        Debug.Log($"GameOverUI: 显示游戏结束界面，得分={GameManager.Instance.CurrentScore}，新纪录={isNewRecord}");
    }

    /// <summary>
    /// 隐藏游戏结束界面
    /// </summary>
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 重新开始按钮点击事件
    /// </summary>
    private void OnRestartButtonClick()
    {
        RestartGame();
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void RestartGame()
    {
        Debug.Log("GameOverUI: 重新开始游戏");
        HideGameOver();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClick);
        }
    }
}
