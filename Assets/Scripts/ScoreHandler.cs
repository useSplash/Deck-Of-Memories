using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class ScoreHandler : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image scoreIcon;
    public int score = 0;
    private int targetScore = 0;
    private float scoreIncreaseDelay = 0.01f;
    
    void Start()
    {
        score = GameValues.score;
        targetScore = score;
        UpdateScoreText();
    }

    void Update()
    {
        if(score != targetScore)
        {
            scoreIcon.transform.localScale = Vector3.one * (1.1f + (math.sin(Time.time * 50) * 0.1f));
        }
        else
        {
            scoreIcon.transform.localScale = Vector3.one;
        }
    }

    public void AddScore(int amount)
    {
        score = targetScore;
        targetScore += amount;
        StartCoroutine(IncreaseScoreGradually());
    }

    public void SubtractScore(int amount)
    {
        score = targetScore;
        // Keep it above 0
        targetScore = math.max(targetScore-amount, 0);
        StartCoroutine(DecreaseScoreGradually());
    }

    private IEnumerator IncreaseScoreGradually()
    {
        AudioManager.instance.Play("PointsUp");
        while (score < targetScore)
        {

            score += math.max(math.min((targetScore-score)/5, 5), 1);
            UpdateScoreText();
            yield return new WaitForSeconds(scoreIncreaseDelay);
        }
        score = targetScore;
        AudioManager.instance.Stop("PointsUp");
    }

    private IEnumerator DecreaseScoreGradually()
    {
        while (score > targetScore)
        {
            score -= math.max(math.min((targetScore-score)/5, 5), 1);
            UpdateScoreText();
            yield return new WaitForSeconds(scoreIncreaseDelay);
        }
        score = targetScore;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "X " + $"{score:D2}";
    }

    public bool DoneAdding()
    {
        return score == targetScore;
    }
}
