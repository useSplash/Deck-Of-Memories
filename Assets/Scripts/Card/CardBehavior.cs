using System.Collections;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{
    public string type;
    public bool faceUp;
    public bool inPlay;
    public Sprite frontSprite;
    public Sprite backSprite;
    public SpriteRenderer spriteRenderer;
    public AnimationCurve cardFlipAnimationCurve;
    Vector3 initialScale;
    
    [HideInInspector]
    public bool isFlipping;

    void Start()
    {
        initialScale = transform.localScale;

        var rotation = spriteRenderer.transform.rotation;
        if (faceUp)
        {
            spriteRenderer.sprite = frontSprite;
            rotation.y = 0;
        }
        else
        {
            spriteRenderer.sprite = backSprite;
            rotation.y = 180;
        }
        spriteRenderer.transform.rotation = rotation;
    }

    public IEnumerator FlipCard(float duration) 
    {
        float time = 0;
        float delta;

        isFlipping = true;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            delta = time / duration;
            
            if (!faceUp) 
            {
                delta = 1 - delta;
            }
            
            spriteRenderer.transform.rotation = Quaternion.Euler(0, cardFlipAnimationCurve.Evaluate(delta) * 180, 0);

            // Change image when spinning
            if (faceUp && cardFlipAnimationCurve.Evaluate(delta) > 0.5)
            {
                spriteRenderer.sprite = backSprite;
            }
            else if (!faceUp && cardFlipAnimationCurve.Evaluate(delta) < 0.5)
            {
                spriteRenderer.sprite = frontSprite;
            }

            yield return null;
        }

        faceUp = !faceUp;
        isFlipping = false;
    }
}
