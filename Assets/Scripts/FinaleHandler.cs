using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class FinaleHandler : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    private List<GameObject> clickedCards = new List<GameObject>();
    GameObject hoveredCard;
    public TextMeshProUGUI finalMessage;
    public bool cardsDealt = false;
    public Transform mask;
    public UnityEvent ending;

    void Update()
    {
        if (!cardsDealt) {return;}

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
        foreach (GameObject card in cards)
        {

            Collider2D col = card.GetComponent<Collider2D>(); // Get Collider2D
            SpecialCardBehavior specialCardBehavior = card.GetComponent<SpecialCardBehavior>();

            if (col != null 
                && col.OverlapPoint(mousePosition)) // Check if mouse is over collider
            {
                
                ScaleObject(card.transform, 90.0f * 1.2f, 0.1f);
                if (hoveredCard != card) 
                {
                    hoveredCard = card;
                    AudioManager.instance.Play("CardRiffle");
                }

                if (clickedCards.Contains(card)) {return;}

                if (Input.GetMouseButtonDown(0))
                {
                    clickedCards.Add(card);
                    specialCardBehavior.onClick?.Invoke();
                    StartCoroutine(specialCardBehavior.FlipCard(2.0f));
                    AudioManager.instance.Play("Mystical");
                    StartCoroutine(TextFadeIn(specialCardBehavior.message));

                    switch (clickedCards.Count)
                    {
                        case 1:
                        AudioManager.instance.SetVolume("AtingDalawa", 0.2f);
                        AudioManager.instance.Play("AtingDalawa");

                        AudioManager.instance.SetVolume("CardRiffle", 0.2f);
                        AudioManager.instance.SetVolume("Mystical", 0.2f);

                        break;

                        case 2:
                        AudioManager.instance.SetVolume("AtingDalawa", 0.4f);

                        break;

                        case 3:
                        AudioManager.instance.SetVolume("AtingDalawa", 0.6f);
                        break;

                        case 4:
                        AudioManager.instance.SetVolume("AtingDalawa", 0.8f);
                        break;

                        case 5:
                        AudioManager.instance.SetVolume("AtingDalawa", 1.0f);
                        StartCoroutine(Ending());
                        break;

                        default:
                        break;
                    }
                }
            }
            else
            {
                if (hoveredCard == card) 
                {
                    hoveredCard = null;
                }
                ScaleObject(card.transform, 90.0f, 0.1f);
            }
        }
    }

    public void ScaleObject(Transform trans, float newScale, float acceleration)
    {
        var scale = trans.localScale;
        scale.x = Mathf.Lerp(scale.x, newScale, acceleration);
        scale.y = Mathf.Lerp(scale.y, newScale, acceleration);
        scale.z = Mathf.Lerp(scale.z, newScale, acceleration);
        trans.localScale = scale;
    }

    public IEnumerator Ending()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(TextFadeIn(finalMessage));

        yield return new WaitForSeconds(3.0f);
        float targetScale = 1500;

        while (mask.localScale.x < targetScale * 0.9)
        {
            ScaleObject(mask, targetScale, 0.01f);
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(3.0f);
        ending?.Invoke();
    } 

    public IEnumerator TextFadeIn(TextMeshProUGUI text)
    {
        float opacity = 0;
        text.color = new Color(1, 1, 1, 0);
        while (opacity < 1)
        {
            opacity += Time.deltaTime / 5;
            text.color = new Color(1, 1, 1, opacity);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetCardsDealtBool()
    {
        cardsDealt = true;
    }

    public void PlayDealSound()
    {
        AudioManager.instance.Play("CardDeal");
    }
}
