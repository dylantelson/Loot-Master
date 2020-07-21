using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCircle : MonoBehaviour
{
    // Start is called before the first frame update

    public float spinSpeed;
    public bool fade = false;
    SpriteRenderer mySprite;

    void Start()
    {
        mySprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        gameObject.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        if(fade)
        {
            mySprite.color = new Color(mySprite.color.r, mySprite.color.b, mySprite.color.g, mySprite.color.a - 0.012f);
        }
    }
}
