using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
            transform.localPosition = pos;
            transform.localScale = scale;
        }
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
}
