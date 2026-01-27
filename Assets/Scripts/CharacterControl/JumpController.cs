using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpController : MonoBehaviour
{
    [Header("跳跃参数")]
    [Tooltip("固定跳跃高度（米）")]
    public float jumpHeight = 3f;

    [Tooltip("跳跃按键")]
    public KeyCode jumpKey = KeyCode.K;

    [Tooltip("最多跳跃次数")]
    public int jumptime = 1;

    [Header("落地检测")]
    public Transform groundCheck;   
    public float groundRadius = 0.1f;
    public LayerMask whatIsGround;  

    private Rigidbody2D rb;
    public bool isGrounded;
    int jumpCount = 0;  

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 落地检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        
        // 落地复位
        if (isGrounded)
            jumpCount = 0;

        // 跳跃输入
        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded || (jumpCount < jumptime-1))
                Jump();
        }
    }

    void Jump()
    {
        jumpCount++;

        float jumpSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);

        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}