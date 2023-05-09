using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {
    //Controleur
    GameManager gm;

    //Components
    public Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sr;

    //Components Scripts
    PlayerPowers powersManager;

    //Gameplay Variable
    float moveSpeed = 265f;
    float jumpForce = 365f;
    float invincibleTime = 2;

    //Variable algorithmique
    public LayerMask physicsLayers;
    float horizontalMovement;

    float flashDelay = 0.15f;
    public int direction = 1;
    Vector3 velocity = Vector3.zero;

    float attackDamage = 1;

    Dictionary<MonoBehaviour, float> attackEffect;
    Dictionary<MonoBehaviour, float> speedEffect;

    int blockActionId;
    Dictionary<int, bool[]> blockAction;

    //Variable d'état (l'état du joueur)
    //[SerializeField] private bool inCinematic;
    bool isInvincible;
    public bool isGrounded;


    // Position Points
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;



    //Debug variable
    public GameObject GameManager; //Instancie un GameManager s'il n'y en a pas.

    //Methodes
    void FindGameManager() {
        gm = FindObjectOfType<GameManager>();
        if (!gm) {
            GameObject gmObject = Instantiate(GameManager);
            gm = gmObject.GetComponent<GameManager>();

            // Le joueur ayant dû instancier le GameManager, on détecte un lancé depuis Unity, donc on utilise la sauvegarde de debug.
            gm.SetSaveFileId(-1, false);
        }
    }

    public void SpawnAt(int spawnNumber) { // Appelé pour placer le joueur à la position du warp
        if (spawnNumber == -1) {
            RestingPoint rp = FindObjectOfType<RestingPoint>();
            transform.position = rp.transform.position;
        }
        else {
            Warp[] warps = FindObjectsOfType<Warp>();
            foreach (Warp warp in warps) {
                int id = int.Parse(warp.name.ToLower().Split("p")[1]);
                if (id == spawnNumber) {
                    transform.position = warp.spawnPoint.position;
                }
            }
        }
    }

    void DisableAnimator() { //Appelé sur la frame de mort du joueur.
        animator.enabled = false;
        StopAllCoroutines();
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    public void StartCinematicAnimation() {
        blockActionId = StartCinematic();
    }

    public void ExitCinematicAnimation() {
        ExitCinematic(blockActionId);
    }

    bool IsAlive() {
        return gm.Health > 0;
    }

    public void AddSpeedFactor(MonoBehaviour script, float factor) {
        speedEffect.Add(script, factor);
    }   

    public void RemoveSpeedFactor(MonoBehaviour script) {
        speedEffect.Remove(script);
    }

    public void AddAttackFactor(MonoBehaviour script, float factor) {
        attackEffect.Add(script, factor);
    }

    public void RemoveAttackFactor(MonoBehaviour script) {
        attackEffect.Remove(script);
    }

    

    public int AddBlockAction(bool[] rule) {
        //rule = bool[5] => { horizontaux, attacks, jump, interaction, }
        int id = 0;
        if (blockAction.Count != 0) {
            List<int> keyList = new List<int>(blockAction.Keys);
            id = Mathf.Max(keyList.ToArray()) + 1;
        }

        blockAction.Add(id, rule);
        return id;
    }

    public void RemoveBlockAction(int id) {
        blockAction.Remove(id);
    }

    bool GetBlockAction(int index) {
        bool rule = false;
        foreach (bool[] blockRule in blockAction.Values) {
            if (index >= blockRule.Length) continue;
            if (blockRule[index]) {
                rule = true;
                break;
            }
        }
        return rule;
    }

    public bool CanMove() {
        return !GetBlockAction(0);
    }

    public bool CanAttacks() {
        return !GetBlockAction(1);
    }

    public bool CanJump() {
        return !GetBlockAction(2);
    }

    public bool CanInteract() {
        return !GetBlockAction(3);
    }
    public bool ForceIdle() {
        return GetBlockAction(4);
    }

    public int StartCinematic(bool _forceIdle = false) {
        bool[] rule = new bool[5] { true, true, true, true, _forceIdle};
        rb.velocity = Vector3.zero;

        return AddBlockAction(rule);
    }

    public void ExitCinematic(int id) {
        RemoveBlockAction(id);
    }

    void Awake() {
        FindGameManager();
        SpawnAt(gm.spawnNumber);

        attackEffect = new Dictionary<MonoBehaviour, float>();
        speedEffect = new Dictionary<MonoBehaviour, float>();
        blockAction = new Dictionary<int, bool[]>();

        //Get Components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        powersManager = GetComponent<PlayerPowers>();

        if (gm.needAwakeAnimation) {
            blockActionId = StartCinematic();
            animator.SetTrigger("Awake");
            gm.needAwakeAnimation = false;
        }
    }

    private void Start() {
        if (gm.Health > 0) {
            animator.SetBool("Alive", true);
        }
        else {
            animator.SetBool("Alive", false);
        }


    }



    /*private void StartCinematicFromAnimation() {
        StartCinematic(false);
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
    }*/

    float SpeedFactor() {
        float speedFactor = 1f;
        foreach (float factor in speedEffect.Values) {
            speedFactor *= factor;
        }
        return speedFactor;
    }

    float CurrentMoveSpeed() { 
        return moveSpeed * SpeedFactor();
    }

    float CurrentJumpBoost() {
        return jumpForce * SpeedFactor();
    }

    public float CurrentAttackFactor() {
        float attackFactor = 1f;
        foreach (float factor in attackEffect.Values) {
            attackFactor *= factor;
        }
        return attackFactor * attackDamage;
    }

    public int CalculateDamage(float damage) {
        return (int) (damage * CurrentAttackFactor());
    }


    void Update() {

        if (ForceIdle()) {
            animator.SetBool("ForceIdle", true);
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("yVelocity", 0f);
            animator.SetBool("Jumping", false);
        }
        else {
            animator.SetBool("ForceIdle", false);
            UpdateAnimator();

        }

        if (!IsAlive()) return;

        if (!CanMove()) return;

        horizontalMovement = Input.GetAxis("Horizontal") * CurrentMoveSpeed();
        if (horizontalMovement > 0) {
            direction = 1;
        }
        else if (horizontalMovement < 0) {
            direction = -1;
        }

        FlipSprite();

    }

    void FixedUpdate() {

        //Check Ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, physicsLayers);

        if (!CanMove()) {

            if (!powersManager.DontNullifyVelocity()) { rb.velocity = new Vector2(0, rb.velocity.y); };
        }
        else {
            MovePlayer();
        }

    }

    void MovePlayer() {
        horizontalMovement *= Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        if (powersManager.NeedJump()) {
            rb.velocity = new Vector2(rb.velocity.x, CurrentJumpBoost() * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int damage) {
        if (!isInvincible) {
            gm.Health -= damage;
            blockAction.Clear();
            if (gm.Health <= 0) {
                animator.SetBool("Alive", false);
            }
            animator.SetTrigger("Hurt");
            StartCoroutine(Invincibility());
        }

    }

    private void FlipSprite() {
        //Flip Sprite
        if (direction == 1) {
            sr.flipX = false;
        }
        else {
            sr.flipX = true;
        }
    }





    private void UpdateAnimator() {
        float currentSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("Jumping", !isGrounded);
    }

    private void OnDrawGizmos() {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawWireSphere(floorCheck.position, floorCheckRadius);
        //Gizmos.color = Color.gray;   
        //Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
        //Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
    }

    private IEnumerator InvincibilityFlash() {
        while (isInvincible) {
            sr.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(flashDelay);
            sr.color = new Color(1f, 1f, 1f, 1f);
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