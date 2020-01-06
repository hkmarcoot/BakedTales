using UnityEngine;
using System.Collections;

public class CharacterControllerScript : MonoBehaviour
{

    public float maxSpeed = 10f;
    bool facingRight = true;
    private int facingDirection;

    Rigidbody2D rb2D;
    Animator anim;

    //sets up the grounded stuff
    bool grounded = false;
    bool touchingWall = false;
    public Transform groundCheck;
    public Transform wallCheck;
    float groundRadius = 0.2f;
    float wallTouchRadius = 0.2f;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public float jumpForce = 700f;
    public float jumpPushForce = 10f;
    public float maxJumpSpeed = 11f;

    private int beforeJump;

    //double jump
    bool doubleJump = false;

    // Use this for initialization
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        }

        if (touchingWall)
        {
            grounded = false;
            doubleJump = false;
        }

        anim.SetFloat("vSpeed", rb2D.velocity.y);



        float move = Input.GetAxis("Horizontal");

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
                Debug.Log("B1");
                rb2D.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
            }
            if (beforeJump == 2) //jump from wall
            {
                Debug.Log("B2");
            }

        }

        if (grounded == true && touchingWall == true)
        {
            Debug.Log("Controller C");
            rb2D.velocity = new Vector2(move * maxSpeed, Mathf.Min(rb2D.velocity.y, maxJumpSpeed));
        }

        if (grounded == false && touchingWall == true)
        {
            Debug.Log("Controller D");
            //rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);
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
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            //the 0 means the last contact is ground
            beforeJump = 0;
        }

        if (Input.GetButtonDown("Jump") && grounded == false)
        {
            //the 1 means the last contact is not ground, so it is double jump
            beforeJump = 1;
        }

        if (Input.GetButtonDown("Jump") && grounded == false && touchingWall == true)
        {
            //the 2 means the last contact is not ground and is touching wall, so it is wall jump
            beforeJump = 2;
        }

        // If the jump button is pressed and the player is grounded then the player should jump.
        if ((grounded || !doubleJump) && Input.GetButtonDown("Jump"))
        {
            anim.SetBool("Ground", false);

            //y-axis jumpforce 
            rb2D.AddForce(new Vector2(0, jumpForce));
            //Debug.Log(rigidbody2D.velocity.y);
            //rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,
            //               Mathf.Min(rigidbody2D.velocity.y, maxJumpSpeed));

            if (!doubleJump && !grounded)
            {
                doubleJump = true;
            }
        }

        if (touchingWall && Input.GetButtonDown("Jump"))
        {

            WallJump();
            
        }

    }

    void WallJump()
    {
        Debug.Log("Performing Wall Jump");
        //no y-axis jumpforce because jumping already have y-axis force
        rb2D.AddForce(new Vector2(facingDirection * (-1) * jumpPushForce, 0));
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
}