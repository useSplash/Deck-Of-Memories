using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Scoreboard : MonoBehaviour
{
    int score = 0;
    public TextMeshProUGUI scoreText;
    private float scoreIncreaseDelay = 0.01f;
    public UnityEvent doneCounting;

    public void ShowFinalScore()
    {
        StartCoroutine(IncreaseScoreGradually());
    }

    private IEnumerator IncreaseScoreGradually()
    {
        int targetScore = GameValues.score;
        AudioManager.instance.Play("PointsUp");

        while (score < targetScore)
        {
            score += Mathf.Max(Mathf.Min((targetScore-score)/5, 5), 1);
            UpdateScoreText();
            yield return new WaitForSeconds(scoreIncreaseDelay);
        }

        score = targetScore;
        AudioManager.instance.Stop("PointsUp");

        doneCounting?.Invoke();
    }
    
    private void UpdateScoreText()
    {
        scoreText.text = "SCORE: " + $"{score:D4}";
    }
}
