using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public bool isActive = false;
    public bool hitCircle = false;
    public float speed;
    public BattleManager myBattleManager;

    public float baseDamage = 10f;

    public float randomness = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        myBattleManager = FindObjectOfType<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitCircle && isActive) transform.Translate(Vector3.up * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!hitCircle && collision.gameObject.name.Length >= 6 && collision.gameObject.name.Substring(0, 6) == "circle")
        {
            hitCircle = true;
            transform.parent = collision.transform;
            //myBattleManager.SetDamageText(10f);
            myBattleManager.SetDamageDone(Random.Range(baseDamage - randomness * 10, baseDamage + randomness * 10));
            myBattleManager.SetDamageText();
        }
        else if(collision.gameObject.name.Substring(0, 5) == "arrow" && myBattleManager.isDefending == false)
        {
            myBattleManager.SwitchToDefense();
        }
    }
}
