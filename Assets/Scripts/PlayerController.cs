/* PlayerController.cs
 * -------------------------------
 * Authors:
 *      - Jay Ganguli
 *      - 
 *      - 
 * 
 * Last edited: 2021-02-18
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 500f;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float bounceForce = 100f;
    [SerializeField] private LayerMask whatIsEnemy;
    private float enemyBounceFrames = 0; // number of frames after bouncing off an enemy where the player can input a jump to gain extra height

    // the animator is in a child component to account for the sprite offset when flipping
    [SerializeField] private Animator anim;
    [SerializeField] private Text scoreText;

    private Rigidbody2D rb;
    private bool isGrounded;

    private int score;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        isGrounded = GroundCheck();

        if (horizontalMove == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else if (horizontalMove > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // face right
            anim.SetBool("isRunning", true);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // face left
            anim.SetBool("isRunning", true);
        }



        // jump code
        if (isGrounded)
        {
            anim.SetBool("isJumping", false);

            if (Input.GetAxis("Jump") > 0)
            {
                rb.AddForce(new Vector2(0.0f, jumpForce));
                isGrounded = false;
                anim.SetBool("isJumping", true);
            }
        }
        else if (enemyBounceFrames > 0)
        {
            enemyBounceFrames--;

            if (Input.GetAxis("Jump") > 0)
            {
                rb.AddForce(new Vector2(0f, bounceForce));
                enemyBounceFrames = 0;
            }
        }
        else if (rb.velocity.y < 0 && EnemyBounceCheck())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0.0f, jumpForce));
            anim.SetTrigger("bounce");
            // set an amount of time where the player can boost their jump
            enemyBounceFrames = 3;
        }

        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "smallGem")
        {
            score += 1;
            scoreText.text = score.ToString();
            Destroy(collision.gameObject);
        }
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, whatIsGround);
    }

    private bool EnemyBounceCheck()
    {
        // might make it a seperate radius value later
        return Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, whatIsEnemy);
    }
}
