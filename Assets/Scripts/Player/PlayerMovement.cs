using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    //Gameplay Variable
    GameManagerScript gm;

    private float moveSpeed = 265f;
    private float jumpForce = 365f;
    private float invincibleTime = 2;
    private bool alive = true;

    //Variable algorithmique
    public float flashDelay = 0.15f;
    private float currentMoveSpeed;
    public float speedFactor = 1f;
    public float direction = 1.0f;

    //Variable d'état (l'état du joueur)
    private bool inCinematic;
    private bool isInvincible;
    public bool isGrounded;

    private PlayerPowers powersManager;
    
    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    public LayerMask collisionLayers;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    public GameObject GameManager;

    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;

    private void Awake() {
        gm = FindObjectOfType<GameManagerScript>();
        if (!gm) {
            GameObject gmObject = Instantiate(GameManager);
            gmObject.GetComponent<GameManagerScript>().SetSaveFileId(-1, false);
        }
        gm = FindObjectOfType<GameManagerScript>();
        
        SpawnAt(gm.spawnNumber);

        animator = GetComponent<Animator>();
        if (gm.needAwakeAnimation) {
            StartCinematic();
            animator.SetTrigger("Awake");
            gm.needAwakeAnimation = false;
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        powersManager = GetComponent<PlayerPowers>();
        if (gm.Health > 0) {
            animator.SetBool("Alive", true);
        }
        else {
            animator.SetBool("Alive", false);
            alive = false;
        }
    }

    private void StartCinematicFromAnimation() {
        StartCinematic(false);
    }

    private void DisableAnimator() {
        animator.enabled = false;
        StopAllCoroutines();
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    
    public void StartCinematic(bool _forceIdle = false, bool _isInvincible = true) {
        inCinematic = true;
        isInvincible = _isInvincible;
        if (_forceIdle) {
            animator.SetBool("ForceIdle", true);
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("yVelocity", 0f);
            animator.SetBool("Jumping", false);
        }
    }
    public void ExitCinematic() {
        isInvincible = false;
        inCinematic = false;
        animator.SetBool("ForceIdle", false);
    }

    public bool IsInCinematic() {
        return inCinematic;
    }

    public void SpawnAt(int spawnNumber) {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject spawnSystem;
        if (spawnNumber == -1) {
            RestingPoint rp = FindObjectOfType<RestingPoint>();
            transform.position = rp.transform.position;
        } else {
            foreach (GameObject element in rootGameObjects) {
                if (element.name == "SpawnSystem") {
                    spawnSystem = element;
                    Warp warp = spawnSystem.transform.Find("Warp" + (int)spawnNumber).GetComponent<Warp>();
                    transform.position = warp.spawnPoint.position;
                }
            }
        }
        
    }

    void Update()
    {
        if (!alive) return;
        currentMoveSpeed = moveSpeed * speedFactor;

        if (inCinematic) return;


        horizontalMovement = Input.GetAxis("Horizontal") * currentMoveSpeed;
        if (horizontalMovement > 0) {
            direction = 1;
        } else if (horizontalMovement < 0) {
            direction = -1;
        }

        FlipSprite();

        UpdateAnimator();
    }

    void FixedUpdate() {
        if (!alive) {
            rb.velocity = Vector3.zero;
            return;
        };
        //Check Ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

        if (powersManager.IsBlockingControl() == 2) {
            if (powersManager.NullifyVelocity()) {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        } else {
            MovePlayer();
        }

    }

    void MovePlayer(){
        horizontalMovement *= Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        if (powersManager.NeedJump()){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int damage) {
        if (!alive) return;
        if (!isInvincible) {
            gm.Health -= damage;
            if (gm.Health <= 0) {
                animator.SetBool("Alive", false);
                alive = false;
            }
            animator.SetTrigger("Hurt");
            StartCoroutine(Invincibility());
        }
        
    }

    private void FlipSprite() {
        //Flip Sprite
        if (direction == 1) {
            spriteRenderer.flipX = false;
        }
        else {
            spriteRenderer.flipX = true;
        }
    }

    

    

    private void UpdateAnimator() {
        float currentSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("Jumping", !isGrounded);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawWireSphere(floorCheck.position, floorCheckRadius);
        //Gizmos.color = Color.gray;   
        //Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
        //Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
    }

    

    
    private IEnumerator InvincibilityFlash() {
        while (isInvincible) {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(flashDelay);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(flashDelay);
        }
        
    }

    private IEnumerator Invincibility() {
        isInvincible = true;
        StartCoroutine(InvincibilityFlash());
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    
}