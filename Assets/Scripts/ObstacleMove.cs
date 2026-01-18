using UnityEngine;

/// <summary>
/// 障碍移动脚本
/// 控制障碍向左移动，移出屏幕后回收
/// 速度随游戏难度动态调整
/// </summary>
public class ObstacleMove : MonoBehaviour
{
    // 障碍对象池
    private ObstaclePool obstaclePool;
    // 回收阈值（屏幕左侧外）
    private float recycleX;

    private void Awake()
    {
        // 计算屏幕左边界（相机可视范围外）
        float cameraWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        recycleX = -cameraWidth * 0.6f; // 屏幕左侧外一点
    }

    /// <summary>
    /// 初始化障碍移动参数
    /// </summary>
    /// <param name="pool">障碍对象池</param>
    public void Init(ObstaclePool pool)
    {
        obstaclePool = pool;
    }

    private void Update()
    {
        // 游戏结束时停止移动
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // 使用游戏管理器的动态速度
        float currentSpeed = GameManager.Instance != null ? GameManager.Instance.GameSpeed : 5f;

        // 向左移动
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        // 移出屏幕左侧后回收
        if (transform.position.x <= recycleX)
        {
            obstaclePool.ReturnObstacleToPool(gameObject);
        }
    }
}
