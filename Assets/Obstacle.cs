
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement & Effects")]
    public float speed = 5f;
    public GameObject hitParticlePrefab;
    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioClip obstacleBounceClip; // 障碍物互撞音效
    private AudioSource audioSource;

    void Awake()
    {
        // 自动获取或动态添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 赋予随机方向的初速度
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 碰到 Obstacle 或 Player 时生成粒子特效
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            if (hitParticlePrefab != null)
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                Instantiate(hitParticlePrefab, contactPoint, Quaternion.identity);
            }
        }

        // 2. 只有当两个障碍物互撞时，播放互撞音效 (Assignment 1 要求)
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (audioSource != null && obstacleBounceClip != null)
            {
                audioSource.PlayOneShot(obstacleBounceClip);
            }
        }

        // 3. 碰到 Player 时触发游戏结束
        if (collision.gameObject.CompareTag("Player"))
        {
            FindFirstObjectByType<GameManager>()?.GameOver();
        }
    }
}