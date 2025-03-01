using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;

	public void TriggerDialogue ()
	{
		StartCoroutine(FindObjectOfType<DialogueManager>().StartDialogue(dialogue));
	}

}
