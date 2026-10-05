using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    public GameObject hitParticlePrefab; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
     
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            if (hitParticlePrefab != null)
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                Instantiate(hitParticlePrefab, contactPoint, Quaternion.identity);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            FindFirstObjectByType<GameManager>()?.GameOver();
           
        }
    }
}