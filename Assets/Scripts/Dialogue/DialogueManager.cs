using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour {

	public TMP_Text dialogueText;
	public Animator animator;
	UnityEvent onDialogueEnd;

	private Queue<string> sentences;
	Dialogue nextDialogue;
	private string currentSentence;


	// Use this for initialization
	void Start () {
		sentences = new Queue<string>();
	}

	void Update () {
		if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
			if (dialogueText.text != currentSentence && currentSentence != null)
			{
				StopAllCoroutines();
				dialogueText.text = currentSentence;
				animator.Play("Idle_BetweenTalking");
			}
			else
			{
				DisplayNextSentence();
			}
		}
	}

	public IEnumerator StartDialogue (Dialogue dialogue)
	{
		dialogue.onDialogueStart?.Invoke();

		if (!animator.GetBool("IsIn"))
		{
			animator.Play("Pop_in");
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.5f);
			animator.SetBool("IsIn", true);
		}

		// Check if there is another dialogue afterwards
		if (dialogue.nextDialogue) {
			nextDialogue = dialogue.nextDialogue.dialogue;
		}
		else {
			nextDialogue = null;
		}

		// Check if there is another dialogue afterwards
		if (dialogue.onDialogueEnd != null) {
			onDialogueEnd = dialogue.onDialogueEnd;
		}
		else {
			onDialogueEnd = null;
		}

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0)
		{
			StartCoroutine(EndDialogue());
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{
		animator.Play("Talking");
		currentSentence = sentence;
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			if (AudioManager.instance)
			{
        		AudioManager.instance.Play("Type");
			}
			dialogueText.text += letter;
			if (letter.Equals(" "))
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.05f);
			}
		}
		animator.Play("Idle_BetweenTalking");
	}

	IEnumerator EndDialogue()
	{
		onDialogueEnd?.Invoke();
		onDialogueEnd = null;

		if (nextDialogue != null) {
			StartCoroutine(StartDialogue(nextDialogue));
			yield return null;
		}

		if (animator.GetBool("IsIn"))
		{
			animator.Play("Pop_out");

			// Theres a delay before it considers the length of the Pop_out animation
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.25f);
			animator.SetBool("IsIn", false);
		}

	}

}
