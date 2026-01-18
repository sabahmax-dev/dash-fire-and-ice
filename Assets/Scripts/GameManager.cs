using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏管理器
/// 单例模式，管理游戏状态（运行/结束）、游戏结束逻辑、分数系统、难度递增
/// </summary>
public class GameManager : MonoBehaviour
{
    // 单例实例
    public static GameManager Instance { get; private set; }
    // 游戏状态
    public bool IsGameOver { get; private set; } = false;
    // 游戏结束文本（拖拽赋值，已废弃，使用GameOverUI替代）
    [SerializeField] private Text gameOverText;
    // 游戏结束UI（拖拽赋值）
    [SerializeField] private GameOverUI gameOverUI;
    // 当前分数
    public int CurrentScore { get; private set; } = 0;
    // 最高分
    public int HighScore { get; private set; } = 0;
    // 当前游戏速度（用于难度递增）
    public float GameSpeed { get; private set; } = 5f;
    // 基础速度
    private const float BASE_SPEED = 5f;
    // 最大速度
    private const float MAX_SPEED = 12f;
    // 分数增长速度（每秒增加的分数）
    private const float SCORE_PER_SECOND = 10f;
    // 难度递增阈值（每多少分增加一次速度）
    private const int DIFFICULTY_STEP = 100;
    // 速度增长量
    private const float SPEED_INCREMENT = 0.5f;

    private float scoreTimer = 0f;

    private void Awake()
    {
        Debug.Log("GameManager.Awake() 被调用");

        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager 单例已创建");
        }

        // 加载最高分
        LoadHighScore();

        // 初始化游戏结束文本（隐藏）
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            // 适配文本尺寸和位置
            gameOverText.fontSize = (int)(Screen.height * 0.05f);
            gameOverText.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void Update()
    {
        // 游戏进行中时更新分数
        if (!IsGameOver)
        {
            scoreTimer += Time.deltaTime;
            // 每秒增加分数
            if (scoreTimer >= 1f)
            {
                AddScore(Mathf.FloorToInt(SCORE_PER_SECOND * GameSpeed / BASE_SPEED));
                scoreTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 游戏结束逻辑
    /// </summary>
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Debug.Log("游戏结束");

        // 保存最高分
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            SaveHighScore();
            Debug.Log($"新最高分: {HighScore}");
        }

        // 显示游戏结束UI
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }

        // 兼容旧版：显示游戏结束文本
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        // 停止背景、地面、障碍移动
        StopAllMovingObjects();
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("重新开始游戏");

        // 重置游戏状态
        IsGameOver = false;
        CurrentScore = 0;
        GameSpeed = BASE_SPEED;
        scoreTimer = 0f;

        // 隐藏游戏结束UI
        if (gameOverUI != null)
        {
            gameOverUI.HideGameOver();
        }

        // 隐藏游戏结束文本（兼容旧版）
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        // 重置所有移动对象
        ResetAllMovingObjects();
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartGame()
    {
        Debug.Log("开始游戏");
        RestartGame();
    }

    /// <summary>
    /// 增加分数
    /// </summary>
    /// <param name="points">增加的分数</param>
    public void AddScore(int points)
    {
        CurrentScore += points;
        CheckDifficultyIncrease();
        Debug.Log($"分数: {CurrentScore}");
    }

    /// <summary>
    /// 检查是否需要增加难度
    /// </summary>
    private void CheckDifficultyIncrease()
    {
        // 每达到一定分数，增加游戏速度
        int speedLevel = (CurrentScore / DIFFICULTY_STEP);
        float newSpeed = BASE_SPEED + speedLevel * SPEED_INCREMENT;
        GameSpeed = Mathf.Min(newSpeed, MAX_SPEED);
    }

    /// <summary>
    /// 保存最高分
    /// </summary>
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载最高分
    /// </summary>
    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log($"加载最高分: {HighScore}");
    }

    /// <summary>
    /// 停止所有移动对象（背景、地面、障碍）
    /// </summary>
    private void StopAllMovingObjects()
    {
        // 停止背景滚动
        BackgroundScroll[] bgScrolls = FindObjectsOfType<BackgroundScroll>();
        foreach (var scroll in bgScrolls)
        {
            scroll.enabled = false;
        }

        // 停止地面生成/移动
        GroundManager[] groundManagers = FindObjectsOfType<GroundManager>();
        foreach (var manager in groundManagers)
        {
            manager.enabled = false;
        }

        // 停止障碍移动/生成
        ObstacleManager[] obstacleManagers = FindObjectsOfType<ObstacleManager>();
        foreach (var manager in obstacleManagers)
        {
            manager.enabled = false;
        }
        ObstacleMove[] obstacleMoves = FindObjectsOfType<ObstacleMove>();
        foreach (var move in obstacleMoves)
        {
            move.enabled = false;
        }

        // 禁用主角输入
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach (var controller in playerControllers)
        {
            controller.enabled = false;
        }
    }

    /// <summary>
    /// 重置所有移动对象
    /// </summary>
    private void ResetAllMovingObjects()
    {
        // 重置主角
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach (var controller in playerControllers)
        {
            controller.enabled = true;
            controller.ResetPlayer();
        }

        // 重置背景滚动
        BackgroundScroll[] bgScrolls = FindObjectsOfType<BackgroundScroll>();
        foreach (var scroll in bgScrolls)
        {
            scroll.enabled = true;
        }

        // 重置地面生成/移动
        GroundManager[] groundManagers = FindObjectsOfType<GroundManager>();
        foreach (var manager in groundManagers)
        {
            manager.enabled = true;
            manager.ResetGround();
        }

        // 清除现有障碍物
        ClearAllObstacles();

        // 重置障碍生成
        ObstacleManager[] obstacleManagers = FindObjectsOfType<ObstacleManager>();
        foreach (var manager in obstacleManagers)
        {
            manager.enabled = true;
            manager.ResetSpawner();
        }
    }

    /// <summary>
    /// 清除所有障碍物
    /// </summary>
    private void ClearAllObstacles()
    {
        ObstacleMove[] obstacleMoves = FindObjectsOfType<ObstacleMove>();
        ObstaclePool pool = FindObjectOfType<ObstaclePool>();
        foreach (var move in obstacleMoves)
        {
            if (pool != null)
            {
                pool.ReturnObstacleToPool(move.gameObject);
            }
            else
            {
                Destroy(move.gameObject);
            }
        }
    }
}
