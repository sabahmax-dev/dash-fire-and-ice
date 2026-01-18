using UnityEngine;

/// <summary>
/// 障碍管理器
/// 控制障碍的随机生成、移动、回收
/// 速度随游戏难度动态调整
/// </summary>
public class ObstacleManager : MonoBehaviour
{
    // 障碍对象池（拖拽赋值）
    [SerializeField] private ObstaclePool obstaclePool;
    // 生成间隔（2-4秒）
    [SerializeField] private float minSpawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 4f;
    // 障碍生成位置（主角右侧0.5~0.8屏幕宽度）
    private float spawnXMin;
    private float spawnXMax;
    // 障碍Y轴位置（地面上方）
    private float spawnY;
    // 下一次生成时间
    private float nextSpawnTime;
    // 基础生成间隔
    private const float BASE_MIN_INTERVAL = 2f;
    private const float BASE_MAX_INTERVAL = 4f;

    private void Awake()
    {
        Debug.Log("ObstacleManager.Awake() 开始初始化");

        // 初始化生成位置（简化，使用固定世界坐标）
        float cameraWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        spawnXMin = cameraWidth * 0.5f;
        spawnXMax = cameraWidth * 0.8f;
        spawnY = -2.5f; // 在地面上（地面Y=-3，障碍物高度0.5，所以-3+0.25=-2.75，取-2.5）

        // 初始化下一次生成时间
        nextSpawnTime = Time.time + 2f;

        Debug.Log($"ObstacleManager 初始化完成: spawnXMin={spawnXMin}, spawnXMax={spawnXMax}, spawnY={spawnY}");
    }

    private void Update()
    {
        // 游戏结束时停止生成
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // 根据游戏速度调整生成间隔（速度越快，间隔越短）
        float speedMultiplier = GameManager.Instance != null ? GameManager.Instance.GameSpeed / 5f : 1f;
        minSpawnInterval = BASE_MIN_INTERVAL / speedMultiplier;
        maxSpawnInterval = BASE_MAX_INTERVAL / speedMultiplier;

        // 到时间生成障碍
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            // 重置下一次生成时间
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    /// <summary>
    /// 生成障碍
    /// </summary>
    private void SpawnObstacle()
    {
        if (obstaclePool == null)
        {
            Debug.LogError("ObstaclePool 引用为空！请在 Inspector 中连接 ObstaclePool 对象");
            return;
        }

        GameObject obstacle = obstaclePool.GetObstacleFromPool();
        // 随机生成位置
        float randomX = Random.Range(spawnXMin, spawnXMax);
        obstacle.transform.position = new Vector3(randomX, spawnY, 0f);
        // 设置障碍尺寸（固定大小）
        obstacle.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        // 添加移动脚本（若未添加）
        if (!obstacle.TryGetComponent(out ObstacleMove moveScript))
        {
            moveScript = obstacle.AddComponent<ObstacleMove>();
        }
        moveScript.Init(obstaclePool);
    }

    /// <summary>
    /// 重置生成器（游戏重开时调用）
    /// </summary>
    public void ResetSpawner()
    {
        nextSpawnTime = Time.time + 2f;
        Debug.Log("ObstacleManager: 生成器已重置");
    }
}
