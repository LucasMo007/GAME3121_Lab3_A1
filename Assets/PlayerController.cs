//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float moveSpeed = 8f;
//    private Rigidbody2D rb;
//    public GameObject explosionPrefab;

//    [Header("Audio Clips")]
//    public AudioClip hitWallClip;      // 撞墙音效
//    public AudioClip explosionClip;    // 死亡爆炸音效
//    public AudioSource audioSource;    // 挂在飞船上的 AudioSource

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//    }

//    void Update()
//    {
//        float moveX = Input.GetAxisRaw("Horizontal");
//        float moveY = Input.GetAxisRaw("Vertical");
//        Vector2 movement = new Vector2(moveX, moveY).normalized;
//        rb.linearVelocity = movement * moveSpeed;


//        if (movement != Vector2.zero)
//        {
//            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
//            transform.rotation = Quaternion.Euler(0, 0, angle);
//        }
//    }
//    //void OnCollisionEnter2D(Collision2D collision)
//    //{

//    //    //if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Wall"))
//    //    //{

//    //    //    if (explosionPrefab != null)
//    //    //    {
//    //    //        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
//    //    //    }


//    //    //    FindFirstObjectByType<GameManager>()?.GameOver();

//    //    //    Destroy(gameObject);
//    //    //}
//    //}
//    void OnCollisionEnter2D(Collision2D collision)
//    {
//        // 1. 飞船撞到墙壁：播放撞墙声
//        if (collision.gameObject.CompareTag("Wall"))
//        {
//            if (audioSource != null && hitWallClip != null)
//            {
//                audioSource.PlayOneShot(hitWallClip);
//            }
//        }
//        // 2. 飞船撞到障碍物：飞船销毁、播放爆炸声、触发游戏结束
//        else if (collision.gameObject.CompareTag("Obstacle"))
//        {
//            // 注意：因为飞船马上要 Destroy，挂在身上的 AudioSource 会中断
//            // 所以使用 PlayClipAtPoint 在飞船当前坐标生成一个临时音效播放器
//            if (explosionClip != null)
//            {
//                AudioSource.PlayClipAtPoint(explosionClip, transform.position);
//            }

//            // 触发你原有的粒子和 GameManager.GameOver() 逻辑
//            // Destroy(gameObject);
//        }
//    }
//}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    private Rigidbody2D rb;

    [Header("Visual Effects")]
    public GameObject explosionPrefab;

    [Header("Audio Clips")]
    public AudioClip hitWallClip;       // 撞墙音效 (Part 2: 1 pt)
    public AudioClip explosionClip;     // 销毁爆炸音效 (Part 2: 1 pt)
    public AudioSource audioSource;

    void Awake()
    {
        // 自动获取或添加 AudioSource 组件
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 移动控制
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = movement * moveSpeed;

        // 旋转朝向移动方向
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 撞到边界墙：播放撞墙声 (飞船存活)
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (audioSource != null && hitWallClip != null)
            {
                audioSource.PlayOneShot(hitWallClip);
            }
        }
        // 2. 撞到敌人/障碍物：播放爆炸声、生成爆炸粒子、触发 GameOver 并销毁飞船
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 因飞船即将 Destroy，身上组件会中断，所以用 PlayClipAtPoint 在当前坐标独立播放爆炸声
            if (explosionClip != null)
            {
                AudioSource.PlayClipAtPoint(explosionClip, transform.position);
            }

            // 生成爆炸粒子特效
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // 触发游戏结束面板与结算
            FindFirstObjectByType<GameManager>()?.GameOver();

            // 销毁飞船
            Destroy(gameObject);
        }
    }
}