using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public GameObject background;
    public int numberOfCopies;
    public float scrollSpeed;
    float imageWidth;
    List<GameObject> backgroundImages = new List<GameObject>();

    void Start()
    {
        if (background != null)
        {
            imageWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
            background.transform.localPosition = new Vector3(-imageWidth, 0, 0);
            backgroundImages.Add(background);
            for (int i = 0; i < numberOfCopies; i++) 
            {
                GameObject copy = Instantiate(background, transform);
                copy.transform.localPosition = new Vector3(imageWidth * i, 0, 0);  // Offset each copy
                backgroundImages.Add(copy);  // Store in list
            }
        }
    }

    void Update()
    {
        foreach (GameObject image in backgroundImages)
        {
            image.transform.localPosition += new Vector3(-scrollSpeed * Time.deltaTime, 0, 0);
            if (image.transform.localPosition.x < -imageWidth)
            {
                image.transform.localPosition = new Vector3(imageWidth * numberOfCopies, 
                                                            image.transform.localPosition.y, 
                                                            image.transform.localPosition.z);
            }
        }
    }
}
