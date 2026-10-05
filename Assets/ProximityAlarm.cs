using UnityEngine;

public class ProximityAlarm : MonoBehaviour
{
    public AudioClip beepClip;
    public float maxDetectionDistance = 6.0f; // 开始触发报警的距离
    public float minDetectionDistance = 0.8f; // 极端危险距离

    public float minBeepInterval = 0.1f;     // 最快间隔（很急促）
    public float maxBeepInterval = 0.8f;     // 最慢间隔

    public float minPitch = 0.8f;            // 远距离低音调
    public float maxPitch = 2.0f;            // 近距离高尖叫音调

    private AudioSource alarmSource;
    private float beepTimer = 0f;

    void Start()
    {
        alarmSource = gameObject.AddComponent<AudioSource>();
        alarmSource.playOnAwake = false;
    }

    void Update()
    {
        // 查找场景中所有障碍物
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length == 0) return;

        // 计算与最近一个障碍物的距离
        float closestDistance = Mathf.Infinity;
        foreach (GameObject obs in obstacles)
        {
            if (obs != null)
            {
                float dist = Vector2.Distance(transform.position, obs.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                }
            }
        }

        // 如果在警报感应范围内
        if (closestDistance <= maxDetectionDistance)
        {
            // 归一化比例：0 表示最远边缘，1 表示贴脸最近
            float dangerFactor = 1f - Mathf.InverseLerp(minDetectionDistance, maxDetectionDistance, closestDistance);

            // 1. 越近，音调（Pitch）越高 (higher)
            alarmSource.pitch = Mathf.Lerp(minPitch, maxPitch, dangerFactor);

            // 2. 越近，鸣叫频率越快 (more frequent: 间隔时间从 0.8s 缩短到 0.1s)
            float currentInterval = Mathf.Lerp(maxBeepInterval, minBeepInterval, dangerFactor);

            beepTimer += Time.deltaTime;
            if (beepTimer >= currentInterval)
            {
                alarmSource.PlayOneShot(beepClip);
                beepTimer = 0f; // 重置计时
            }
        }
        else
        {
            beepTimer = 0f; // 离开范围不鸣叫
        }
    }
}
