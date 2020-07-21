using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    public Transform center;
    private Rigidbody2D myRigidBody;

    private bool hasHit = false;

    public float baseSpeed = 9f;
    public float speed = 9f;

    public int damageToGive = 10;
    public float damageRandomness = 0.4f;
    public float speedRandomness = 0.2f;

    private SpriteRenderer mySR;

    private BattleManager myBattleManager;

    // Start is called before the first frame update
    void Start()
    {
        //center = Instantiate(new GameObject(), gameObject.transform, true).transform;
        //center.position = new Vector2(0f, 0f);
        center = FindObjectOfType<Player>().gameObject.transform;
        center.position = center.transform.position;
        gameObject.transform.LookAt(center);
        gameObject.transform.right = center.position - transform.position;
        myRigidBody = GetComponent<Rigidbody2D>();
        mySR = GetComponent<SpriteRenderer>();
        myBattleManager = FindObjectOfType<BattleManager>();

        speed = (int)(Random.Range(baseSpeed - speedRandomness * 10, baseSpeed + speedRandomness * 10)); 
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasHit)
            myRigidBody.AddForce(transform.right * speed);
        if (transform.position.y < -30f && hasHit)
        {
            myBattleManager.enemyProjectiles--;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        if (collision.gameObject.name.Substring(0, 6) == "Player")
        {
            //hasHit = true;
            myRigidBody.gravityScale = 1.4f;
            myBattleManager.PlayerDamaged((int)Random.Range(damageToGive - 10*damageRandomness, damageToGive + 10*damageRandomness));
            //FindObjectOfType<DungeonManager>().Trigger("GameOver");
        }
        else
        {
            if (myBattleManager.playerDead) return;
            center = collision.transform;
            center.position = center.transform.position;
            gameObject.transform.LookAt(center);
            gameObject.transform.right = center.position - transform.position;
            //myRigidBody.AddForce(transform.right * baseSpeed * -15f);

            //hasHit = true;
            myRigidBody.gravityScale = 1.4f;
        }
    }
}
