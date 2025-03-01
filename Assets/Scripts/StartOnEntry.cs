using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StartOnEntry : MonoBehaviour
{
    public UnityEvent startOnEntry;

    void Start()
    {
        StartCoroutine(StartDialogue());
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1.0f);
        startOnEntry?.Invoke();
    }
}
    
