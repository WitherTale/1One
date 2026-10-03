using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] private float JumpForce = 10f;
    public bool isGrounded;
    public bool isSlide;
    public bool isDead = false;

    public Vector2 BoxSize = new Vector2(0.8f, 1.3f);
    public Vector2 normalSize = new Vector2(0.9f, 1.3f);
    public Vector2 normalOffSet = new Vector2(0.02f, -0.4f);
    public Vector2 crouchSize = new Vector2(0.4f, 0.5f);
    public Vector2 crouchOffSet = new Vector2(-0.02f, -0.8f);
    public float slideYOffset = -0.25f;
    private Rigidbody2D rb;
    private BoxCollider2D box;
    private Animator anim;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return;

        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing)
            return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapBox(groundCheck.position, BoxSize, 0f, groundLayer);
        }
        if (anim != null) anim.SetBool("isGrounded", isGrounded);

       
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && isGrounded)
        {
            ExecuteJump();
        }



        if ((Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && isGrounded)
        {
            box.size = crouchSize;
            box.offset = crouchOffSet;
            isSlide = true;

            if (anim != null)
            {
                anim.SetBool("isSlide", true);

                
                anim.transform.localPosition = new Vector3(0f, slideYOffset, 0f);
            }
        }
        else 
        {
            box.size = normalSize;
            box.offset = normalOffSet;
            isSlide = false;

            if (anim != null)
            {
                anim.SetBool("isSlide", false);
                anim.transform.localPosition = Vector3.zero;
            }
        }
    }
    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        isGrounded = false;
        if (anim != null) anim.SetBool("isGrounded", false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (groundCheck != null) Gizmos.DrawWireCube(groundCheck.position, BoxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Fall"))
        {
            TriggerDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Fall"))
        {
            TriggerDie();
        }
    }

    private void TriggerDie()
    {
        if (isDead) return;
        isDead = true;


        if (anim != null) anim.SetTrigger("Die");


        if (sr != null) sr.color = Color.red;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 4. Phát tiếng Chết
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDie();

        if (GameManager.Instance != null) GameManager.Instance.GameOver();
    }

    public void ResetPlayerState()
    {
        isDead = false;
        isSlide = false;
        isGrounded = true;

   
        if (box != null)
        {
            box.size = normalSize;
            box.offset = normalOffSet;
        }

      
        if (sr != null) sr.color = Color.white;

        transform.rotation = Quaternion.identity;

       
        if (anim != null)
        {
            anim.ResetTrigger("Die");
            anim.SetBool("isSlide", false);
            anim.SetBool("isGrounded", true);

            anim.transform.localPosition = Vector3.zero;
            anim.transform.localRotation = Quaternion.identity;

            if (gameObject.activeInHierarchy)
            {
                anim.Rebind();
                anim.Update(0f);
                anim.Play("Player", 0, 0f); 
            }
        }

       
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}