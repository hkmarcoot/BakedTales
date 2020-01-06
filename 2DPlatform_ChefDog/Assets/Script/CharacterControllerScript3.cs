using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterControllerScript3 : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float jumpForce = 25f;
    public float jumpPushForce = 10f;
    public float maxJumpSpeed = 11f;
    public float dashForceY = 1.0f;
    public float maxDashSpeed = 10f;
    bool facingRight = true;
    private int facingDirection;

    Rigidbody2D rb2D;
    Animator anim;
    AudioSource AS;

    //sets up the grounded stuff
    bool grounded = false;
    bool touchingWall = false;
    bool allowDash = false;
    public Transform groundCheck;
    public Transform wallCheck;
    float groundRadius = 0.2f;
    float wallTouchRadius = 0.2f;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public AudioClip jumpSound;
    public Slider slider;

    private int beforeJump;
    private int before2Jump;

    bool startDash;
    private float time = 0.0f;

    bool jumpButton;
    bool dashButton;

    //double jump
    bool doubleJump = false;

    float move;

    // Use this for initialization
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        //if (grounded == true)
        //{
        //    Debug.Log("Grounded is true");
        //} else if (grounded == false) {
        //    Debug.Log("Grounded is false");
        //}

        touchingWall = Physics2D.OverlapCircle(wallCheck.position, wallTouchRadius, whatIsWall);
        anim.SetBool("Ground", grounded);

        if (grounded)
        {
            doubleJump = false;
            allowDash = true;
            before2Jump = 0;
        }

        if (touchingWall)
        {
            grounded = false;
            doubleJump = false;
        }

        anim.SetBool("Wall", touchingWall);

        anim.SetFloat("vSpeed", rb2D.velocity.y);



        //move = Input.GetAxis("Horizontal");

        move = Input.GetAxis("Horizontal");
        if (move == 0)
        {
            move = slider.value;
            if (move < -0.1f)
                move = -1;
            else if (move > 0.1f)
                move = 1;
            else
                move = 0;
        }

        anim.SetFloat("Speed", Mathf.Abs(move));

        if (grounded == true && touchingWall == false)
        {
            Debug.Log("Controller A");
            rb2D.velocity = new Vector2(move * maxSpeed, rb2D.velocity.y);
        }

        if (grounded == false && touchingWall == false)
        {
            Debug.Log("Controller B");
            if (beforeJump == 0) // jump from ground
            {
                Debug.Log("B0");
                rb2D.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
            }
            if (beforeJump == 1) //jump from air
            {
                if (startDash == false)
                {
                    if(before2Jump == 2)
                    {
                        Debug.Log("No double jump in wall jump");
                    }
                    if (before2Jump == 0) //before before Jump is ground
                    {
                        Debug.Log("B1-1");
                        rb2D.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
                    }
                    
                }else if(startDash == true){
                    Debug.Log("B1-2");
                }
            }
            if (beforeJump == 2) //jump from wall
            {
                Debug.Log("B2");
                //Mark down before before jump is wall
                before2Jump = 2;
                //Not allow double jump when wall jump
                doubleJump = true;
            }

        }

        if (grounded == true && touchingWall == true)
        {
            //Not possible to have Controller C because touchingWall is true when grounded is false
            Debug.Log("Controller C");
            rb2D.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
        }

        if (grounded == false && touchingWall == true)
        {
            Debug.Log("Controller D");
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
        }


        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !facingRight)
        {
            // ... flip the player.
            Flip();
        }// Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && facingRight)
        {
            // ... flip the player.
            Flip();
        }

        if (facingRight)
        {
            facingDirection = 1;
        }else {
            facingDirection = -1;
        }

        if (startDash == true && !grounded)
        {
            Debug.Log("Added Dash Speed");
            //Debug.Log("Using Controller B");

            anim.SetBool("Dash", true);

            //For reference
            //rb2d.AddForce(new Vector2(dashDirection * dashForceX, dashForceY), ForceMode2D.Impulse);
            //rb2d.velocity = new Vector2(Mathf.Min(rb2d.velocity.x, maxDashSpeed), Mathf.Min(rb2d.velocity.y, maxDashSpeed));

            time = time + Time.fixedDeltaTime;
            //Debug.Log(time);
            if (time <= 0.06f)
            {
                rb2D.velocity = new Vector2(facingDirection * maxDashSpeed * 5, maxDashSpeed / 10);
            }

            if (time > 0.06f && time < 0.6f)
            {
                rb2D.velocity = new Vector2(facingDirection * maxDashSpeed, maxDashSpeed / 10);
            }

            if (time == 0.6f)
            {
                rb2D.AddForce(new Vector2(0, -dashForceY), ForceMode2D.Impulse);
            }

            if (time > 0.6f)
            {
                rb2D.velocity = new Vector2(facingDirection * maxDashSpeed, rb2D.velocity.y);
            }

        }
        else if (startDash == false && !grounded)
        {
            anim.SetBool("Dash", false);
        }
    }

    public void JumpButtonPress ()
    {
        jumpButton = true;
    }

    public void JumpButtonRelease ()
    {
        jumpButton = false;
    }

    public void DashButtonPress ()
    {
        dashButton = true;
    }

    public void DashButtonRelease ()
    {
        dashButton = false;
    }

    void Update()
    {
        if (dashButton == true)
        {
            if (allowDash == true)
            {
                startDash = true;
                allowDash = false;
                Debug.Log("You pressed K");
            }
        }

        if (dashButton == false)
        {
            startDash = false;
            //Debug.Log("K key was released.");
            time = 0.0f;
        }

        if (jumpButton == true && grounded == true)
        {
            //the 0 means the last contact is ground
            beforeJump = 0;
        }

        if (jumpButton == true && grounded == false)
        {
            //the 1 means the last contact is not ground, so it is double jump
            beforeJump = 1;
        }

        if (jumpButton == true && grounded == false && touchingWall == true)
        {
            //the 2 means the last contact is not ground and is touching wall, so it is wall jump
            beforeJump = 2;
        }

        // If the jump button is pressed and the player is grounded then the player should jump.
        if ((grounded || !doubleJump) && jumpButton == true)
        {
            anim.SetBool("Ground", false);

            //y-axis jumpforce 
            rb2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            AS.PlayOneShot(jumpSound);
            jumpButton = false;
            //Debug.Log(rigidbody2D.velocity.y);
            
            //rb2D.velocity = new Vector2(rb2D.velocity.x,
            //               Mathf.Min(rb2D.velocity.y, maxJumpSpeed));

            if (!doubleJump && !grounded)
            {
                doubleJump = true;
                //Debug.Log("Old Controller");
                //rb2D.velocity = new Vector2(move * maxSpeed, rb2D.velocity.y);
                anim.SetTrigger("Jump");
            }

            if (touchingWall)
            {
                WallJump();
            }

        } else 
            jumpButton = false;

        //if (touchingWall && jumpButton == true)
        //{
        //    
        //   WallJump();
        //   
        //}

    }

    void WallJump()
    {
        Debug.Log("Performing Wall Jump");
        //no y-axis jumpforce because jumping already have y-axis force
        rb2D.AddForce(new Vector2(facingDirection * (-1) * jumpPushForce, 0));
        //move = facingDirection * (-1);
        
    }


    void Flip()
    {

        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        //Multiply the player's x local cale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void LateUpdate()
    {

        anim.ResetTrigger("Jump");
    }
}