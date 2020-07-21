using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootScreen : MonoBehaviour
{
    
    public Button[] invSlots;
    List<GameObject> invSlotImages = new List<GameObject>();
    public GameObject myInventory;
    private Canvas myCanvas;
    private DungeonManager myDungeonManager;
    private BattleManager myBattleManager;
    public GameObject chest;
    public Button checkButton;
    public GameObject lootGainedObject;
    
    bool isTrashing = false;
    public Text confirmText;

    void Start()
    {
        myDungeonManager = GameObject.FindObjectOfType<DungeonManager>();
        myBattleManager = GameObject.FindObjectOfType<BattleManager>();
        myCanvas = gameObject.transform.Find("Canvas").GetComponent<Canvas>();
    }
    
    public void ShowCheckButton()
    {
        checkButton.gameObject.SetActive(true);
    }
    
    public void ShowChest()
    {
        //lootGainedObject.SetActive(false);
        chest.SetActive(true);
    }
    
    public void TrashOpenedItem(int slotNum)
    {
        if (isTrashing) TrashConfirmed(slotNum);
        else
        {
            confirmText.gameObject.SetActive(true);
            invSlots[slotNum].GetComponent<Image>().color = Color.red;
            isTrashing = true;
        }
    }

    void TrashConfirmed(int slotNum)
    {
        confirmText.gameObject.SetActive(false);
        invSlots[slotNum].GetComponent<Image>().color = Color.white;
        isTrashing = false;
        
        myDungeonManager.inventory.RemoveAt(slotNum);
        ShowInventory();
    }

    public void CheckButtonClicked()
    {
        myBattleManager.Continue();
        //myCanvas.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void ShowInventory()
    {
        foreach (GameObject slotImage in invSlotImages)
        {
            Destroy(slotImage);
        }

        for (int i = 0; i < myDungeonManager.inventory.Count; i++)
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
        chest.SetActive(false);
        myInventory.gameObject.SetActive(true);
    }
}