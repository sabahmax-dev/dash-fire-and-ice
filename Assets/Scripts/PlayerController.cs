using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 冰人主角控制器
/// 处理触屏跳跃、动画状态切换
/// </summary>
public class PlayerController : MonoBehaviour
{
    // 跳跃初速度
    [SerializeField] private float jumpSpeed = 7f;
    // 主角刚体组件
    private Rigidbody2D rb;
    // 动画控制器（可选，支持两种方式：PlayerAnimation 或 Animator）
    private PlayerAnimation playerAnim;
    private Animator anim;
    // 是否在地面（防止二次跳跃）
    private bool isGrounded = true;
    // 是否死亡
    private bool isDead = false;
    // 输入动作
    private PlayerInputActions inputActions;

    private void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        TryGetComponent(out playerAnim); // PlayerAnimation 是可选的
        TryGetComponent(out anim);      // Animator 是可选的（兼容旧版）

        // 初始化输入动作
        inputActions = new PlayerInputActions();
        inputActions.Gameplay.Tap.performed += OnTapPerformed;

        // 设置主角初始位置（屏幕左侧）
        // 地面Y=-3，主角站在地面上需要根据sprite高度调整位置
        // 假设sprite高度约1单位，pivot在中心，则位置应为-3+0.5=-2.5
        transform.position = new Vector3(-2f, -2.5f, 0f);

        // 初始化刚体参数
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        Debug.Log("PlayerController: 主角初始化完成");
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // 调试：每秒输出一次状态
        if (Time.frameCount % 60 == 0)
        {
            // Debug.Log($"Update运行中: isGrounded={isGrounded}, position={transform.position}");
        }

        // 更新动画 - 使用 PlayerAnimation
        if (playerAnim != null)
        {
            playerAnim.SetRunning(isGrounded && !isDead);
            playerAnim.SetJumping(!isGrounded && !isDead);
        }

        // 兼容旧版 Animator
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            bool animIsDead = anim.GetBool("isDead");
            anim.SetBool("isRunning", isGrounded && !animIsDead);
            anim.SetBool("isJumping", !isGrounded && !animIsDead);
        }

        // 备选输入检测（旧输入系统）- 增强调试
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool mousePressed = Input.GetMouseButtonDown(0);

        if (spacePressed || mousePressed)
        {
            Debug.Log($"检测到输入! 空格={spacePressed}, 鼠标={mousePressed}, isGrounded={isGrounded}");
            if (isGrounded && !isDead)
            {
                Jump();
            }
        }
    }

    /// <summary>
    /// 触屏点击回调
    /// </summary>
    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("新输入系统：收到跳跃输入，isGrounded=" + isGrounded + ", isDead=" + isDead);
        if (isGrounded && !isDead)
        {
            Jump();
        }
    }

    /// <summary>
    /// 跳跃逻辑
    /// </summary>
    private void Jump()
    {
        isGrounded = false;
        // 重置垂直速度，添加跳跃力
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        Debug.Log("跳跃！");
    }

    /// <summary>
    /// 碰撞检测（落地判断）
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"碰撞检测: 碰到对象={collision.gameObject.name}, Tag={collision.gameObject.tag}");

        // 检查碰撞对象是否是地面
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("落地: isGrounded = true");
        }

        // 碰撞障碍触发死亡
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("碰到障碍物，游戏结束！");
            isDead = true;

            // 使用 PlayerAnimation 设置死亡状态
            if (playerAnim != null)
            {
                playerAnim.SetDead(true);
            }

            // 兼容旧版 Animator
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                anim.SetBool("isDead", true);
            }

            GameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// 重置主角状态（游戏重开时调用）
    /// </summary>
    public void ResetPlayer()
    {
        isDead = false;
        isGrounded = true;

        // 重置位置和刚体
        transform.position = new Vector3(-2f, -2.5f, 0f);
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;

        // 重置动画
        if (playerAnim != null)
        {
            playerAnim.ResetAnimation();
        }

        // 兼容旧版 Animator
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.SetBool("isDead", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("isJumping", false);
        }

        Debug.Log("PlayerController: 主角已重置");
    }
}
