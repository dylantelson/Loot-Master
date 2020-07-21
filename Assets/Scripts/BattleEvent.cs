using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEvent : MonoBehaviour
{
    public Button choice1Button;

    public Text choice1Text;

    public Text eventText;
    public string eventString;

    public string decision1;

    public string location;

    DungeonManager myDungeonManager;
    BattleManager myBattleManager;

    public string enemyToFight;

    void Start()
    {
        myDungeonManager = FindObjectOfType<DungeonManager>();
        myBattleManager = FindObjectOfType<BattleManager>();
        choice1Text.text = decision1;
        eventText.text = eventString;
    }

    public string GetLocation() { return location; }

    public void Clicked()
    {
        myBattleManager.BattleStart(enemyToFight);
        Destroy(gameObject);
    }
}
