using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Rendering.MaterialUpgrader;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] GameObject prefabPlayerOption;
    [SerializeField] Transform gridPlayerOption;
    [SerializeField] GameObject dialogueParent;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] float speedShowingText = 1;

    private DialogueTree dialogueTree;
    private DialogueNode currentNode;
    private List<GameObject> dialoguePOList;
    private bool interactionAreOpen = false;
    private PlayerInteractionType playerInteractionType = PlayerInteractionType.None;

    public delegate void OnEndDialogue();

    private void Awake()
    {
        Instance = this;
        dialogueParent.SetActive(false);
        
    }

    private void OnEnable()
    {
        
    }

    public void Initialize()
    {
        dialoguePOList = new List<GameObject>();
    }

    public void StartDialogue(DialogueTree tree)
    {
        dialogueTree = tree;
        currentNode = tree.GetNode(1.ToString());
        ChoseDialogueByID(currentNode.nodeId);
    }

    public void ChoseDialogueByID(string id, PlayerInteractionType pit = PlayerInteractionType.None)
    {
        StartCoroutine(Dialogue(dialogueTree.GetNode(id), pit));
    }

    private IEnumerator Dialogue(DialogueNode node, PlayerInteractionType pit)
    {
        dialogueParent.SetActive(true);
        if(pit != PlayerInteractionType.None)
        {
            interactionAreOpen = true;
            NPCManager.Instance.InteractionNode(pit);
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

        Debug.Log("Start dialog");
        if(!string.IsNullOrEmpty(node.actionId))
        {
            NPCManager.Instance.CurrentNPCObject.ActionNPC(node.actionId);
        }
        if(!string.IsNullOrEmpty(node.actionId))
        {
            RunAction(node.actionId);
        }
        int countDialogs = node.npcLine.Count;
        for (int i = 0; i < node.npcLine.Count; i++)
        {
            char[] dialogueTextInChar = node.npcLine[i].Get().ToCharArray();
            dialogueText.text = NPCManager.Instance.CurrentNPCObject.NPCDatas.humanName + ": ";
            float waitForNextChar = 0f;
            for (int j = 0; j < dialogueTextInChar.Length; j++)
            {
                while (waitForNextChar > 0)
                {
                    waitForNextChar -= Time.deltaTime;
                    yield return null;
                }
                dialogueText.text += dialogueTextInChar[j];
                waitForNextChar = speedShowingText;
            }
            Debug.Log("First dialogue ");
            countDialogs--;
            if (countDialogs != 0)
            {
                yield return new WaitForSeconds(.5f);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }
        }

        yield return new WaitForSeconds(1f);
        for (int j = 0; j < node.options.Count; j++)
        {
            GameObject obj = Instantiate(prefabPlayerOption, gridPlayerOption);
            obj.GetComponentInChildren<TMP_Text>().text = node.options[j].playerLine.Get();
            ObjectPlayerOption pObj = obj.GetComponent<ObjectPlayerOption>();
            pObj.nextID = node.options[j].nextNodeId;
            pObj.interactionType = node.options[j].typeInteraction;
            dialoguePOList.Add(obj);
        }

        if (node.exitNode)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        currentNode = null;
        dialogueTree = null;

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
        NPCManager.Instance.EndDialogue();
    }

    private void RunAction(string id)
    {
        switch (id)
        {
            case "test":

                break;
        }
    }


    public void StopInteraction() => interactionAreOpen = false;
}
