using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{

    public Animator BlackScreenAnimator;
    
    public Text youText;
    public Text areText;
    public Text deadText;

    public Text lootCollectedText;
    public Text lootAmountText;
    public Text pressToContinueText;

    public Animator lootCollectedHolder;

    private bool isTyping = false;

    public bool isFinished = false;

    private string phase = "start";
    
    //public GameObject youText;
    
    void Update()
    {
        if (isFinished && Input.GetMouseButtonDown(0))
        {
            FindObjectOfType<DungeonManager>().NewGame();
            Destroy(gameObject);
        }
    }

    public void ShowDead()
    {
        StartCoroutine(ShowTexts());
    }

    IEnumerator ShowTexts()
    {
        BlackScreenAnimator.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(1.3f);
        youText.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.5f);
        areText.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.5f);
        deadText.GetComponent<Animator>().SetTrigger("FadeIn");

        yield return new WaitForSeconds(1f);
        if (FindObjectOfType<BattleManager>() != null)
        {
            BattleManager myBattleManager = FindObjectOfType<BattleManager>();
            myBattleManager.Reset();
            Destroy(myBattleManager.gameObject);
        }
        StartCoroutine(TypeMessage());
    }
    
    IEnumerator TypeMessage()
    {
        isTyping = true;

        lootAmountText.text = "" + FindObjectOfType<DungeonManager>().GetGold();

        //typemessage for the Loot Collected text
        string lootCollectedFull = "Loot Collected:";
        int characterIndex = 0;
        while (characterIndex <= lootCollectedFull.Length)
        {
            lootCollectedText.text = lootCollectedFull.Substring(0, characterIndex) + "<color=#00000000>" + lootCollectedFull.Substring(characterIndex, lootCollectedFull.Length - characterIndex) + "</color>";
            characterIndex++;
            yield return new WaitForSeconds(0.05f);
        }
        
        lootCollectedHolder.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(0.7f);
        
        string tapToContinueFull = "Tap Anywhere to Continue";
        characterIndex = 0;
        while (characterIndex <= tapToContinueFull.Length)
        {
            pressToContinueText.text = tapToContinueFull.Substring(0, characterIndex) + "<color=#00000000>" + tapToContinueFull.Substring(characterIndex, tapToContinueFull.Length - characterIndex) + "</color>";
            characterIndex++;
            yield return new WaitForSeconds(0.05f);
        }
    
        isTyping = false;
        isFinished = true;
    }
}
