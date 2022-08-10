using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpVelocity;
    public LayerMask wallMask;
    public float bounceVelocity;
    public float gravity;
    public LayerMask floorMask;
    public Vector2 velocity;
    private bool walk, walkLeft, walkRight, jump;
    public enum PlayerState
    {
        jumping,idle,walking,bouncing
    }
    private PlayerState playerState = PlayerState.idle;
    private bool grounded = false;
    private bool bounce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPosition();
        UpdateAnimationStates();
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
        if (jump && playerState != PlayerState.jumping) 
        {
            playerState = PlayerState.jumping;
            velocity = new Vector2(velocity.x, jumpVelocity);

        }
        if (bounce && playerState != PlayerState.bouncing)
        {
            playerState = PlayerState.bouncing;
            velocity = new Vector2(velocity.x, bounceVelocity);

        }
        if (playerState == PlayerState.bouncing)
        {
            pos.y += velocity.y*Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;

            //velocity = new Vector2(velocity.x, bounceVelocity);

        }
        if (playerState == PlayerState.jumping)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }
        if(velocity.y <= 0)
        {
            pos = CheckFloorRays(pos);
        }
        if(velocity.y >=0)
        {
            pos = CheckCellingRays(pos);
        }
        transform.localPosition = pos;
        transform.localScale = scale;

    }
    Vector3 CheckFloorRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 1f);
        Vector2 originMiddle = new Vector2(pos.x , pos.y - 1f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y - 1f);
        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.x * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.x * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.x * Time.deltaTime, floorMask);
        if (floorLeft.collider != null || floorMiddle.collider != null || floorRight.collider != null)
        {
            RaycastHit2D hitRay = floorRight;
            if (floorLeft)
            {
                hitRay = floorLeft;
            }
            else if (floorMiddle)
            {
                hitRay = floorMiddle;
            }
            else if(floorRight) 
            {
                hitRay = floorRight;
            }
            if (hitRay.collider.tag=="Enemy")
            {
                hitRay.collider.GetComponent<EnemyAI>().Crush();
                bounce = true;
                
            }
            playerState = PlayerState.idle;
            grounded = true;
            velocity.y = 0;
            pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1;
            
        }
        else
        {
            if(playerState!= PlayerState.jumping)
            {
                fall();
            }
        }
        return pos;
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
    void UpdateAnimationStates()
    {
        if(grounded && !walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
        if (grounded && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }
        if (playerState==PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }
    Vector3 CheckCellingRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y + 1f);
        Vector2 originRight = new Vector2(pos.x + 0.5f -0.2f, pos.y + 1f);
        RaycastHit2D ceilLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.x * Time.deltaTime, floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.x * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast(originRight, Vector2.up, velocity.x * Time.deltaTime, floorMask);
        if(ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null )
        {
            RaycastHit2D hitRay = ceilLeft;
            if (ceilLeft)
            {
                hitRay = ceilLeft;
            }
            else if(ceilMiddle)
            {
                hitRay = ceilMiddle;
            }
            else
            {
                hitRay = ceilRight;
            }
            pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - 1;
            fall();
        }
        return pos;
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
    void fall() 
    {
        velocity.y = 0;

        playerState = PlayerState.jumping;
        bounce = false;
        grounded = false;
    }
}
