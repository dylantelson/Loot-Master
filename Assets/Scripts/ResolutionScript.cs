using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScript : MonoBehaviour
{

    public DungeonManager myDungeonManager;

    public string resolutionTextString;
    public Text resolutionText;
    public Text myButtonText;

    public string resolutionEffect;

    public float buttonYPos;
    public float mainTextYPos;

    public string resolutionNewLocation;

    public int eventTextFontSize = 36;
    public int buttonFontSize = 36;

    public GameObject resolutionButton;
    public GameObject mainTextHolder;

    public float timeToWaitTyping = 0.01f;
    private bool isTyping = false;

    public GameObject spriteToShow;
    public float spriteToShowYPos;

    void Start()
    {
        resolutionButton.gameObject.SetActive(false);
        gameObject.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        myDungeonManager = FindObjectOfType<DungeonManager>();
        resolutionText.fontSize = eventTextFontSize;
        myButtonText.fontSize = buttonFontSize;

        resolutionButton.transform.position = new Vector2(resolutionButton.transform.position.x, buttonYPos);
        mainTextHolder.transform.position = new Vector2(mainTextHolder.transform.position.x, resolutionButton.transform.position.y + 1.7f);

        resolutionText.alignment = TextAnchor.LowerLeft;
        myButtonText.alignment = TextAnchor.MiddleLeft;
        myButtonText.transform.localScale = new Vector2(0.9f, 1f);

        resolutionText.GetComponent<RectTransform>().sizeDelta = new Vector2(913,115);
        
        resolutionText.alignByGeometry = true;
        myButtonText.alignByGeometry = true;
    }
    public void SetText(string resText, string resButtonText, float timeToWaitTyping)
    {
        if (spriteToShow != null)
        {
            GameObject character = Instantiate(spriteToShow, new Vector2(0f, spriteToShowYPos), transform.rotation);
            if (character.GetComponent<Image>() != null)
            {
                character.transform.SetParent(gameObject.transform);
                character.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, spriteToShowYPos);
            }
            else
            {
                character.transform.SetParent(GameObject.Find("Doors(Clone)").transform);
                character.transform.position = new Vector2(0f, spriteToShowYPos / 100f);
            }
        }
        this.timeToWaitTyping = timeToWaitTyping;
        resolutionTextString = resText;
        resolutionText.text = "<color=#00ffff00>" + resolutionTextString + "</color>";
        myButtonText.text = resButtonText;
        resolutionText.GetComponent<RectTransform>().sizeDelta = new Vector2(913f,115f);
        resolutionText.transform.parent.transform.position = new Vector2(resolutionText.transform.parent.transform.position.x,
            myButtonText.transform.parent.transform.position.y + 1.7f);
        StartCoroutine("TypeMessage");
    }
    
    void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            StopCoroutine("TypeMessage");
            resolutionText.text = resolutionTextString;
            StartCoroutine("ShowButton");
        }
    }
    
    IEnumerator TypeMessage()
    {
        isTyping = true;
        int characterIndex = 0;
        while (characterIndex <= resolutionTextString.Length)
        {
            resolutionText.text = resolutionTextString.Substring(0, characterIndex) + "<color=#00ffff00>" + resolutionTextString.Substring(characterIndex, resolutionTextString.Length - characterIndex) + "</color>";
            characterIndex++;
            yield return new WaitForSeconds(timeToWaitTyping);
        }
        isTyping = false;
        StartCoroutine("ShowButton");
    }
    
    IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(0.1f);
        resolutionButton.gameObject.SetActive(true);
        StartCoroutine("Animations");
    }

    IEnumerator Animations()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.7f);
            resolutionButton.transform.Translate(Vector2.up * 0.04f);
            yield return new WaitForSeconds(0.7f);
            resolutionButton.transform.Translate(Vector2.down * 0.04f);
        }
    }
    
    public void Clicked()
    {
        if (System.String.IsNullOrEmpty(resolutionEffect) == false) myDungeonManager.Trigger(resolutionEffect);
        if (System.String.IsNullOrEmpty(resolutionNewLocation) == false) myDungeonManager.NewLocation(resolutionNewLocation);
        myDungeonManager.SetActive();
        StopCoroutine("Animations");
        Destroy(gameObject);
    }
}
