using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{

    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;

    public Text choice1Text;
    public Text choice2Text;
    public Text choice3Text;

    public Text eventText;
    public string eventString;

    public string decision1;
    public string decision2;
    public string decision3;

    public string location;

    public string decision1Outcome;
    public string decision2Outcome;
    public string decision3Outcome;

    public int decision1MoralityMod = 0;
    public int decision2MoralityMod = 0;
    public int decision3MoralityMod = 0;
    
    public string choice1ResolutionText;
    public string choice1ResolutionButtonText;

    public string choice2ResolutionText;
    public string choice2ResolutionButtonText;

    public string choice3ResolutionText;
    public string choice3ResolutionButtonText;

    public bool triggerAtResolution1;
    public bool triggerAtResolution2;
    public bool triggerAtResolution3;

    public bool choice2Locked;
    public bool choice3Locked;

    public string choice2Requirement;
    public string choice3Requirement;

    public string choice1NewLocation;
    public string choice2NewLocation;
    public string choice3NewLocation;

    public bool repeatable;

    public int EventTextFontSize = 40;
    public int ButtonsFontSize = 40;

    private Sprite choiceButtonSprite;

    public GameObject resolutionObject;
    
    //must divide the actual numbers by 192 to get what you want? canvases work pretty weird
    private float choice1ButtonYPos = -7.4f;
    private float choiceButtonSpacing = 1.35f;

    DungeonManager myDungeonManager;

    public bool isThereCharacter;
    public GameObject spriteToShow;
    public float spriteToShowYPos;

    public float timeToWaitTyping = 0.01f;
    private bool isTyping = false;

    public bool choice1ResolutionHasCharacter = false;
    public bool choice2ResolutionHasCharacter = false;
    public bool choice3ResolutionHasCharacter = false;

    public GameObject resolution1Character;
    public GameObject resolution2Character;
    public GameObject resolution3Character;
        
    //private GameObject character;

    // void Update()
    // {
    //     if (Input.GetKeyDown("space"))
    //     {
    //         print("space key was pressed");
    //         choice1Button.transform.position = new Vector2(choice1Button.transform.position.x, -1.458);
    //     }
    // }

    void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            StopCoroutine("TypeMessage");
            eventText.text = eventString;
            StartCoroutine("ShowButtons");
        }
    }
    
    void Start()
    {
        if (isThereCharacter)
        {
            GameObject character = Instantiate(spriteToShow, new Vector2(0f, spriteToShowYPos), transform.rotation);
            character.transform.SetParent(gameObject.transform);
            character.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, spriteToShowYPos);
        }

        choiceButtonSprite = Resources.Load <Sprite>("choicebutton");
        
        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);
        choice3Button.gameObject.SetActive(false);
        
        myDungeonManager = FindObjectOfType<DungeonManager>();
        gameObject.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        choice1Text.text = decision1;
        choice2Text.text = decision2;
        choice3Text.text = decision3;
        //eventText.text = eventString;
        StartCoroutine("TypeMessage");

        choice1Text.fontSize = ButtonsFontSize;
        choice2Text.fontSize = ButtonsFontSize;
        choice3Text.fontSize = ButtonsFontSize;
        eventText.fontSize = EventTextFontSize;
        
        choice1Text.font = (Font)Resources.Load("courbd");
        choice2Text.font = (Font)Resources.Load("courbd");
        choice3Text.font = (Font)Resources.Load("courbd");
        eventText.font = (Font)Resources.Load("courbd");

        choice1Text.alignment = TextAnchor.MiddleLeft;
        choice2Text.alignment = TextAnchor.MiddleLeft;
        choice3Text.alignment = TextAnchor.MiddleLeft;
        eventText.alignment = TextAnchor.LowerLeft;

        choice1Text.transform.position =
            new Vector3(choice1Text.transform.position.x + 0f, choice1Text.transform.position.y + 0.1f, choice1Text.transform.position.z);
        choice2Text.transform.position =
            new Vector3(choice2Text.transform.position.x + 0f, choice2Text.transform.position.y + 0.1f, choice2Text.transform.position.z);
        choice3Text.transform.position =
            new Vector3(choice3Text.transform.position.x + 0f, choice3Text.transform.position.y + 0.1f, choice3Text.transform.position.z);

        choice1Text.transform.localScale = new Vector2(0.9f, 1f);
        choice2Text.transform.localScale = new Vector2(0.9f, 1f);
        choice3Text.transform.localScale = new Vector2(0.9f, 1f);
        
        choice1Text.alignByGeometry = true;
        choice2Text.alignByGeometry = true;
        choice3Text.alignByGeometry = true;
        eventText.alignByGeometry = true;
        
        choice1Text.color = Color.black;
        choice2Text.color = Color.black;
        choice3Text.color = Color.black;
        if (choice2Locked)
        {
            choice2Button.interactable = false;
            choice2Text.color = new Color(99f/255f, 99f/255f, 99f / 255f, 255f / 255f);
            if (myDungeonManager.FulfillsReq(choice2Requirement))
            {
                choice2Text.color = new Color(8f / 255f, 163f / 255f, 255f / 255f, 255f / 255f);
                choice2Locked = false;
                choice2Button.interactable = true;
            }
        }
        if (choice3Locked)
        {
            choice3Button.interactable = false;
            choice3Text.color = new Color(99f / 255f, 99f / 255f, 99f / 255f, 255f / 255f);
            if (myDungeonManager.FulfillsReq(choice3Requirement))
            {
                choice3Text.color = new Color(8f / 255f, 163f / 255f, 255f / 255f, 255f / 255f);
                choice3Locked = false;
                choice3Button.interactable = true;
            }
        }

        if (System.String.IsNullOrEmpty(decision2)) Destroy(choice2Button.gameObject);
        if (System.String.IsNullOrEmpty(decision3)) Destroy(choice3Button.gameObject);

        choice1Button.GetComponent<RectTransform>().sizeDelta = new Vector2(913,115);
        choice2Button.GetComponent<RectTransform>().sizeDelta = new Vector2(913,115);
        choice3Button.GetComponent<RectTransform>().sizeDelta = new Vector2(913,115);
        
        choice1Button.GetComponent<Image>().sprite = choiceButtonSprite;
        choice2Button.GetComponent<Image>().sprite = choiceButtonSprite;
        choice3Button.GetComponent<Image>().sprite = choiceButtonSprite;
        choice1Button.transform.position = new Vector2(0f, choice1ButtonYPos);
        choice2Button.transform.position = new Vector2(0f, choice1ButtonYPos - choiceButtonSpacing);
        choice3Button.transform.position = new Vector2(0f, choice1ButtonYPos - choiceButtonSpacing * 2);
        
        Transform eventTextBackground = gameObject.transform.Find("Background");
        eventTextBackground.GetComponent<Image>().enabled = false;
        eventText.GetComponent<RectTransform>().sizeDelta = new Vector2(913,115);
        eventText.text = "<color=#00000000>" + eventString + "</color>";
        eventTextBackground.transform.position = new Vector2(eventTextBackground.transform.position.x,
            choice1Button.transform.position.y + 1.7f);
    }

    public string GetLocation() { return location; }

    IEnumerator TypeMessage()
    {
        isTyping = true;
        int characterIndex = 0;
        while (characterIndex <= eventString.Length)
        {
            eventText.text = eventString.Substring(0, characterIndex) + "<color=#00000000>" + eventString.Substring(characterIndex, eventString.Length - characterIndex) + "</color>";
            characterIndex++;
            yield return new WaitForSeconds(timeToWaitTyping);
        }

        isTyping = false;
        StartCoroutine("ShowButtons");
    }

    IEnumerator Animations()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);
            if(decision1 != "") choice1Button.transform.Translate(Vector2.up * 0.04f);
            if(decision2 != "") choice2Button.transform.Translate(Vector2.up * 0.04f);
            if(decision3 != "") choice3Button.transform.Translate(Vector2.up * 0.04f);
            yield return new WaitForSeconds(0.7f);
            if(decision1 != "") choice1Button.transform.Translate(Vector2.down * 0.04f);
            if(decision2 != "") choice2Button.transform.Translate(Vector2.down * 0.04f);
            if(decision3 != "") choice3Button.transform.Translate(Vector2.down * 0.04f);
        }
    }

    IEnumerator ShowButtons()
    {
        yield return new WaitForSeconds(0.15f);
        choice1Button.gameObject.SetActive(true);

        if (decision2 == "")
        {
            StartCoroutine("Animations");
            yield break;
        }
        yield return new WaitForSeconds(0.15f);
        choice2Button.gameObject.SetActive(true);
        if (decision3 == "")
        {
            StartCoroutine("Animations");
            yield break;
        }
        yield return new WaitForSeconds(0.15f);
        choice3Button.gameObject.SetActive(true);
        StartCoroutine("Animations");
    }
    public void Clicked(int choice)
    {
        if (choice == 1)
        {
            myDungeonManager.morality += decision1MoralityMod;
            if(decision1Outcome.Length > 4 && decision1Outcome.Substring(0, 4) == "next")
            {
                myDungeonManager.NewEvent(decision1Outcome);
                StopCoroutine("Animations)");
                Destroy(gameObject);
                return;
            }
            GameObject resolutionInstance = Instantiate(resolutionObject, transform.position, transform.rotation);
            ResolutionScript resScript = resolutionInstance.GetComponent<ResolutionScript>();
            if (decision1Outcome != "")
            {
                if (System.String.IsNullOrEmpty(choice1NewLocation) == false) myDungeonManager.NewLocation(choice1NewLocation);
                if (triggerAtResolution1)
                    resScript.resolutionEffect = decision1Outcome;
                else
                    myDungeonManager.Trigger(decision1Outcome);
            }
            //if (System.String.IsNullOrEmpty(choice1NewLocation) == false) resScript.resolutionNewLocation = choice1NewLocation;
            resScript.buttonYPos = choice1ButtonYPos;
            resScript.mainTextYPos = choice1Button.transform.position.y + 1.7f;
            if (choice1ResolutionHasCharacter)
            {
                resScript.spriteToShowYPos = this.spriteToShowYPos;
                if (resolution1Character == null) resScript.spriteToShow = this.spriteToShow;
                else resScript.spriteToShow = resolution1Character;
            }
            resScript.SetText(choice1ResolutionText, choice1ResolutionButtonText, timeToWaitTyping);
            StopCoroutine("Animations)");
            Destroy(gameObject);
        }
        if (choice == 2 && !choice2Locked)
        {
            myDungeonManager.morality += decision2MoralityMod;
            if (decision2Outcome.Length > 4 && decision2Outcome.Substring(0, 4) == "next")
            {
                myDungeonManager.NewEvent(decision2Outcome);
                StopCoroutine("Animations)");
                Destroy(gameObject);
                return;
            }

            GameObject resolutionInstance = Instantiate(resolutionObject, transform.position, transform.rotation);
            ResolutionScript resScript = resolutionInstance.GetComponent<ResolutionScript>();
            if (choice2ResolutionHasCharacter)
            {
                resScript.spriteToShowYPos = this.spriteToShowYPos;
                if (resolution2Character == null) resScript.spriteToShow = this.spriteToShow;
                else resScript.spriteToShow = resolution2Character;
            }
            resScript.SetText(choice2ResolutionText, choice2ResolutionButtonText, timeToWaitTyping);
            if (decision2Outcome != "")
            {
                if (System.String.IsNullOrEmpty(choice2NewLocation) == false) myDungeonManager.NewLocation(choice2NewLocation);
                if (triggerAtResolution2)
                    resScript.resolutionEffect = decision2Outcome;
                else
                    myDungeonManager.Trigger(decision2Outcome);
            }
            //if (System.String.IsNullOrEmpty(choice2NewLocation) == false) resScript.resolutionNewLocation = choice2NewLocation;
            if (System.String.IsNullOrEmpty(choice2Requirement) == false) myDungeonManager.CheckRemoval(choice2Requirement);
            resScript.buttonYPos = choice1ButtonYPos;
            resScript.mainTextYPos = choice1Button.transform.position.y + 1.7f;
            StopCoroutine("Animations)");
            Destroy(gameObject);
        }
        if (choice == 3 && !choice3Locked)
        {
            myDungeonManager.morality += decision3MoralityMod;
            if (decision3Outcome.Length > 4 && decision3Outcome.Substring(0, 4) == "next")
            {
                myDungeonManager.NewEvent(decision3Outcome);
                StopCoroutine("Animations)");
                Destroy(gameObject);
                return;
            }
            GameObject resolutionInstance = Instantiate(resolutionObject, transform.position, transform.rotation);
            ResolutionScript resScript = resolutionInstance.GetComponent<ResolutionScript>();
            if (choice3ResolutionHasCharacter)
            {
                resScript.spriteToShowYPos = this.spriteToShowYPos;
                if (resolution3Character == null) resScript.spriteToShow = this.spriteToShow;
                else resScript.spriteToShow = resolution3Character;
            }
            resScript.SetText(choice3ResolutionText, choice3ResolutionButtonText, timeToWaitTyping);
            if (decision3Outcome != "")
            {
                if (System.String.IsNullOrEmpty(choice3NewLocation) == false) myDungeonManager.NewLocation(choice3NewLocation);
                if (triggerAtResolution3)
                    resScript.resolutionEffect = decision3Outcome;
                else
                    myDungeonManager.Trigger(decision3Outcome);
            }
            //if (System.String.IsNullOrEmpty(choice3NewLocation) == false) resScript.resolutionNewLocation = choice3NewLocation;
            if (System.String.IsNullOrEmpty(choice3Requirement) == false) myDungeonManager.CheckRemoval(choice3Requirement);
            resScript.buttonYPos = choice1ButtonYPos;
            resScript.mainTextYPos = choice1Button.transform.position.y + 1.7f;
            StopCoroutine("Animations)");
            Destroy(gameObject);
        }
    }
}
