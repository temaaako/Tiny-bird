using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float speed = 5f;

    private Animator animator;
    public float jumpForce = 10f;
    public AudioSource runningSound;
    public Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private bool isRunning = false;
    private bool rotationMode = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }

        if (horizontalInput == 0f && isRunning && !rotationMode)
        {
            animator.SetBool("Running", false);
            runningSound.Stop();
            isRunning = false;
        }

        if (horizontalInput != 0f && !isRunning)
        {
            animator.SetBool("Running", true);
            runningSound.Play();
            isRunning = true;
        }

        //transform.Translate(new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0));
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotationMode = SwitchRotationMode();
        }

        if (rotationMode)
        {
            transform.Rotate(Vector3.back * horizontalInput * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += new Vector3(horizontalInput * speed * Time.deltaTime, 0, 0);
        }

        RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down);


        DrawGizmos();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            animator.SetBool("Flying", true);
        }
    }

    private bool SwitchRotationMode()
    {

        if (rotationMode)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Flip()
    {
        // Изменяем значение флага разворота
        facingRight = !facingRight;

        // Переворачиваем спрайт по оси X
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
        
    void Jump()
    {
        float angleInRadians = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float sinAngle = Mathf.Sin(angleInRadians);
        float cosAngle = Mathf.Cos(angleInRadians);
        Vector2 jumpVector = new Vector2(sinAngle, cosAngle).normalized;
        Debug.Log(jumpVector);
        rb.velocity = jumpVector * jumpForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // The collision belongs to the ground layer
            Debug.Log("Collision belongs to ground layer");
        }
    }

    private void DrawGizmos()
    {
        float angleInRadians = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float sinAngle = Mathf.Sin(angleInRadians);
        float cosAngle = Mathf.Cos(angleInRadians);
        Vector2 jumpVector = new Vector2(sinAngle, cosAngle).normalized;

        Debug.DrawLine(transform.position, transform.position+new Vector3(jumpVector.x,jumpVector.y), Color.red);

    }
}
