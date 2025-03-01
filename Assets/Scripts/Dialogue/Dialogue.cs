using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue {

	public DialogueTrigger nextDialogue;
	public UnityEvent onDialogueStart;
	public UnityEvent onDialogueEnd;

	[TextArea(3, 10)]
	[NonReorderable]	
	public string[] sentences;

}
