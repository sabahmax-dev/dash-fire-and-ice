using UnityEngine;

/// <summary>
/// 背景滚动脚本
/// 实现双背景循环向左滚动，模拟无限跑酷视觉效果
/// 速度随游戏难度动态调整
/// </summary>
public class BackgroundScroll : MonoBehaviour
{
    // 两个背景Sprite（拖拽赋值）
    [SerializeField] private Transform bg1;
    [SerializeField] private Transform bg2;
    // 背景宽度（屏幕宽度）
    private float bgWidth;

    private void Awake()
    {
        // 初始化背景宽度（使用相机可视宽度）
        float cameraHeight = Camera.main.orthographicSize * 2f;
        bgWidth = cameraHeight * Camera.main.aspect;
        // 设置初始位置
        bg1.position = new Vector3(0, bg1.position.y, bg1.position.z);
        bg2.position = new Vector3(bgWidth, bg2.position.y, bg2.position.z);
    }

    private void Update()
    {
        // 游戏结束时停止
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        // 使用游戏管理器的动态速度
        float currentSpeed = GameManager.Instance != null ? GameManager.Instance.GameSpeed : 5f;

        // 背景向左滚动
        bg1.Translate(Vector3.left * currentSpeed * Time.deltaTime);
        bg2.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        // 循环拼接：当背景完全移出左侧屏幕时，重置到右侧
        if (bg1.position.x <= -bgWidth)
        {
            bg1.position = new Vector3(bg2.position.x + bgWidth, bg1.position.y, bg1.position.z);
        }
        if (bg2.position.x <= -bgWidth)
        {
            bg2.position = new Vector3(bg1.position.x + bgWidth, bg2.position.y, bg2.position.z);
        }
    }
}
