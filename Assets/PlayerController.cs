
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    // 在 PlayerController.cs 中：

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
        // 2. 撞到敌人/障碍物：关停警报、播放爆炸声、生成爆炸粒子、触发 GameOver 并销毁飞船
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 关键改动 ①：立即掐断警报脚本，防止临死前爆发出刺耳的近距离哔哔声
            ProximityAlarm alarm = GetComponent<ProximityAlarm>();
            if (alarm != null)
            {
                alarm.enabled = false;
            }

            // 停止当前飞船正在播的任何声音
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            // 关键改动 ②：把声音发出的位置定位到摄像机前方，避免 2D/3D 衰减导致听不清
            if (explosionClip != null)
            {
                Vector3 soundPos = Camera.main != null ? Camera.main.transform.position : transform.position;
                AudioSource.PlayClipAtPoint(explosionClip, soundPos, 1.0f);
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
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // 1. 撞到边界墙：播放撞墙声 (飞船存活)
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        if (audioSource != null && hitWallClip != null)
    //        {
    //            audioSource.PlayOneShot(hitWallClip);
    //        }
    //    }
    //    // 2. 撞到敌人/障碍物：播放爆炸声、生成爆炸粒子、触发 GameOver 并销毁飞船
    //    else if (collision.gameObject.CompareTag("Obstacle"))
    //    {
    //        // 因飞船即将 Destroy，身上组件会中断，所以用 PlayClipAtPoint 在当前坐标独立播放爆炸声
    //        if (explosionClip != null)
    //        {
    //            AudioSource.PlayClipAtPoint(explosionClip, transform.position);
    //        }

    //        // 生成爆炸粒子特效
    //        if (explosionPrefab != null)
    //        {
    //            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    //        }

    //        // 触发游戏结束面板与结算
    //        FindFirstObjectByType<GameManager>()?.GameOver();

    //        // 销毁飞船
    //        Destroy(gameObject);
    //    }
    //}

}