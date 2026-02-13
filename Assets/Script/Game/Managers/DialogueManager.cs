using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XNode;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject prefabPlayerOption;
    [SerializeField] Transform gridPlayerOption;
    [SerializeField] GameObject dialogueParent;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] float speedShowingText = 1;
    [SerializeField] AudioClip typingClip;
    [SerializeField] InputActionReference inputAction;

    private DialogueTree dialogueTree;
    private DialogueNode currentNode;
    private List<GameObject> dialoguePOList;
    private bool interactionAreOpen = false;
    private bool isClicking;
    private PlayerInteractionType playerInteractionType = PlayerInteractionType.None;

    private Coroutine dialogueCorutine;

    private void Awake()
    {
        dialogueParent.SetActive(false);
        dialoguePOList = new List<GameObject>();
        inputAction.action.started += ctx => Click();
    }

    // Trigger By NPCMovement
    public void StartDialogue(DialogueTree tree)
    {
        dialogueTree = tree;

        // Szukamy pierwszego DialogueNode w grafie
        DialogueNode startNode = null;
        foreach (var node in tree.nodes)
        {
            if (node is DialogueNode)
            {
                startNode = node as DialogueNode;
                break; // Bierze pierwszy znaleziony
            }
        }

        if (startNode != null)
        {
            ChoseDialogueByNode(startNode);
        }
        else
        {
            Debug.LogError("Graf nie zawiera ¿adnych wêz³ów DialogueNode!");
        }
    }

    public void ChoseDialogueByID(string id, PlayerInteractionType pit = PlayerInteractionType.None)
    {
        //dialogueCorutine = StartCoroutine(Dialogue(dialogueTree.GetNode(id), pit));
    }

    // Zmieñ sygnaturê metody, by przyjmowa³a obiekt wêz³a zamiast ID
    public void ChoseDialogueByNode(DialogueNode node, PlayerInteractionType pit = PlayerInteractionType.None)
    {
        if(node == null)
        {
            Debug.Log("There you need a node");
            return;
        }

        if (dialogueCorutine != null) StopCoroutine(dialogueCorutine);
        dialogueCorutine = StartCoroutine(Dialogue(node, pit));
    }

    private IEnumerator Dialogue(DialogueNode node, PlayerInteractionType pit)
    {
        inputAction.action.Enable();
        dialogueParent.SetActive(true);
        if (playerInteractionType == PlayerInteractionType.Shoot)
        {
            if (GameManager.Instance.MovementManager.CurrentBullet > 0)
            {
                interactionAreOpen = true;
                StopCoroutine(dialogueCorutine);
                GameManager.Instance.NPCDocumentDisplay.InteractionNode(pit, GameManager.Instance.GetCurrentNpcData);
            }
        }else if (pit != PlayerInteractionType.None)
        {
            interactionAreOpen = true;
            GameManager.Instance.NPCDocumentDisplay.InteractionNode(pit, GameManager.Instance.GetCurrentNpcData);
        }

        yield return new WaitUntil(() => !interactionAreOpen);

        if (dialoguePOList != null)
        {
            for (int i = 0; i < dialoguePOList.Count; i++)
            {
                Destroy(dialoguePOList[i].gameObject);
            }

            dialoguePOList.Clear();
        }

        if(!string.IsNullOrEmpty(node.actionId))
        {
            GameManager.Instance.NPCMovement.CurrentNPCObject.ActionNPC(node.actionId);
            RunAction(node.actionId);
        }
        int countDialogs = node.npcLine.Count;
        for (int i = 0; i < node.npcLine.Count; i++)
        {
            char[] dialogueTextInChar = node.npcLine[i].Get().ToCharArray();
            dialogueText.text = GameManager.Instance.NPCMovement.CurrentNPCObject.NpcData.humanName + ": ";
            float waitForNextChar = 0f;
            float speed = speedShowingText;
            GameManager.Instance.SoundsManager.PlayOneTimeClipWithoutDestroying(typingClip, 0.6f, true);
            isClicking = false;
            for (int j = 0; j < dialogueTextInChar.Length; j++)
            {
                while (waitForNextChar > 0)
                {
                    waitForNextChar -= Time.deltaTime;

                    if(isClicking)
                    {
                        speed = 0;
                        isClicking = false;
                    }
                    yield return null;
                }
                dialogueText.text += dialogueTextInChar[j];
                waitForNextChar = speed;
            }
            GameManager.Instance.SoundsManager.DestroyAudioSourceByClipFromAdditionalAudio(typingClip);
            Debug.Log("First dialogue ");
            countDialogs--;
            if (countDialogs != 0)
            {
                yield return new WaitForSeconds(.5f);
                isClicking = false;
                yield return new WaitUntil(() => isClicking);
                isClicking = false;
            }
        }

        yield return new WaitForSeconds(1f);
        for (int j = 0; j < node.options.Count; j++)
        {
            GameObject obj = Instantiate(prefabPlayerOption, gridPlayerOption);
            obj.GetComponentInChildren<TMP_Text>().text = node.options[j].playerLine.Get();

            // Pobieramy port wyjœciowy dla danej opcji
            NodePort outPort = node.GetOutputPort("options " + j);
            ObjectPlayerOption pObj = obj.GetComponent<ObjectPlayerOption>();

            // Przekazujemy referencjê do nastêpnego wêz³a
            if (outPort != null)
            {
                if (outPort.IsConnected)
                {
                    pObj.nextNode = outPort.Connection.node as DialogueNode;
                }
                else
                    pObj.nextNode = null;
            }

            pObj.interactionType = node.options[j].typeInteraction;
            dialoguePOList.Add(obj);
        }

        if (node.exitNode)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            EndDialogue();
            GameManager.Instance.NPCMovement.EndDialogue();
        }
        inputAction.action.Disable();
    }

    private void Click() => isClicking = true;

    public void EndDialogue()
    {
        if (dialogueCorutine != null)
        {
            StopCoroutine(dialogueCorutine);
            GameManager.Instance.SoundsManager.DestroyAudioSourceByClipFromAdditionalAudio(typingClip);
        }
        currentNode = null;
        dialogueTree = null;
        inputAction.action.Disable();

        if (dialoguePOList != null)
        {
            for (int i = 0; i < dialoguePOList.Count; i++)
            {
                Destroy(dialoguePOList[i].gameObject);
            }

            dialoguePOList.Clear();
        }
        dialogueText.text = "";
        dialogueParent.gameObject.SetActive(false);
        interactionAreOpen = false;
    }

    private void RunAction(string id)
    {
        // Test
    }


    public void StopInteraction() => interactionAreOpen = false;
}
