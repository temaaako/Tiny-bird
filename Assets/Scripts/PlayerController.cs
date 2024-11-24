using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float speed = 5f;
    public float jumpForce = 10f;
    public AudioSource runningSound;
    public Rigidbody2D rb;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _facingRight = true;
    private bool _isRunning = false;
    private bool _rotationMode = false;
    private bool _isGrounded = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        MoveCharacter();
        if (_isGrounded)
        {
            _animator.SetBool("Flying", false);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        
    }

    void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (_rotationMode)
        {
            transform.Rotate(Vector3.back * horizontalInput * rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }

        RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down);
        _isGrounded = raycast.collider != null && raycast.distance < 0.5f;
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Flip character sprite based on movement direction
        if (horizontalInput > 0 && !_facingRight || horizontalInput < 0 && _facingRight)
        {
            Flip();
        }

        // Handle running animation and sound
        if (Mathf.Abs(horizontalInput) > 0f && !_isRunning)
        {
            _animator.SetBool("Running", true);
            if (!runningSound.isPlaying) runningSound.Play();
            _isRunning = true;
        }
        else if (Mathf.Abs(horizontalInput) == 0f && _isRunning)
        {
            _animator.SetBool("Running", false);
            runningSound.Stop();
            _isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _rotationMode = !_rotationMode;
        }
    }

    void Jump()
    {
        _animator.SetBool("Flying", true);
        rb.linearVelocity = Vector2.up * jumpForce;
        _isGrounded = false;
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isGrounded = true;
        }
    }
}
