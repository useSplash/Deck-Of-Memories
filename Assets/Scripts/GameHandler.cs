using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Unity.Mathematics;

public class GameHandler : MonoBehaviour
{
    [Header("Objects")]
    public DeckHandler deckHandler;
    public ScoreHandler scoreHandler;

    [Header("UI Text")]
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI heartsText;

    [Header("End Screen Text")]
    public TextMeshProUGUI endingHighestMultiplierText;
    public TextMeshProUGUI endingHeartBonusText;

    [Header("Events")]
    public UnityEvent complete;
    public UnityEvent end;

    int multiplier = 0;
    int highestMultiplier = 0;
    int heartsRemaining;
    bool isRunning = false;
    public bool isTutorial = false;

    public void Start()
    {
        heartsRemaining = math.max(deckHandler.deck.Count/2 + 1, 1);
        UpdateComboText(); 
        UpdateHeartText();
    }

    public void Update()
    {
        if (deckHandler.isComplete && scoreHandler.DoneAdding() && !isRunning)
        {
            StartCoroutine(Complete());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Complete());
        }
    }

    public void Matched()
    {
        AudioManager.instance.Play("Collect");
        multiplier ++;
        scoreHandler.AddScore(25 * multiplier);
        UpdateComboText();
    }

    public void Mistake()
    {
        multiplier --;
        if (multiplier <= 0)
        {
            multiplier = 0;
        }
        UpdateComboText();


        heartsRemaining -- ;
        if (heartsRemaining < 0)
        {
            heartsRemaining = 0;
        }
        UpdateHeartText();
    }

    public void UpdateComboText()
    {
        multiplierText.text = " Combo X " + $"{multiplier:D2}";  
        if (highestMultiplier < multiplier)
        {
            highestMultiplier = multiplier;
        }
    }

    public void UpdateHeartText()
    {
        heartsText.text = "X " + $"{heartsRemaining:D2}";
    }

    IEnumerator Complete()
    {
        isRunning = true;

        AudioManager.instance.Play("Level Complete");

        if (!isTutorial)
        {
            endingHighestMultiplierText.text = "Highest Combo: ";  
            endingHeartBonusText.text = "Lives Bonus: ";  
            yield return new WaitForSeconds(0.5f);

            complete?.Invoke();

            yield return new WaitForSeconds(1.5f);

            // Add Combo Score
            scoreHandler.AddScore(highestMultiplier * 50);
            endingHighestMultiplierText.text = "Highest Combo: " + highestMultiplier + " X 50";  

            while (!scoreHandler.DoneAdding())
            {
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(1.0f);

            // Add Heart Bonus Score
            scoreHandler.AddScore(heartsRemaining * 150);
            endingHeartBonusText.text = "Lives Bonus: " + heartsRemaining + " X 150";  

            while (!scoreHandler.DoneAdding())
            {
                yield return new WaitForSeconds(0.1f);
            }

            GameValues.score = scoreHandler.score;
        }
        else
        {
            endingHighestMultiplierText.text = "Highest Combo: -";  
            endingHeartBonusText.text = "Lives Bonus: -";  
            yield return new WaitForSeconds(0.5f);

            complete?.Invoke();

            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(3.0f);
        end?.Invoke();
    }
}
