using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;

    private float currentScore = 0f;
    private bool isGameOver = false;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            currentScore += Time.deltaTime * 10f;
            if (scoreText != null)
            {
                scoreText.text = "Score: " + Mathf.FloorToInt(currentScore);
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        int finalScore = Mathf.FloorToInt(currentScore);

        if (finalScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
            PlayerPrefs.Save();
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + finalScore;
            }
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
