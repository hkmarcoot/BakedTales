using UnityEngine;
using System.Collections;

public class PlayerControllerWDash : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float jumpForce = 30f;
    public float maxJumpSpeed = 13;
    public bool airControl = true;
    public bool allowdoubleJump = true;
    public LayerMask whatIsGround;
    public AudioClip jumpSound;

    Transform groundCheck;
    const float groundedRadius = .2f;
    bool grounded;
    Animator anim;
    Rigidbody2D rb2d;
    AudioSource AS;
    bool facingRight = true;
    float move;
    bool startJump;
    bool doubleJumped;

    bool startDash;
    float dashDirection;
    //public float dashForceX = 10f;
    public float dashForceY = 1.0f;
    public float maxDashSpeed = 10f;

    private float time = 0.0f;

    bool touchingWall = false;
    public Transform wallCheck;
    float wallTouchRadius = 0.2f;
    public LayerMask whatIsWall;
    public float wallJumpForce = 700f;
    public float jumpPushForce = 10f;

    void Start()
    {
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        AS = GetComponent<AudioSource>();
        startJump = false;
        doubleJumped = false;

        startDash = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            startJump = true;

        if (Input.GetKeyDown(KeyCode.K))
        {
            startDash = true;
            Debug.Log("You pressed K");
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            startDash = false;
            Debug.Log("K key was released.");
            time = 0.0f;
        }

    }

    void FixedUpdate()
    {

        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position,
                                 groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                doubleJumped = false;

                //reset startDash
                startDash = false;
            }
        }
        anim.SetBool("Ground", grounded);

        touchingWall = Physics2D.OverlapCircle(wallCheck.position, wallTouchRadius, whatIsWall);

        if (touchingWall)
        {
            grounded = false;
            doubleJumped = false;
        }

        move = Input.GetAxis("Horizontal");

        if ((grounded || airControl)  && startDash == false)
        {
            anim.SetFloat("Speed", Mathf.Abs(move));
            Debug.Log("Using Controller A1");
            rb2d.velocity = new Vector2(move * maxSpeed, rb2d.velocity.y);
            //Try set all in one line
            //rb2d.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2d.velocity.y, maxJumpSpeed));
        }

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        //Add dashdirection
        if (facingRight)
            dashDirection = 1;
        else if (!facingRight)
            dashDirection = -1;
        



        if ((grounded || (!doubleJumped && allowdoubleJump)) && startJump)
        {
            anim.SetBool("Ground", false);

            Debug.Log("Added jump force");
            rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AS.PlayOneShot(jumpSound);
            startJump = false;

            if (!grounded && !doubleJumped)
            {
                doubleJumped = true;
                //Another controller when moving in air
                //Debug.Log("Using Controller C");
                //rb2d.velocity = new Vector2(move * maxSpeed, rb2d.velocity.y);
                anim.SetTrigger("Jump");

            }
        }
        else
            startJump = false;

        if (startDash == false)
        {
            Debug.Log("Using Controller A2");

            rb2d.velocity = new Vector2(rb2d.velocity.x,
                           Mathf.Min(rb2d.velocity.y, maxJumpSpeed));
        }

        if (touchingWall && Input.GetButtonDown("Jump"))
        {
            //Debug.Log("Performing Wall Jump");
            
            WallJump();
            Flip();
        }

        //adding Dash

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
                rb2d.velocity = new Vector2(dashDirection * maxDashSpeed * 5, maxDashSpeed / 10);
            }
                
            if(time > 0.06f && time < 0.6f)
            { 
                rb2d.velocity = new Vector2(dashDirection * maxDashSpeed, maxDashSpeed / 10);
            }

            if(time == 0.6f)
            {
                rb2d.AddForce(new Vector2(0 , -dashForceY), ForceMode2D.Impulse);
            }
            
        } else if (startDash == false && !grounded)
        {
            anim.SetBool("Dash", false);
        }
        //

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void WallJump()
    {
        Debug.Log("Performing Wall Jump");
        rb2d.AddForce(new Vector2(dashDirection * (-1) * jumpPushForce, wallJumpForce));
        //rb2d.velocity = new Vector2(dashDirection * maxDashSpeed, maxDashSpeed);

    }

    void LateUpdate()
    {
        
        anim.ResetTrigger("Jump");
    }

}
