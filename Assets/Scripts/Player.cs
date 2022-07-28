using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   public LayerMask wallMask;
    public Vector2 velocity;
    private bool walk, walkLeft, walkRight, jump;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPosition();
    }
    void UpdatePlayerPosition()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;
        if (walk)
        {
            if (walkLeft)
            {
                pos.x -= velocity.x * Time.deltaTime;
                scale.x = -1;
             
            }
        
            if (walkRight)
            {
            pos.x += velocity.x* Time.deltaTime;
            scale.x = 1;       
            }
            pos = CheckWallRays(pos, scale.x);

        }
        transform.localPosition = pos;
        transform.localScale = scale;

    }

    void CheckPlayerInput()
    {
        bool inputLeft = Input.GetKey(KeyCode.LeftArrow);
        bool inputRight = Input.GetKey(KeyCode.RightArrow);
        bool inputSpace = Input.GetKey(KeyCode.Space);
        walk = inputLeft || inputRight;
        walkLeft = inputLeft && !inputRight;
        walkRight = !inputLeft && inputRight;
        jump = inputSpace;
    }
    Vector3 CheckWallRays(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * .4f, pos.y + 1f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * .4f, pos.y );
        Vector2 originBottom = new Vector2(pos.x + direction * .4f, pos.y - 1f + 0.2f);
        RaycastHit2D wallTop = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);  
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if (wallTop.collider != null || wallMiddle.collider!=null || wallBottom.collider!= null)
        {
            pos.x -= velocity.x * Time.deltaTime * direction;
        }
        return pos;
    }

}
