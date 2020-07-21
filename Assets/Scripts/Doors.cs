using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour {


	public string typeOfObject;

	public string direction;

	private DungeonManager myDungeon;
	private BattleManager myBattleManager;
	public GameObject Star;
	private GameObject myStar;

	private bool opened = false;

	// Use this for initialization
	void Start () {
		if (typeOfObject == "door") {
			myDungeon = FindObjectOfType<DungeonManager> ();
		} else if (typeOfObject == "chest") {
			myBattleManager = FindObjectOfType<BattleManager> ();
			// myStar = Instantiate (Star, transform.position, Quaternion.identity);
			// myStar.transform.parent = transform.parent;
			// myStar.transform.localScale = new Vector2 (0f, 0f);
			// StartCoroutine (stupidStar ());
		}
	}

	IEnumerator stupidStar() {
		yield return new WaitForSeconds (0.05f);
		myStar.transform.localScale = new Vector2 (1f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		if (typeOfObject == "door") {
			myDungeon.DoorClicked (direction);
		} else if (typeOfObject == "chest") {
			if (!opened) {
				GetComponent<Animator> ().SetTrigger ("OpenedChest");
				//myStar.transform.parent = transform;
				myBattleManager.ShowLoot ();
				opened = true;
			} else {
				myBattleManager.Continue ();
			}
		}
	}
}
