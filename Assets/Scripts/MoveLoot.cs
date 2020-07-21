using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLoot : MonoBehaviour
{
    private bool canMove = false;

    public Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        if(FindObjectOfType<BattleManager>().GiveItem() == true) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
        {
            if (gameObject.GetComponent<RectTransform>().anchoredPosition.y < targetPos.y - 2f)
                gameObject.GetComponent<RectTransform>().anchoredPosition  =
                    Vector2.Lerp(gameObject.GetComponent<RectTransform>().anchoredPosition, targetPos, 10.0f * Time.fixedDeltaTime);
            else canMove = false;
        }
    }

    public void Move(Vector2 targetPos)
    {
        this.targetPos = targetPos;
        canMove = true;
    }
}
