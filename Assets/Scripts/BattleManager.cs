using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// COUNTERATTACK IDEA:
///     Occasionally, as soon as you attack, the enemy blocks it and counterattacks. A new slider quickly appears, with only one white bar, and the closer you hit the less damage you take (and you can counterattack yourself) but you need fast reflexes
/// </summary>
public class BattleManager : MonoBehaviour {

	List<string> typesOfLoot = new List<string> { "warrior helmet", "sword", "glock", "fishing rod"};

	public GameObject rect;
	public GameObject whiteRect;

	public Canvas enemyCanvas;
	public GameObject enemy;

    public Canvas playerCanvas;

    public GameObject damageText;
	public GameObject LootScreen;
	public GameObject doorScreen;

	public GameObject currentLoot;
	public GameObject lootObject;

    public DungeonManager myDungeonManager;

	public float maxEnemyHealth = 100;
	private float currentEnemyHealth;
	public float maxPlayerHealth = 100;
	private float currentPlayerHealth;
	public float str = 11;
	public float dex = 10;

	private float damagedone;

	public Slider enemyHealthBar;

    public Slider playerHealthBar;

    public GameObject rectHolder;
    public GameObject circlePrefab;
    public GameObject circle;
    public GameObject arrowPrefab;
    public GameObject arrow;

    public GameObject enemyCircle;
    public GameObject enemySquare;

    public GameObject defenseGameObject;
    public GameObject defenseInstance;

    public string currWeapon = "sword";

    public int enemyProjectiles;
    public bool isDefending = false;

	List<GameObject> rectangleList = new List<GameObject>();
	List<string> lootList = new List<string>();

	GameObject whitey = null;
	private float whiteyRight = 1;

    public bool playerDead = false;

    public string enemyToFight;

    public GameObject CombatPopupCanvas;

    public GameObject lootSquare;
    
    Dictionary<string,string> translator = new Dictionary<string, string>
    {
        { "brunohelmet", "bruno"},
        { "armlessboy", "armless boy"}
    };
    
	// Use this for initialization
	void Start () {

        myDungeonManager = FindObjectOfType<DungeonManager>();
        playerCanvas.enabled = false;
        enemyCanvas.enabled = false;

		currentEnemyHealth = maxEnemyHealth;
		currentPlayerHealth = maxPlayerHealth;

		enemyHealthBar.value = currentEnemyHealth / maxEnemyHealth;
        playerHealthBar.value = currentPlayerHealth / maxPlayerHealth;
    }

    public void StartBow()
    {
        if (circle == null)
        {
            circle = Instantiate(circlePrefab);
            circle.transform.position = new Vector2(0f, 1.5f);
        }
    }

    public void PlayerDamaged(int damage)
    {
        currentPlayerHealth -= damage;
        playerHealthBar.value = currentPlayerHealth / maxPlayerHealth;
        GameObject damageTextInstance = Instantiate(damageText, transform.position, Quaternion.identity);

        damageTextInstance.transform.SetParent(playerCanvas.transform, false);

        if (damage >= 10)
        {
            damageTextInstance.GetComponent<Text>().text = "-" + damage + "!";
        }
        else
        {
            damageTextInstance.GetComponent<Text>().text = "-" + damage;
        }
        damageTextInstance.GetComponent<Text>().fontSize = (int)Mathf.Round(50f + damage * 5.5f);
        Destroy(damageTextInstance, 1f);

        if (currentPlayerHealth <= 0 && !playerDead)
        {
            playerDead = true;
            ClearEverything();
            myDungeonManager.GameOver();
            //StartCoroutine(BattleOver());
            //following: placeholder for testing

        }
    }

    public void AttackStageBow()
    {
        if (circle == null)
        {
            StartBow();
            return;
        }
        arrow = Instantiate(arrowPrefab);
        arrow.transform.position = new Vector2(0f, -4f);
    }

    public void AttackStageSword()
    {
        rectHolder.transform.position = new Vector2(0f, 0f);

        if (rectangleList.Count > 0f)
        {
            for (int i = 0; i < rectangleList.Count; i++)
            {
                int shouldBeEmpty = Random.Range(0, 2);
                if (shouldBeEmpty == 0) rectangleList[i].transform.localScale = new Vector3(0.23f, 0f, transform.localScale.z);
                else rectangleList[i].transform.localScale = new Vector3(0.23f, Random.Range(0.1f, 1.7f), transform.localScale.z);
            }
        }
        else
        {
            for (int i = 0; i < 14; i++)
            {
                rectangleList.Add(Instantiate(rect, transform.position, Quaternion.identity));
                rectangleList[i].transform.parent = rectHolder.transform;
                rectangleList[i].transform.position = new Vector2(-2.4f + i * 0.37f, -4);
                rectangleList[i].transform.localScale = new Vector3(0.23f, Random.Range(0.1f, 1.7f), transform.localScale.z);
            }
        }

        if (whitey != null)
        {
            Destroy(whitey);
        }
        whitey = Instantiate(whiteRect, transform.position, Quaternion.identity);
        whitey.transform.parent = transform;
        whitey.transform.position = new Vector2(-2.4f, -4);

        whiteyRight = 1;
    }

    public void AttackStage() {
        enemyCanvas.enabled = true;
        playerCanvas.enabled = false;
        if (currWeapon == "sword")
            enemy.SetActive(true);
        if (currWeapon == "sword") AttackStageSword();
        else if (currWeapon == "bow") AttackStageBow();
	}

    public void BattleStart(string enemyToFight)
    {
        this.enemyToFight = enemyToFight;
        string enemyName = enemyToFight;
        //Set exceptions here, such as changing brunohelmet to bruno
        if(translator.ContainsKey(enemyToFight)) enemyName = translator[enemyToFight];
            enemyCanvas.transform.Find("EnemyName").GetComponent<Text>().text = enemyName;
        StartCoroutine("BattleStartPopup");
    }

    IEnumerator BattleStartPopup()
    {
        SpriteRenderer mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>(); 
        mySpriteRenderer.sprite = Resources.Load <Sprite>("combatscreen");
        //mySpriteRenderer.color = Color.white;
        GameObject combatPopup = Instantiate(CombatPopupCanvas, transform.position, transform.rotation);
        Sprite enemySprite = Resources.Load <Sprite>(enemyToFight);
        combatPopup.transform.Find("EnemyImagePopup").GetComponent<Image>().sprite = enemySprite;
        enemy.GetComponent<Image>().sprite = enemySprite;
        yield return new WaitForSeconds(2f);
        Destroy(combatPopup);
        BattleStart2();
    }

	public void BattleStart2() {
        
        //Reset all battle vars to defaults, set up battle
        int randomClass = Random.Range(0, 1);
        if (randomClass == 0) currWeapon = "sword";
        else currWeapon = "bow";

        Reset();
        enemyCanvas.enabled = true;
        playerCanvas.enabled = false;
        if (currWeapon == "sword") {
            rectHolder = new GameObject();
            rectHolder.transform.parent = transform;
            AttackStage();
        }
        else if (currWeapon == "bow")
        {
            circlePrefab = Resources.Load <GameObject>("circle" + enemyToFight);
            circle = Instantiate(circlePrefab);
            circle.transform.position = new Vector2(0f, 1.5f);
            enemy.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            whitey.GetComponent<Animator>().SetTrigger("Fade");
            enemy.GetComponent<Animator>().SetTrigger("Death");
            enemyCanvas.GetComponent<Animator>().SetTrigger("FadeCanvas");
            StartCoroutine(BattleOver());
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentEnemyHealth > 0f && !isDefending)
        {
            if (currWeapon == "sword")
            {
                whiteyRight = -1;
                float closestint = 1000f;
                int closestGameObjectIndex = 2;
                for (int i = 0; i < rectangleList.Count; i++)
                {
                    if (Mathf.Abs(whitey.transform.position.x - rectangleList[i].transform.position.x) < closestint)
                    {
                        closestGameObjectIndex = i;
                        closestint = Mathf.Abs(whitey.transform.position.x - rectangleList[i].transform.position.x);
                    }
                }
                whitey.GetComponent<Animator>().SetTrigger("Fade");
                damagedone = Mathf.Round(rectangleList[closestGameObjectIndex].transform.localScale.y * 30);
                SetDamageText();
            }

            else if (currWeapon == "bow")
            {
                if (arrow != null && arrow.GetComponent<Arrow>().hitCircle == false)
                {
                    return;
                }
                AttackStage();
                arrow.GetComponent<Arrow>().isActive = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (whitey != null || currWeapon == "bow")
        {
            if (whitey != null)
            {
                if (whiteyRight == 1)
                {
                    if (whitey.transform.position.x > 2.4f)
                    {
                        whiteyRight = 0;
                    }
                    else
                    {
                        whitey.transform.position = new Vector2(whitey.transform.position.x + 0.3f, whitey.transform.position.y);
                    }
                }
                else if (whiteyRight == 0)
                {
                    if (whitey.transform.position.x < -2.4f)
                    {
                        whiteyRight = 1;
                    }
                    else
                    {
                        whitey.transform.position = new Vector2(whitey.transform.position.x - 0.3f, whitey.transform.position.y);
                    }
                }
            }
        }
    }

    //public void SetDamageText(float damage)
    //{
    //    damagedone = damage;
    //    GameObject damageTextInstance = Instantiate(damageText, transform.position, Quaternion.identity);
    //    damageTextInstance.transform.SetParent(myCanvas.transform, false);

    //    if (damagedone >= 10)
    //    {
    //        damageTextInstance.GetComponent<Text>().text = "-" + damagedone + "!";
    //    }
    //    else
    //    {
    //        damageTextInstance.GetComponent<Text>().text = "-" + damagedone;
    //    }
    //    damageTextInstance.GetComponent<Text>().fontSize = (int)Mathf.Round(50f + damagedone * 5.5f);
    //    Destroy(damageTextInstance, 1f);
    //    currentEnemyHealth -= damagedone;
    //    enemyHealthBar.value = currentEnemyHealth / maxEnemyHealth;

    //    if (currentEnemyHealth > 0)
    //    {
    //        if (currWeapon == "sword")
    //            AttackStage();
    //    }
    //    else
    //    {
    //        enemy.GetComponent<Animator>().SetTrigger("Death");
    //        myCanvas.GetComponent<Animator>().SetTrigger("FadeCanvas");
    //        StartCoroutine(BattleOver());

    //    }
    //}
    public void SetDamageDone()
    {
        damagedone = 10f;
    }

    public void SetDamageDone(float damage)
    {
        damagedone = Mathf.Ceil(damage);
    }

    public void SetDamageText()
    {
        GameObject damageTextInstance = Instantiate(damageText, transform.position, Quaternion.identity);
        damageTextInstance.transform.SetParent(enemyCanvas.transform, false);

        if (damagedone >= 10)
        {
            damageTextInstance.GetComponent<Text>().text = "-" + damagedone + "!";
        }
        else
        {
            damageTextInstance.GetComponent<Text>().text = "-" + damagedone;
        }
        damageTextInstance.GetComponent<Text>().fontSize = (int)Mathf.Round(50f + damagedone * 5.5f);
        Destroy(damageTextInstance, 1f);
        currentEnemyHealth -= damagedone;
        enemyHealthBar.value = currentEnemyHealth / maxEnemyHealth;

        if (currentEnemyHealth > 0)
        {
            if (currWeapon == "sword")
            {
                //AttackStage();
                SwitchToDefense();
            }
        }
        else
        {
            enemy.GetComponent<Animator>().SetTrigger("Death");
            enemyCanvas.GetComponent<Animator>().SetTrigger("FadeCanvas");
            StartCoroutine(BattleOver());

        }
    }

    public void SwitchToDefense()
    {
        StartCoroutine(SwitchToDefenseCoroutine());
    }

    public void ClearEverything()
    {
        if (circle != null) Destroy(circle);
        if (defenseInstance != null) Destroy(defenseInstance);
        playerCanvas.enabled = false;
        enemyCanvas.enabled = false;
    }

    public IEnumerator SwitchToDefenseCoroutine()
    {

        isDefending = true;
        if (currWeapon == "bow") PushCircle();
        yield return new WaitForSeconds(0.4f);
        playerCanvas.enabled = true;
        enemy.SetActive(false);
        enemyCanvas.enabled = false;
        StartCoroutine(HideRect());
        SetDefense();
        yield break;
    }

    IEnumerator HideRect()
    {
        if (rectHolder == null) yield break;
        while (rectHolder.transform.position.y > -9f)
        {
            rectHolder.transform.transform.Translate(Vector2.down * 1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PushCircle()
    {
        if (circle == null) return;
        circle.GetComponent<SpinCircle>().fade = true;
        Rigidbody2D circleBody = circle.GetComponent<Rigidbody2D>();
        circleBody.bodyType = RigidbodyType2D.Dynamic;
        circleBody.gravityScale = 1.9f;
        circleBody.GetComponent<PolygonCollider2D>().isTrigger = true;
        circleBody.AddForce(new Vector2(60f, 290f));
    }

    IEnumerator BattleOver() {
        if(currWeapon == "sword")
        {
            StartCoroutine(HideRect());
        }
        else
        {
            PushCircle();
            Reset();
        }
        yield return new WaitForSeconds (0.4f);
        
        foreach(EnemyProjectile projectile in FindObjectsOfType<EnemyProjectile>()) {
            Destroy(projectile.gameObject);
        }

        Destroy(enemyCanvas.gameObject);
        Destroy(playerCanvas.gameObject);
        StartCoroutine(ShowMoney());
		//StartCoroutine(ShowChest ());
	}

    public void Reset()
    {
        if(currWeapon == "bow" && circle != null)
        {
            foreach (Arrow a in FindObjectsOfType<Arrow>())
            {
                Destroy(a.gameObject);
            }
            arrow = null;
            Destroy(circle);
        }
    }

    public void SetDefense()
    {
        Reset();
        isDefending = true;
        defenseInstance = Instantiate(defenseGameObject);
        StartCoroutine(CreateProjectiles());
    }

    IEnumerator CreateProjectiles()
    {
        int i = 0;
        while (i < 25f && !playerDead)
        {
            Instantiate(enemyCircle).transform.position = new Vector2(Random.Range(-4f, 4f), 5.6f);
            i++;
            enemyProjectiles++;
            yield return new WaitForSeconds(0.18f);
        }
        while(enemyProjectiles > 0f)
        {
            yield return new WaitForSeconds(1f);
        }
        isDefending = false;
        Destroy(defenseInstance);
        if(!playerDead)
            AttackStage();
    }


    //	public void SlideIn () {
    //		StartCoroutine (SlideInCo ());
    //	}
    //
    //	IEnumerator SlideInCo() {
    //		while (transform.position.x > 0) {
    //			transform.position = new Vector2 (transform.position.x - 0.2f, transform.position.y);
    //			yield return new WaitForSeconds (0.01f);
    //		}
    //		AttackStage ();
    //	}

    IEnumerator ShowMoney()
    {
        LootScreen = Instantiate (LootScreen);
        LootScreen.transform.parent = transform;
        LootScreen.transform.localScale = new Vector2 (0f, 0f);
        
        Text lootGainedText = GameObject.Find("LootGainedText").GetComponent<Text>();
        int moneyToGive = GiveMoney();
        lootGainedText.text = "" + moneyToGive;

        GameObject treasureImage = GameObject.Find("Treasure");
        treasureImage.transform.localScale = new Vector2 (0f, 0f);
        
        while (LootScreen.transform.localScale.y < 1f) {
            LootScreen.transform.localScale = new Vector2 (LootScreen.transform.localScale.x + 0.1f, LootScreen.transform.localScale.y + 0.1f);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.5f);
        while (treasureImage.transform.localScale.y < 1f) {
            treasureImage.transform.localScale = new Vector2 (treasureImage.transform.localScale.x + 0.14f, treasureImage.transform.localScale.y + 0.14f);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.6f);
        while (moneyToGive > 0)
        {
            if (moneyToGive >= 3)
            {
                myDungeonManager.GiveGold(3);
                moneyToGive -= 3;
                lootGainedText.text = "" + moneyToGive;
            }
            else
            {
                myDungeonManager.GiveGold(1);
                moneyToGive -= 1;
                lootGainedText.text = "" + moneyToGive;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.7f);

        CanvasScaler lootGainedCanvasScaler = GameObject.Find("LootGained").GetComponent<CanvasScaler>();
        while (lootGainedCanvasScaler.scaleFactor > 0.03f)
        {
            lootGainedCanvasScaler.scaleFactor -= 0.05f;
            yield return new WaitForFixedUpdate();
        }
        lootGainedCanvasScaler.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        
        LootScreen.GetComponent<LootScreen>().ShowChest();
    }

    int GiveMoney()
    {
        return Random.Range(0, 300);
    }

	public void ShowLoot() {
		StartCoroutine (ShowLootCo ());
	}

	IEnumerator ShowLootCo()
    {
        yield return new WaitForSeconds(1.05f);
		lootList.Clear ();
        
		//for (int i = 0; i < Random.Range(2, 4); i++) {
		//	lootList.Add (typesOfLoot [Random.Range (0, typesOfLoot.Count)]);
		//}
        
        //changed so only 1 item can be given
        lootList.Add (typesOfLoot [Random.Range (0, typesOfLoot.Count)]);
		yield return new WaitForSeconds (0.2f);

		//For now operating under assumption I won't need to reference these, and the whole LootScreen will fade out then be deleted or delete all children.
		//Change this if I end up needing to delete/fade these specifically.
		for (int i = 0; i < lootList.Count; i++) {
            GameObject currentLootBackground = Instantiate(lootSquare, new Vector2(0f, 0f), Quaternion.identity);
            currentLootBackground.transform.SetParent(LootScreen.transform.Find("Canvas").transform);
            currentLootBackground.transform.localScale = new Vector3(1f,1f,1f);
            currentLootBackground.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
            currentLoot = currentLootBackground.transform.Find("lootimage").gameObject;
            //currentLoot.transform.SetParent(lootSquare.transform);
            //currentLoot.transform.position = new Vector2(0, 0);
            //currentLoot.transform.position = Vector2.Lerp (currentLoot.transform.position, new Vector2(-0.75f * (lootList.Count - 1) + i * 1.5f, 2.5f), 1.0f * Time.fixedDeltaTime);
            currentLootBackground.GetComponent<MoveLoot>().Move((new Vector2(-1.5f * (lootList.Count - 1) + i * 3f, 200f)));
			currentLoot.GetComponent<Image>().sprite = Resources.Load(lootList[i], typeof(Sprite)) as Sprite;
        }

        yield return new WaitForSeconds(0.7f);
        LootScreen.GetComponent<LootScreen>().ShowInventory();
    }

    public bool GiveItem()
    {
        if (myDungeonManager.inventory.Count < 9)
        {
            myDungeonManager.inventory.Add(lootList[0]);
            //LootScreen.ReceivedItem();
            //GameObject.Find("LootButton(Clone)").SetActive(false);
            LootScreen myLootScreenScript = LootScreen.GetComponent<LootScreen>();
            myLootScreenScript.ShowInventory();
            myLootScreenScript.ShowCheckButton();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GiveLoot(string loot)
    {
        // if (loot == "helmet")
        // {
        //     myDungeonManager.GiveHelmet();
        //     return;
        // }
        // if (loot == "key")
        // {
        //     myDungeonManager.GiveKey();
        //     return;
        // }
        // if (loot == "sword")
        // {
        //     myDungeonManager.GiveSword();
        //     return;
        // }
        if (loot == "coins")
        {
            myDungeonManager.GiveGold(100);
            return;
        }
        else
        {
            myDungeonManager.GiveLoot(loot);
        }

    }

    public void Continue() {

        myDungeonManager.NewDoorsInstant();
        Destroy(gameObject);
    }

	IEnumerator SlideDown() {
		while (transform.position.y >= -10.8f) {
			transform.position = new Vector2 (transform.position.x, transform.position.y - 0.2f);
			yield return new WaitForSeconds (0.05f);
		}
		Destroy (this.gameObject);
	}
}