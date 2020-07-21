using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class DungeonManager : MonoBehaviour {

	public GameObject battleManagerObject;
    public GameObject battleManagerObjectToPick;
    public GameObject currObject;
    public GameObject doorObject;
    private BattleManager myBattleManager;
	private DungeonManager myDungeonManager;

    public bool isPaused;
    public GameObject pauseScreen;
    public Event myEventPaused;

	public bool active = true;

    public float slideSpeed = 0.5f;

    private int gold;
    public readonly int numOfEvents;
    public List<GameObject> totalEvents;
    public List<GameObject> eventsLeft;
    public List<GameObject> nestedEvents;
    public List<GameObject> queuedEvents;

    public List<string> queuedEventsNames;

    public List<string> inventory = new List<string>();
    //inventory can be list of inventory items, or there can be accumulators for each item such as amountOfSwords and increment/decrement every time it's modified?

    List<string> companions = new List<string>();

    public int str = 5;
    public int intel = 5;
    public int dex = 5;
    public int morality = 0;

    public Text lootText;

    public GameObject deathScreenPrefab;

    //temporary, for testing- start with multiple items
    void Start()
    {
        eventsLeft = new List<GameObject>(totalEvents);
        inventory.Add("warrior helmet");
        inventory.Add("bone");
        inventory.Add("sword");
        inventory.Add("fishing rod");
        inventory.Add("glock");
        inventory.Add("bone");
        inventory.Add("sword");
        inventory.Add("fishing rod");
        inventory.Add("glock");
    }
    
    public GameObject NewEvent(string eventName)
    {
        GameObject myEvent;
        int i = 0;
        if (eventName.Length > 4 && eventName.Substring(0, 4) == "next")
        {
            while (i < nestedEvents.Count && nestedEvents[i].name != eventName) i++;
            myEvent = Instantiate(nestedEvents[i], new Vector2(transform.position.x, transform.position.y), transform.rotation);
            return myEvent;
        }
        else if (eventName.Length > 6 && eventName.Substring(0, 6) == "queued")
        {
            while (i < queuedEvents.Count && queuedEvents[i].name != eventName) i++;
            myEvent = Instantiate(queuedEvents[i], new Vector2(transform.position.x, transform.position.y), transform.rotation);
            return myEvent;
        }
        while (i < eventsLeft.Count && eventsLeft[i].name != eventName) i++;
        myEvent = Instantiate(eventsLeft[i], new Vector2(transform.position.x, transform.position.y), transform.rotation);
        if (myEvent.GetComponent<Event>() != null && myEvent.GetComponent<Event>().repeatable == false) eventsLeft.RemoveAt(i);
        return myEvent;
    }

    public GameObject NewEvent()
    {

        GameObject myEvent = RandomEvent();
        return myEvent;
    }

    public void NewGame()
    {
        inventory.Clear();
        companions.Clear();
        gold = 0;
        lootText.text = "" + 0;
        eventsLeft = new List<GameObject>(totalEvents);
        NewDoorsInstant();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // Debug.Log("Gold: " + gold);
        // Debug.Log("Inventory:");
        // PrintInventory();
        // PrintCompanions();
        
        GameObject deathScreen = Instantiate(deathScreenPrefab, transform.position, Quaternion.identity);
        deathScreen.GetComponent<DeathScreen>().ShowDead();
    }

    public int GetGold()
    {
        return gold;
    }

    public void GiveGold(int goldToGive)
    {
        gold += goldToGive;
        lootText.text = gold.ToString();
    }

    public void GiveGold()
    {
        int goldToAdd = Random.Range(100, 301);
        gold += goldToAdd;
        lootText.text = gold.ToString();
        Debug.Log("Received " + goldToAdd + " gold!");
    }

    public void GiveBone()
    {
        inventory.Add("bone");
        Debug.Log("Received a bone!");
    }
    
    public void GiveLoot(string item)
    {
        inventory.Add(item);
        Debug.Log("Received 1 " + item + "!");
    }

    public void GiveSword()
    {
        inventory.Add("sword");
        Debug.Log("Received a sword!");
    }
    
    public void GiveFishingRod()
    {
        inventory.Add("fishing rod");
        Debug.Log("Received a fishing rod!");
    }

    public void GiveHelmet()
    {
        inventory.Add("warrior helmet");
        Debug.Log("Received a helmet!");
    }

   public void GiveKey()
    {
        inventory.Add("key");
        Debug.Log("Received a key!");
    }

   public void SetActive()
    {
        active = true;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("p"))
        {
            PrintInventory();
        }

        if (Input.GetKeyDown("c"))
        {
            PrintCompanions();
        }
    }

    public void NewLocation(string location)
    {
        if (location == "door") NewNormalDoors();
        if (location == "battle") SwitchToBattle();
    }

    public void SwitchToBattle()
    {
        GameObject battleScreen = Instantiate(battleManagerObjectToPick);
        Destroy(currObject);
        //active = shouldBeActive;
        currObject = battleScreen;
        currObject.transform.position = new Vector2(0f, 0f);
    }

    public void DoorClicked(string dir) {
        //myBattleManager.SlideIn (dir);

        if (isPaused)
        {
            return;
        }

        GameObject myEvent;


        if (active) {


            if(queuedEventsNames.Count > 0)
            {
                myEvent = NewEvent(queuedEventsNames[0]);
                queuedEventsNames.RemoveAt(0);

                myEvent.SetActive(false);

                if (myEvent.GetComponent<Event>() != null)
                {
                    Event myEventScript = myEvent.GetComponent<Event>();
                    StartCoroutine(Slide(dir, myEventScript.GetLocation(), false, myEvent));
                }
                else
                {
                    BattleEvent myEventScript = myEvent.GetComponent<BattleEvent>();
                    StartCoroutine(Slide(dir, myEventScript.GetLocation(), false, myEvent));
                }
            }

            else if (Random.Range (1, 2) == 1) {
                myEvent = NewEvent();
                myEvent.SetActive(false);
                if (myEvent.GetComponent<Event>() != null)
                {
                    Event myEventScript = myEvent.GetComponent<Event>();
                    StartCoroutine(Slide(dir, myEventScript.GetLocation(), false, myEvent));
                }
                else
                {
                    BattleEvent myEventScript = myEvent.GetComponent<BattleEvent>();
                    StartCoroutine(Slide(dir, myEventScript.GetLocation(), false, myEvent));
                }
            } else {
                StartCoroutine(Slide(dir, "door", true));
			}
			active = false;
		}
	}

    public void Trigger(string triggerName, int amount)
    {
        if (triggerName == "GiveGold")
        {
            GiveGold(amount);
            return;
        }
    }

    public bool FulfillsReq(string requirement)
    {
        if (inventory.Contains(requirement)) return true;
        return false;
    }

    public void PrintInventory()
    {
        foreach (string item in inventory) Debug.Log(item);
    }

    public void PrintCompanions()
    {
        foreach (string companion in companions) Debug.Log(companion);
    }

    public void CheckRemoval(string requirement)
    {
        if (inventory.Contains(requirement)) inventory.Remove(requirement);
        //here, add all items which would be lost when used in a quest (i.e. giving a skeleton a bone, you lose the bone). if easier, could do reverse (if not requirements which you don't lose)
        //if (requirement == "bone" || requirement == "warrior helmet")
        //{
            //inventory.Remove(requirement);
        //}
    }

    public void PauseClicked()
    {
        if (pauseScreen.activeSelf)
        {
            //If pause screen is enabled, unpause game
            if (myEventPaused != null)
            {
                myEventPaused.gameObject.SetActive(true);
            }
            isPaused = false;
        } else {
            //If pause screen is disabled, pause game
            pauseScreen.SetActive(true);
            myEventPaused = FindObjectOfType<Event>();
            if (myEventPaused != null)
            {
                myEventPaused.gameObject.SetActive(false);
            }
            isPaused = true;
        }
        pauseScreen.GetComponent<PauseScript>().Paused();
    }
    
    public void Trigger(string triggerName)
    {
        if(triggerName == "GiveHelmet")
        {
            GiveHelmet();
            return;
        }
        else if (triggerName == "GiveSword")
        {
            GiveSword();
            return;
        }
        else if (triggerName == "GiveBone")
        {
            GiveBone();
            return;
        }
        else if (triggerName.Substring(0, 6) == "battle")
        {
            myBattleManager = FindObjectOfType<BattleManager>();
            myBattleManager.BattleStart(triggerName.Substring(6, triggerName.Length - 6));
            return;
        }
        else if (triggerName == "GiveGold")
        {
            GiveGold();
            return;
        }
        else if(triggerName.Length > 12 && triggerName.Substring(0, 12) == "NewCompanion")
        {
            companions.Add(triggerName.Substring(12));
            Debug.Log(triggerName.Substring(12) + " added to party!");
            return;
        }
        else if (triggerName.Length > 6 && triggerName.Substring(0, 6) == "queued")
        {
            queuedEventsNames.Add(triggerName);
            Debug.Log("Queued event " + triggerName + " added!");
            return;
        }
        else if (triggerName == "GameOver")
        {
            GameOver();
            return;
        }
    }

    public GameObject RandomEvent()
    {
        int randomNum = Random.Range(0, eventsLeft.Count);
        GameObject myEvent = Instantiate(eventsLeft[randomNum], new Vector2(transform.position.x, transform.position.y), transform.rotation);
        if (myEvent.GetComponent<Event>() != null && myEvent.GetComponent<Event>().repeatable == false) eventsLeft.RemoveAt(randomNum);
        return myEvent;
    }

    public void NewNormalDoors()
    {
        StartCoroutine(Slide("up", "door", true));
    }

    public void NewDoorsInstant()
    {
        currObject = Instantiate(doorObject);;
        currObject.transform.position = new Vector2(0f, 0f);
    }


    IEnumerator Slide(string dir, string type, bool shouldBeActive)
    {

        GameObject newDoorObject;
        if (type == "door")
            newDoorObject = Instantiate(doorObject);
        else
            newDoorObject = Instantiate(battleManagerObjectToPick);

        if (dir == "right")
        {
            newDoorObject.transform.position = new Vector2(12.5f, 0f);
            while (currObject.transform.position.x > -12.3f)
            {
                currObject.transform.position = new Vector2(currObject.transform.position.x - slideSpeed,
                    currObject.transform.position.y);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x - slideSpeed,
                    newDoorObject.transform.position.y);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (dir == "left")
        {
            newDoorObject.transform.position = new Vector2(-12.5f, 0f);
            while (currObject.transform.position.x < 12.3f)
            {
                currObject.transform.position = new Vector2(currObject.transform.position.x + slideSpeed,
                    currObject.transform.position.y);
                currObject.transform.Find("DoorsCanvas").transform.position =
                    new Vector2(currObject.transform.position.x + slideSpeed, currObject.transform.position.y);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x + slideSpeed,
                    newDoorObject.transform.position.y);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else
        {
            newDoorObject.transform.position = new Vector2(0f, 22f);
            while (currObject.transform.position.y >= -22f)
            {
                currObject.transform.position = new Vector2(currObject.transform.position.x,
                    currObject.transform.position.y - slideSpeed * 2);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x,
                    newDoorObject.transform.position.y - slideSpeed * 2);
                if (newDoorObject.transform.position.y < 0f)
                {
                    newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x, 0f);
                }

                yield return new WaitForSeconds(0.02f);
            }
        }

        Destroy(currObject);
        active = shouldBeActive;
        currObject = newDoorObject;
        currObject.transform.position = new Vector2(0f, 0f);
    }

    IEnumerator Slide(string dir, string type, bool shouldBeActive, GameObject myEvent)
    {
        GameObject newDoorObject;
            if(type == "door")
                newDoorObject = Instantiate(doorObject);
            else
                newDoorObject = Instantiate(battleManagerObjectToPick);

        if (dir == "right") {
            newDoorObject.transform.position = new Vector2(12.5f, 0f);
            while (currObject.transform.position.x > -12.3f)
            { 
                currObject.transform.position = new Vector2(currObject.transform.position.x - slideSpeed, currObject.transform.position.y);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x - slideSpeed, newDoorObject.transform.position.y);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (dir == "left")
        {
            newDoorObject.transform.position = new Vector2(-12.4f, 0f);
            while (currObject.transform.position.x < 12.3f)
            { 
                currObject.transform.position = new Vector2(currObject.transform.position.x + slideSpeed, currObject.transform.position.y);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x + slideSpeed, newDoorObject.transform.position.y);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else
        {
            newDoorObject.transform.position = new Vector2(0f, 21.9f);
            while (currObject.transform.position.y >= -22f)
            {
                currObject.transform.position = new Vector2(currObject.transform.position.x, currObject.transform.position.y - slideSpeed * 2);
                newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x, newDoorObject.transform.position.y - slideSpeed * 2);
                if (newDoorObject.transform.position.y < 0f)
                { 
                    newDoorObject.transform.position = new Vector2(newDoorObject.transform.position.x, 0f);
                }
                yield return new WaitForSeconds(0.02f);
            }
        }
        Destroy(currObject);
        active = shouldBeActive;
        currObject = newDoorObject;
        currObject.transform.position = new Vector2(0f, 0f);
        myEvent.SetActive(true);
    }
}