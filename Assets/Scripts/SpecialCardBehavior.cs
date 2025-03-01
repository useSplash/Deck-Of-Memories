using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SpecialCardBehavior : MonoBehaviour
{
    public bool faceUp;
    public RectTransform backRectTransform;
    public RectTransform pictureRectTransform;
    public Transform mask;
    public float maskSize;
    public AnimationCurve cardFlipAnimationCurve;
    public UnityEvent onClick;
    public UnityEvent afterFlip;
    public TextMeshProUGUI message;

    public IEnumerator FlipCard(float duration) 
    {
        float time = 0;
        float delta;
        
        while (time < duration)
        {

            time += Time.deltaTime;
            delta = time / duration;
            
            if (!faceUp) 
            {
                delta = 1 - delta;
            }
            
            float rotationY = cardFlipAnimationCurve.Evaluate(delta) * 180;
            backRectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            pictureRectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);

            // Change image when flipping past 90 degrees
            if (faceUp && cardFlipAnimationCurve.Evaluate(delta) > 0.5)
            {
                backRectTransform.gameObject.SetActive(true);
                pictureRectTransform.gameObject.SetActive(false);
            }
            else if (!faceUp && cardFlipAnimationCurve.Evaluate(delta) < 0.5)
            {
                backRectTransform.gameObject.SetActive(false);
                pictureRectTransform.gameObject.SetActive(true);
            }

            yield return null;
        }

        faceUp = !faceUp;
        StartCoroutine(ScaleMask(mask, maskSize, 0.02f));
        afterFlip?.Invoke();
    }

    public IEnumerator ScaleMask(Transform trans, float newScale, float acceleration)
    {
        while (trans.localScale.x < newScale * 0.95f)
        {
            ScaleObject(trans, newScale, acceleration);
            yield return new WaitForEndOfFrame();
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
}
