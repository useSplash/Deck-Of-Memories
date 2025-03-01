using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Unity.VisualScripting;

public class DeckHandler : MonoBehaviour
{
    [Header("Deck")]
    public List<GameObject> deck = new List<GameObject>();
    public List<GameObject> pairedCards = new List<GameObject>();
    public List<GameObject> dealtCards = new List<GameObject>();
    GameObject hoveredCard;
    public GameObject holder;

    [Header("Deal Display")]
    public int numberOfColumns;
    public int numberOfRows;
    public float paddingX;
    public float paddingY;
    
    GameObject selectedCard1 = null;
    GameObject selectedCard2 = null;

    [HideInInspector]
    public bool isComplete = false;
    public bool areCardsDealt = false;
    public UnityEvent matched;
    public UnityEvent mistake;

    int numberOfPairsMatched = 0;

    private Vector2 upperLeft = new Vector2(-4, 3);
    private Vector2 lowerRight = new Vector2(5, -3);


    void Start()
    {
        ShuffleDeck();
    }

    void Update()
    {
        if (!areCardsDealt) {return;}

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates
        foreach (GameObject card in dealtCards)
        {
            Collider2D col = card.GetComponent<Collider2D>(); // Get Collider2D
            CardBehavior cardBehavior = col.gameObject.GetComponent<CardBehavior>();

            if (col != null 
                && col.OverlapPoint(mousePosition)) // Check if mouse is over collider
            {
                if (selectedCard1 != null && selectedCard2 != null) {return;}

                if (hoveredCard != col.gameObject) 
                {
                    hoveredCard = col.gameObject;
                    AudioManager.instance.Play("CardRiffle");
                }

                ScaleObject(cardBehavior.spriteRenderer.transform, 1.2f);

                if (Input.GetMouseButtonDown(0) 
                    && !cardBehavior.faceUp 
                    && !cardBehavior.isFlipping)
                {
                    if (selectedCard1 == null)
                    {
                        selectedCard1 = col.gameObject;
                        StartCoroutine(FlipCardUp(selectedCard1));
                    }
                    else
                    {
                        selectedCard2 = col.gameObject;
                        StartCoroutine(FlipCardUp(selectedCard2));
                        StartCoroutine(CompareSelectedCards(selectedCard1, selectedCard2));
                    }
                }
            }
            else
            {
                if (col.gameObject != selectedCard1 &&
                    col.gameObject != selectedCard2)
                    {
                        ScaleObject(cardBehavior.spriteRenderer.transform, 1.0f);
                    }
            }
        }
    }

    public void PlayDealCards()
    {
        StartCoroutine(DealCards());
    }

    public IEnumerator DealCards(float buffer = 0.15f)
    {
        
        // Calculate the grid center
        Vector2 center = (upperLeft + lowerRight) / 2;

        // Start placing objects
        for (int i = 0; i < deck.Count; i++)
        {
            AudioManager.instance.Play("CardDeal");

            int row = i / numberOfColumns;
            int col = i % numberOfColumns;

            float rowMidPoint = numberOfRows / 2;
            float colMidPoint = numberOfColumns / 2;

            Vector3 newPosition = new Vector3(center.x + (col - colMidPoint) * paddingX, 
                                    center.y + (row - rowMidPoint) * paddingY, 
                                    0);

            StartCoroutine(MoveCard(deck[i], newPosition));
            dealtCards.Add(deck[i]);
            yield return new WaitForSeconds(buffer);
            deck[i].GetComponent<CardBehavior>().inPlay = true;
        }

        yield return new WaitForSeconds(0.15f);
        areCardsDealt = true;
    }

    public IEnumerator ReturnAllCards(float buffer = 0.0f)
    {
        foreach (GameObject card in deck)
        {
            StartCoroutine(MoveCard(card, holder.transform.position));
            yield return new WaitForSeconds(buffer);
        }
    }

    public IEnumerator FlipDownAllCards(float buffer = 0.1f)
    {
        foreach (GameObject card in deck)
        {
            StartCoroutine(FlipCardDown(card));
            yield return new WaitForSeconds(buffer);
        }
    }

    public IEnumerator FlipCardUp(GameObject card)
    {
        AudioManager.instance.SetVolume("CardFlip", 0.5f);
        AudioManager.instance.Play("CardFlip");

        CardBehavior behavior = card.GetComponent<CardBehavior>();
        if (!behavior.faceUp)
        {
            StartCoroutine(behavior.FlipCard(1));
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator FlipCardDown(GameObject card)
    {
        AudioManager.instance.SetVolume("CardFlip", 0.2f);
        AudioManager.instance.Play("CardFlip");

        CardBehavior behavior = card.GetComponent<CardBehavior>();
        if (behavior.faceUp)
        {
            StartCoroutine(behavior.FlipCard(1));
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator MoveCard(GameObject card, Vector3 target, float duration = 0.5f)
    {
        Vector3 startPos = card.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth the movement

            card.transform.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }

        card.transform.position = target; // Ensure it reaches exact position
    }

    public IEnumerator CompareSelectedCards(GameObject card1, GameObject card2)
    {
        Debug.Log("Comparing");
        yield return new WaitForSeconds(1.5f);
        
        if (card1.GetComponent<CardBehavior>().type == card2.GetComponent<CardBehavior>().type)
        {
            Debug.Log("CORRECT");
            matched?.Invoke();
            numberOfPairsMatched += 1;
            
            card1.GetComponent<CardBehavior>().inPlay = false;
            card2.GetComponent<CardBehavior>().inPlay = false;

            StartCoroutine(FlipCardDown(card1));
            StartCoroutine(FlipCardDown(card2));

            StartCoroutine(MoveCard(card1, holder.transform.position));
            StartCoroutine(MoveCard(card2, holder.transform.position));
            
            Debug.Log("Reset Selection");

            StartCoroutine(CheckComplete());
        }
        else
        {
            Debug.Log("WRONG");
            mistake?.Invoke();

            StartCoroutine(FlipCardDown(card1));
            StartCoroutine(FlipCardDown(card2));
        }

        selectedCard1 = null;
        selectedCard2 = null;
    }

    public IEnumerator CheckComplete()
    {
        if (numberOfPairsMatched == deck.Count/2)
        {
            yield return new WaitForSeconds(0.5f);
            isComplete = true;
            Debug.Log("Complete!");
        }
    }

    public void ShuffleDeck()
    {
        deck = deck.OrderBy(x => Random.value).ToList();
    }

    public void ScaleObject(Transform trans, float newScale)
    {
        var scale = trans.localScale;
        scale.x = Mathf.Lerp(scale.x, newScale, 0.5f);
        scale.y = Mathf.Lerp(scale.y, newScale, 0.5f);
        scale.z = Mathf.Lerp(scale.z, newScale, 0.5f);
        trans.localScale = scale;
    }
}
