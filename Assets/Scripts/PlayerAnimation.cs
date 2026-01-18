using UnityEngine;

/// <summary>
/// 主角动画控制器
/// 使用 Sprite 切换实现简单的跑步、跳跃、死亡动画
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [Header("Sprite 资源")]
    [SerializeField] private Sprite runSprite1;    // 跑步帧1 (Person_1.png)
    [SerializeField] private Sprite runSprite2;    // 跑步帧2 (Person_2.png)
    [SerializeField] private Sprite jumpSprite;   // 跳跃帧（可选，不设置则用跑步帧1）
    [SerializeField] private Sprite deadSprite;    // 死亡帧（可选）

    [Header("动画参数")]
    [SerializeField] private float runSpeed = 8f;     // 跑步动画切换速度（帧/秒）
    [SerializeField] private float jumpScale = 1.1f;   // 跳跃时缩放比例

    private SpriteRenderer spriteRenderer;
    private float runTimer = 0f;
    private int currentRunFrame = 1;
    private bool isRunning = true;
    private bool isJumping = false;
    private bool isDead = false;

    // 保存用户设置的原始 Scale
    private Vector3 originalScale;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerAnimation: 未找到 SpriteRenderer 组件！");
        }
        // 保存用户设置的 Scale
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isDead)
        {
            UpdateDeadAnimation();
        }
        else if (isJumping)
        {
            UpdateJumpAnimation();
        }
        else if (isRunning)
        {
            UpdateRunAnimation();
        }
    }

    /// <summary>
    /// 更新跑步动画
    /// </summary>
    private void UpdateRunAnimation()
    {
        runTimer += Time.deltaTime;

        // 切换跑步帧
        if (runTimer >= 1f / runSpeed)
        {
            currentRunFrame = currentRunFrame == 1 ? 2 : 1;
            runTimer = 0f;

            if (runSprite1 != null && runSprite2 != null)
            {
                spriteRenderer.sprite = currentRunFrame == 1 ? runSprite1 : runSprite2;
            }
        }

        // 恢复用户设置的 Scale
        transform.localScale = originalScale;
    }

    /// <summary>
    /// 更新跳跃动画
    /// </summary>
    private void UpdateJumpAnimation()
    {
        // 跳跃时使用跳跃帧或跑步帧1
        Sprite jumpSpriteToUse = jumpSprite != null ? jumpSprite : runSprite1;
        if (jumpSpriteToUse != null && spriteRenderer.sprite != jumpSpriteToUse)
        {
            spriteRenderer.sprite = jumpSpriteToUse;
        }

        // 跳跃时在原始 Scale 基础上放大
        transform.localScale = originalScale * jumpScale;
    }

    /// <summary>
    /// 更新死亡动画
    /// </summary>
    private void UpdateDeadAnimation()
    {
        // 死亡时使用死亡帧或保持当前帧
        if (deadSprite != null && spriteRenderer.sprite != deadSprite)
        {
            spriteRenderer.sprite = deadSprite;
        }

        // 死亡时倒下（旋转）
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }

    #region 公共方法

    /// <summary>
    /// 设置跑步状态
    /// </summary>
    public void SetRunning(bool running)
    {
        isRunning = running;
        if (running && !isJumping && !isDead)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 设置跳跃状态
    /// </summary>
    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
        if (jumping && !isDead)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 设置死亡状态
    /// </summary>
    public void SetDead(bool dead)
    {
        isDead = dead;
        if (!dead)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 重置动画状态
    /// </summary>
    public void ResetAnimation()
    {
        isRunning = true;
        isJumping = false;
        isDead = false;
        currentRunFrame = 1;
        runTimer = 0f;
        transform.rotation = Quaternion.identity;
        transform.localScale = originalScale;

        if (runSprite1 != null)
        {
            spriteRenderer.sprite = runSprite1;
        }
    }

    #endregion
}
