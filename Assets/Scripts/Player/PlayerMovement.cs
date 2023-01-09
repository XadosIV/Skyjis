using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    //Gameplay Variable
    GameManagerScript data;

    private float moveSpeed = 265f;
    private float jumpForce = 365f;
    private float invincibleTime = 2;

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
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public GameObject GameManager;

    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;

    private void Awake() {
        data = FindObjectOfType<GameManagerScript>();
        if (!data) {
            GameObject gmObject = Instantiate(GameManager);
            gmObject.GetComponent<GameManagerScript>().SetSaveFileId(-1, false);
        }
        data = FindObjectOfType<GameManagerScript>();
        
        SpawnAt(data.spawnNumber);

        animator = GetComponent<Animator>();
        if (data.needAwakeAnimation) {
            StartCinematic();
            animator.SetTrigger("Awake");
            data.needAwakeAnimation = false;
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        powersManager = GetComponent<PlayerPowers>();

        data.UpdateUI();
        if (data.health > 0) {
            animator.SetBool("Alive", true);
        }
        else {
            animator.SetBool("Alive", false);
        }


    }
    
    public void StartCinematic() {
        inCinematic = true;
        isInvincible = true;
    }
    public void ExitCinematic() {
        isInvincible = false;
        inCinematic = false;
    }

    public bool IsInCinematic() {
        return inCinematic;
    }

    public void SpawnAt(int spawnNumber) {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject spawnSystem;
        foreach (GameObject element in rootGameObjects) {
            if (element.name == "SpawnSystem") {
                spawnSystem = element;
                Transform warp = spawnSystem.transform.Find("Warp"+(int)spawnNumber);
                transform.position = warp.Find("SpawnPoint").position;
            }
        }
    }

    void Update()
    {
        if (data.health <= 0) return;
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
        if (data.health <= 0) {
            rb.velocity = Vector3.zero;
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
        if (data.health <= 0) return;
        if (!isInvincible) {
            data.health -= damage;
            if (data.health <= 0) {
                animator.SetBool("Alive", false);
                this.enabled = false;
            }
            animator.SetTrigger("Hurt");
            data.UpdateUI();
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