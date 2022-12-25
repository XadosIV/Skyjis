using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    //Gameplay Variable
    GameManagerScript data;

    //Variable algorithmique
    public float flashDelay = 0.15f;
    private float currentMoveSpeed;
    private float jumpTimeCounter;
    private bool dashPressed;
    private bool canDash = true;
    private bool canHit = true;
    public float direction = 1.0f;

    //Variable d'état (l'état du joueur)
    private bool inCinematic;
    private bool isInvincible;
    private bool isDashing;
    private bool isJumping;
    private bool isGrounded;
    private bool isCrouching;
    private bool isDoubleJumping;
    private bool isChoosingTeleportation;
    private bool isTeleporting;

    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D standingCollider;
    [SerializeField] private Collider2D crouchingCollider;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform attackPointLeft;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float floorCheckRadius;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform teleportIndicator;
    [SerializeField] private Transform teleportIndicatorSprite;
    [SerializeField] private Transform teleportIndicatorTruePosition;

    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;

    private void Awake() {
        data = FindObjectOfType<GameManagerScript>();
        SpawnAt(data.spawnName);
    }

    private void Start() {
        data.UpdateHearts();
        teleportIndicatorSprite.transform.localPosition = new Vector3(data.teleportRange, 0, 0);
        animator.SetBool("Alive", true);
        
        if (data.needAwakeAnimation) {
            startCinematic();
            animator.SetTrigger("Awake");
            data.needAwakeAnimation = false;
        }
        
    }
    
    public void startCinematic() {
        inCinematic = true;
    }
    public void exitCinematic() {
        inCinematic = false;
    }

    public void SpawnAt(string spawnName) {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject spawnSystem;
        foreach (GameObject element in rootGameObjects) {
            if (element.name == "SpawnSystem") {
                spawnSystem = element;
                Transform spawn = spawnSystem.transform.Find(spawnName);
                transform.position = spawn.position;
            }
        }
    }

    void Update()
    {
        if (inCinematic) return;

        FlipSprite();

        CheckInputs();

        UpdateAnimator();
    }

    void FixedUpdate() {
        //Check Ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);
        
        if (isDashing) return;

        if (isChoosingTeleportation) {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (dashPressed) {
            dashPressed = false;
            StartCoroutine(Dash(direction));
            return;
        }

        if (isGrounded) {
            isDoubleJumping = false;
        }

        MovePlayer();
    }

    void MovePlayer(){
        horizontalMovement *= Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        if (isJumping){
            rb.velocity = new Vector2(rb.velocity.x, data.jumpForce * Time.fixedDeltaTime);
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
            data.UpdateHearts();
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

    private void CheckInputs() {
        horizontalMovement = Input.GetAxis("Horizontal") * currentMoveSpeed;
        if (horizontalMovement > 0) {
            direction = 1;
        }
        else if (horizontalMovement < 0){
            direction = -1;
        }
        if (isDashing || isTeleporting) return;
        
        MeleeHitInput();

        DashInput();

        JumpInput();

        CrouchInput();

        TeleportInput();

        //Get Horizontal Movement
    }

    private void MeleeHitInput() {
        if (isCrouching) return;
        if (!canHit) return;
        if (Input.GetButtonDown("MeleeHit")) {
            animator.SetTrigger("MeleeHit");
            StartCoroutine(HandleMeleeHit());
        }
    }

    private void MeleeHit() {
        Transform attackPoint = direction == 1 ? attackPointRight : attackPointLeft;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, data.attackRange, enemyLayers);
        bool somethingHit = false;
        foreach (Collider2D collider in hitColliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy.health > 0) {
                enemy.TakeDamage(data.attackDamage, new Vector2(data.knockback * direction, 0));
                somethingHit = true;
            }
        }
        if (somethingHit) {
            rb.AddForce(new Vector2(data.knockback * direction * -1 * data.selfKnockback, 0));
        }
    }

    private void DashInput() {
        if (!data.hasDash) return;
        // Cant Dash if Crouch
        if (isCrouching || isChoosingTeleportation) return;
        if (Input.GetButtonDown("Dash") && canDash) {
            dashPressed = true;
        }
    }

    private void JumpInput() {
        //stop jumping if button release
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
        }

        //Dont handle jump when dashing or teleporting
        if (isChoosingTeleportation) {
            isJumping = false; // stop jump if dashing or teleporting
            return;
        }

        //Cant Jump if crouching or dashing
        if (Input.GetButtonDown("Jump") && !isCrouching) {

            //Jump Only if grounded or is a doublejump
            if (isGrounded || (data.hasDoubleJump && !isDoubleJumping)) {
                isJumping = true;
                jumpTimeCounter = data.jumpTime; //reset counter

                if (data.hasDoubleJump && !isDoubleJumping) { //detect double jump
                    isDoubleJumping = true;
                }
            }
        }

        //Detect jump handle to jump higher
        if (Input.GetButton("Jump")) {
            if (jumpTimeCounter > 0 && isJumping) {
                jumpTimeCounter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }
    }

    private void CrouchInput() {
        //Dont handle Crouch if dashing or jumping
        if (dashPressed || isJumping || isChoosingTeleportation) return;

        //Crouch if grounded 
        if (Input.GetAxis("Crouch") > 0 && isGrounded) {
            isCrouching = true;
        } else if (isCrouching) {

            //If trying to uncrouch, check collisions

            if (!Physics2D.OverlapCircle(floorCheck.position, floorCheckRadius, collisionLayers)) {
                isCrouching = false;
            }
        }

        UpdateCrouchHitbox();
    }

    private void TeleportInput() {
        // Cant Teleport if Dashing or crouching
        if (!data.hasTeleport) return;
        if (dashPressed || isCrouching) return;
        

        if (Input.GetButton("Teleport")) {

            int teleportMovement = 0;
            if (Input.GetButton("IndicatorLeft")) {
                teleportMovement += 2;
            }
            if (Input.GetButton("IndicatorRight")) {
                teleportMovement -= 2;
            }


            isChoosingTeleportation = true;
            Time.timeScale = 0.2f;

            teleportIndicator.Rotate(0, 0, teleportMovement);
            teleportIndicatorSprite.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z);

            SpriteRenderer sr2 = teleportIndicatorTruePosition.GetComponent<SpriteRenderer>();
            Vector3? position = TeleportGetPosition();
            sr2.enabled = true;
            if (position != null) {
                sr2.color = new Color(0f, 1f, 0f, 0.5f);
                teleportIndicatorTruePosition.position = (Vector3) TeleportGetPosition();
                teleportIndicatorTruePosition.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z);
            }
            else {
                teleportIndicatorTruePosition.position = teleportIndicatorSprite.position;
                teleportIndicatorTruePosition.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z);
                sr2.color = new Color(1f, 0f, 0f, 0.5f);
            }

            
        }
        else {
            isChoosingTeleportation = false;
            Time.timeScale = 1;
            SpriteRenderer sr2 = teleportIndicatorTruePosition.GetComponent<SpriteRenderer>();
            sr2.enabled = false;
        }

        if (Input.GetButtonUp("Teleport")) {
            Vector3? position = TeleportGetPosition();
            if (position != null && !isTeleporting) {
                StartCoroutine(Teleport((Vector3)position));
                //transform.position = (Vector3) position;
            }
        }
    }

    private Vector3? TeleportGetPosition() {


        Transform gc = teleportIndicatorSprite.Find("GroundCheck");
        Transform fc = teleportIndicatorSprite.Find("FloorCheck");

        bool midAvailable = !TeleportInWalls(transform.position, teleportIndicatorSprite.position, collisionLayers);
        bool botAvailable = !TeleportInWalls(groundCheck.position, gc.position, collisionLayers);
        bool topAvailable = !TeleportInWalls(floorCheck.position, fc.position, collisionLayers);

        if (midAvailable && botAvailable && topAvailable) { //Cas classique
            return teleportIndicatorSprite.position;
        }
        else if (topAvailable && midAvailable && !botAvailable) { //Cas avec mid => TP Sprite sur l'extrémité
            return fc.position;
        }
        else if (botAvailable && midAvailable && !topAvailable) { //Cas avec mid => TP Sprite sur l'extrémité
            return gc.position;
        }
        else if (topAvailable && !midAvailable && !botAvailable) { //Cas only top => TP bas du sprite sur le top
            Vector3 topPosition = groundCheck.position - new Vector3(0, groundCheckRadius, 0);
            Vector2 translation = fc.position - topPosition;

            Vector3 position = new Vector3(transform.position.x+translation.x, transform.position.y+translation.y, 0);
            return position;
        }
        else if (botAvailable && !midAvailable && !topAvailable) { //Cas only bot => Tp haut du sprite sur le bas
            Vector3 botPosition = floorCheck.position + new Vector3(0, floorCheckRadius, 0);
            Vector2 translation = gc.position - botPosition;

            Vector3 position = new Vector3(transform.position.x + translation.x, transform.position.y + translation.y, 0);
            return position;
        }
        else { // Autres cas : aucun dispo / seulement mid / top et bot sans mid
            return null;
        }
    }

    private bool ValideCoordinate(Vector3 position) {
        Vector2[] boundaries = data.getBoundaries();
        if (position.x < boundaries[0].x || boundaries[1].x < position.x) return false;
        if (position.y < boundaries[0].y || boundaries[1].y < position.y) return false; 
        return true;
    }

    private bool TeleportInWalls(Vector2 _position, Vector2 _finalPosition, LayerMask _layer) {
        if (ValideCoordinate(_finalPosition)) {
            return RaycastIntersection(_position, _finalPosition, _layer) % 2 == 1;
        } else {
            return true;
        }

    }

    private int RaycastIntersection(Vector2 _position, Vector2 _finalPosition, LayerMask _layer) {
        float distance = Vector2.Distance(_finalPosition, _position);
        Vector2 direction = _finalPosition - _position;
        direction.Normalize();

        int intersection = 0;
        distance = Mathf.Abs(distance);
        RaycastHit2D hit = Physics2D.Raycast(_position, direction, distance, _layer);
        //Debug.DrawRay(_position, direction * distance, Color.green, 3, false);

        while (hit.collider != null) {
            distance -= hit.distance;
            _position = hit.point;

            _position = new Vector2(_position.x + direction.x*0.001f, _position.y + direction.y * 0.001f);

            intersection += 1;
            if (intersection > 20) {
                break;
            }

            hit = Physics2D.Raycast(_position, direction, distance, _layer);
            /*if (intersection % 2 == 1) {
                Debug.DrawRay(_position, direction * distance, Color.red, intersection*3+3, false);
            }
            else {
                Debug.DrawRay(_position, direction * distance, Color.green, intersection*3+3, false);
            }*/

        }
        return intersection;
    }

    private void UpdateCrouchHitbox() {
        //Change Collider if crouch & update MoveSpeed
        if (isCrouching) {
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;
            currentMoveSpeed = data.moveSpeed * 0.7f;
        } else {
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
            currentMoveSpeed = data.moveSpeed;
        }
    }

    private void UpdateAnimator() {
        float currentSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("Crouching", isCrouching);
        animator.SetBool("Jumping", !isGrounded);

    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawWireSphere(floorCheck.position, floorCheckRadius);
        //Gizmos.color = Color.gray;   
        //Gizmos.DrawWireSphere(attackPointLeft.position, data.attackRange);
        //Gizmos.DrawWireSphere(attackPointRight.position, data.attackRange);
    }

    private IEnumerator Dash(float direction)
    {
        isDashing = true;
        canDash = false;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(data.dashingDistance * direction, 0f), ForceMode2D.Impulse);
        float gravity = rb.gravityScale;
        rb.gravityScale = 0f;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(data.dashingTime);
        trailRenderer.emitting = false;
        rb.gravityScale = gravity;
        isDashing = false;
        yield return new WaitForSeconds(data.dashingCooldown);
        canDash = true;
    }

    private IEnumerator Teleport(Vector3 position) {
        isTeleporting = true;
        animator.SetTrigger("Teleporting");
        float gravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(0.3f);
        transform.position = position;
        yield return new WaitForSeconds(0.3f);
        isTeleporting = false;
        rb.gravityScale = gravity;
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
        yield return new WaitForSeconds(data.invincibleTime);
        isInvincible = false;
    }

    private IEnumerator HandleMeleeHit() {
        canHit = false;
        yield return new WaitForSeconds(data.attackSpeed);
        canHit = true;
    }
}