using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    
    Dictionary<string,string> itemDescriptions = new Dictionary<string, string>
    {
        { "bone", "The bone of an unknown character or creature. Could prove to be a tasty snack for a bone-hungry beast." },
        { "warrior helmet", "A rockin' helmet for a rockin' boy." },
        {"sword", "A pretty ordinary sword. One-hander, pretty lame really."},
        {"fishing rod", "A fishing rod. To fish. Fish."},
        {"glock", "A glock acquired from a glocktopus. Would be very dangerous in the hands/tentacles of the wrong creature."}
    };

    private bool isPaused = false;
    public Canvas myCanvas;

    public Sprite hamburgerInGame;
    public Sprite hamburgerInMenu;
    public Sprite lootIconInGame;
    public Sprite lootIconInMenu;
    

    public Button pauseButton;
    public Image lootIcon;

    public DungeonManager myDungeonManager;

    public Button[] invSlots;

    public GameObject bigSquare;

    private string lastClickedItem;

    public Text confirmText;
    public Button trashButton;

    private bool isTrashing = false;

    List<GameObject> invSlotImages = new List<GameObject>();
    public void Paused()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseButton.GetComponent<Image>().sprite = hamburgerInGame;
            lootIcon.GetComponent<Image>().sprite = lootIconInGame;
            myCanvas.gameObject.SetActive(false);
            if(bigSquare.gameObject.activeSelf) CloseOpenedItem();
            StartCoroutine(ClosePauseMenu());
        }
        else
        {
            isPaused = true;
            pauseButton.GetComponent<Image>().sprite = hamburgerInMenu;
            lootIcon.GetComponent<Image>().sprite = lootIconInMenu;
            gameObject.transform.localScale = new Vector2 (1.8f, 1.8f);
            myCanvas.gameObject.SetActive(false);
            bigSquare.gameObject.SetActive(false);
            ShowInventory();
            StartCoroutine(OpenPauseMenu());
        }
    }

    private IEnumerator OpenPauseMenu() {
        //while (gameObject.transform.localScale.y < 20.5f) {
        //    gameObject.transform.localScale = new Vector2 (gameObject.transform.localScale.x + 0.83f, gameObject.transform.localScale.y + 1.6f);
        //    yield return new WaitForSeconds (0.05f);
        //}
        gameObject.GetComponent<Animator>().SetTrigger("PauseGrow");

        //gameObject.transform.localScale = new Vector2(12.3f, 22f);
        yield return new WaitForSeconds(0.28f);
        myCanvas.gameObject.SetActive(true);
    }

    public void InvSlotClicked(int item)
    {
        String itemName = myDungeonManager.inventory[item];
        lastClickedItem = itemName;
        foreach (Button slot in invSlots)
        {
            slot.gameObject.SetActive(false);
        }

        //GameObject bigSquare = Instantiate(Resources.Load<GameObject>("bigsquare"), transform.position, transform.rotation);
        //bigSquare.transform.SetParent(myCanvas.transform);
        //RectTransform bigSquareRect = bigSquare.GetComponent<RectTransform>();
        //bigSquareRect.anchoredPosition = new Vector2(0f, -491f);
        //bigSquareRect.sizeDelta = new Vector2(832f, 832f);
        //bigSquareRect.localScale = new Vector2(1f, 1f);

        bigSquare.gameObject.SetActive(true);
        bigSquare.transform.Find("ItemName").GetComponent<Text>().text = UppercaseFirst(itemName);
        bigSquare.transform.Find("ItemDescription").GetComponent<Text>().text = itemDescriptions[itemName];
        
        GameObject itemImageGameObject = new GameObject("itemImage");
        Image itemImage = itemImageGameObject.AddComponent<Image>();
        itemImage.sprite = (Resources.Load<Sprite>("inv" + itemName));
        itemImage.transform.SetParent(bigSquare.transform.Find("ItemBackground").transform);
        RectTransform itemImageRect = itemImage.GetComponent<RectTransform>();
        itemImageRect.anchoredPosition = new Vector2(-5f, -5f);
        itemImageRect.sizeDelta = new Vector2(175f, 175f);
        itemImageRect.localScale = new Vector2(1f, 1f);
        invSlotImages.Add(itemImageGameObject);
    }
    
    string UppercaseFirst(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        String[] splitStr = str.Split(' ');
        for(int i = 0; i < splitStr.Length; i++)
        {
            splitStr[i] = char.ToUpper(splitStr[i][0]) + splitStr[i].Substring(1).ToLower();
        }

        return String.Join(" ", splitStr);
        //return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    public void CloseOpenedItem()
    {
        confirmText.gameObject.SetActive(false);
        trashButton.GetComponent<Image>().color = Color.white;
        isTrashing = false;
        
        bigSquare.gameObject.SetActive(false);
        foreach (Button slot in invSlots)
        {
            slot.gameObject.SetActive(true);
        }
        ShowInventory();
    }

    public void TrashOpenedItem()
    {
        if (isTrashing) TrashConfirmed();
        else
        {
            confirmText.gameObject.SetActive(true);
            trashButton.GetComponent<Image>().color = Color.red;
            isTrashing = true;
        }
    }

    void TrashConfirmed()
    {
        confirmText.gameObject.SetActive(false);
        trashButton.GetComponent<Image>().color = Color.white;
        isTrashing = false;
        
        myDungeonManager.inventory.Remove(lastClickedItem);
        CloseOpenedItem();
        ShowInventory();
    }

    void ShowInventory()
    {
        foreach (GameObject slotImage in invSlotImages)
        {
            Destroy(slotImage);
        }
        for(int i = 0; i < myDungeonManager.inventory.Count; i++)
        {
            //invSlots[i].GetComponent<Image>().sprite = Resources.Load <Sprite>(myDungeonManager.inventory[i]);
            GameObject invItemImage = new GameObject("slotImage" + i);
            Image invSlotImage = invItemImage.AddComponent<Image>();
            invSlotImage.sprite = (Resources.Load<Sprite>("inv" + myDungeonManager.inventory[i]));
            invItemImage.transform.SetParent(invSlots[i].transform);
            //invItemImage.transform.position = new Vector2(-5f, -10f);
            RectTransform invItemRect = invItemImage.GetComponent<RectTransform>();
            invItemRect.anchoredPosition = new Vector2(-5f, -5f);
            invItemRect.sizeDelta = new Vector2(175f, 175f);
            invItemRect.localScale = new Vector2(1f, 1f);
            
            invSlotImages.Add(invItemImage);
            
            invSlots[i].interactable = true;
        }

        for (int i = myDungeonManager.inventory.Count; i < invSlots.Length; i++)
        {
            invSlots[i].interactable = false;
        }
    }
    
    private IEnumerator ClosePauseMenu() {
        confirmText.gameObject.SetActive(false);
        trashButton.GetComponent<Image>().color = Color.white;
        isTrashing = false;
        
        gameObject.GetComponent<Animator>().SetTrigger("PauseShrink");

        //gameObject.transform.localScale = new Vector2(12.3f, 22f);
        yield return new WaitForSeconds(0.35f);

        gameObject.SetActive(false);
    }
}
