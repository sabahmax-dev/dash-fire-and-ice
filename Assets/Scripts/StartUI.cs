using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏开始界面控制器
/// 显示游戏标题、最高分，提供开始游戏按钮
/// </summary>
public class StartUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private TextMeshProUGUI gameTitleText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button startButton;

    [Header("样式设置")]
    [SerializeField] private Color titleColor = new Color(0f, 1f, 1f); // 冰蓝色
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color instructionColor = new Color(0.8f, 0.8f, 0.8f);

    private bool gameStarted = false;

    private void Awake()
    {
        // 自动获取UI组件（如果未手动赋值）
        if (startPanel != null)
        {
            gameTitleText = startPanel.transform.Find("GameTitle")?.GetComponent<TextMeshProUGUI>();
            highScoreText = startPanel.transform.Find("HighScore")?.GetComponent<TextMeshProUGUI>();
            instructionText = startPanel.transform.Find("Instruction")?.GetComponent<TextMeshProUGUI>();
            startButton = startPanel.transform.Find("StartButton")?.GetComponent<Button>();
        }

        // 设置开始按钮事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        // 显示开始界面
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        UpdateUI();

        Debug.Log("StartUI: 初始化完成");
    }

    private void Update()
    {
        // 更新最高分显示
        UpdateHighScore();

        // 游戏未开始时，点击或按空格可以开始
        if (!gameStarted && startPanel != null && startPanel.activeSelf)
        {
            bool spacePressed = Input.GetKeyDown(KeyCode.Space);
            bool tapPressed = Input.GetMouseButtonDown(0);

            if (spacePressed || tapPressed)
            {
                StartGame();
            }
        }
    }

    private void UpdateUI()
    {
        // 设置游戏标题
        if (gameTitleText != null)
        {
            gameTitleText.text = "DASH: FIRE & ICE";
            gameTitleText.color = titleColor;
            gameTitleText.fontSize = Screen.height * 0.1f;
            gameTitleText.alignment = TextAlignmentOptions.Center;
        }

        // 设置操作说明
        if (instructionText != null)
        {
            instructionText.text = "Tap screen or press Space to jump\nClick Start to begin";
            instructionText.color = instructionColor;
            instructionText.fontSize = Screen.height * 0.04f;
            instructionText.alignment = TextAlignmentOptions.Center;
        }
    }

    private void UpdateHighScore()
    {
        if (GameManager.Instance == null) return;

        if (highScoreText != null)
        {
            highScoreText.text = $"HIGH SCORE: {GameManager.Instance.HighScore}";
            highScoreText.color = textColor;
            highScoreText.fontSize = Screen.height * 0.05f;
            highScoreText.alignment = TextAlignmentOptions.Center;
        }
    }

    /// <summary>
    /// 开始按钮点击事件
    /// </summary>
    private void OnStartButtonClick()
    {
        StartGame();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;

        // 隐藏开始界面
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // 启动游戏
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }

        Debug.Log("StartUI: 游戏开始");
    }

    /// <summary>
    /// 显示开始界面（重新开始时调用）
    /// </summary>
    public void ShowStartScreen()
    {
        gameStarted = false;
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }
        UpdateHighScore();
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClick);
        }
    }
}
