using System.Collections;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private GameManager gm;
    private PlayerMovement pm;

    public Transform teleportIndicator;
    public Transform teleportIndicatorSprite;
    public Transform teleportIndicatorTruePosition;
    public Transform groundCheck;
    public float groundCheckRadius;
    public Transform floorCheck;
    public float floorCheckRadius;

    public bool isTeleporting;
    private Rigidbody2D rb;
    private Animator animator;

    private float teleportRange = 4f;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        teleportIndicatorSprite.transform.localPosition = new Vector3(teleportRange, 0, 0);
    }

    public bool IsAvailable() {
        if (!gm.save.hasTeleport) return false;
        if (isTeleporting) return false;
        return true;
    }

    /*public int BlockControl() {
        if (isTeleporting) return 2; //On bloque les contrôles durant tout le processus de téléportation
        return 0;
    }*/

    public void Execute() {
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport() {
        isTeleporting = true;
        int id = pm.AddBlockAction(new bool[] { true, true, true, true, true });
        Time.timeScale = 0.2f; // Ralentissement du temps pour laisser choisir le joueur

        //Activation du sprite de choix visuel de la position
        SpriteRenderer sr2 = teleportIndicatorTruePosition.GetComponent<SpriteRenderer>();
        sr2.enabled = true;

        //Tant qu'il choisit sa téléportation
        while (Input.GetButton("Teleport")) {

            //Déplacement du sprite par le joueur
            float teleportMovement = 0;
            if (Input.GetButton("IndicatorLeft")) {
                teleportMovement += 2.5f;
            }
            if (Input.GetButton("IndicatorRight")) {
                teleportMovement -= 2.5f;
            }

            //Update Sprite Position
            teleportIndicator.Rotate(0, 0, teleportMovement);
            teleportIndicatorSprite.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z);
            
            //On récupère la position visuel du téléport
            Vector3? positionVisuel = TeleportGetPosition();
            
            if (positionVisuel != null) { // Si il y en a une
                sr2.color = new Color(0f, 1f, 0f, 0.5f); //On colorie le sprite en vert 
                teleportIndicatorTruePosition.position = (Vector3)positionVisuel; //On le place au bon endroit
                teleportIndicatorTruePosition.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z); //On annule sa rotation
            }
            else { // Si il n'y a pas de position valide
                teleportIndicatorTruePosition.position = teleportIndicatorSprite.position; // On met le sprite à la position du calcul
                teleportIndicatorTruePosition.localRotation = Quaternion.Euler(0, 0, -teleportIndicator.eulerAngles.z); //On annule sa rotation
                sr2.color = new Color(1f, 0f, 0f, 0.5f); // On le colorie en rouge
            }

            yield return null; //On attend la prochaine frame
        }

        //Une fois la touche relachée, on remet le jeu à la normal :
        Time.timeScale = 1; //remise du temps
        sr2.enabled = false; // on désactive le sprite visuel

        // On récupère la position demandé par le joueur
        Vector3? position = TeleportGetPosition();

        // Si elle est valide, on exécute l'animation et la téléportation.
        if (position != null) { 
            pm.RemoveBlockAction(id);
            id = pm.AddBlockAction(new bool[] { true, true, true, true, false });
            animator.SetTrigger("Teleporting");
            float gravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(0.3f);
            transform.position = (Vector3)position;
            yield return new WaitForSeconds(0.3f);
            rb.gravityScale = gravity;
        }
        isTeleporting = false;
        pm.RemoveBlockAction(id);
    }


    // Renvoie une position correcte ou nul à partir de la position du IndicatorSprite
    private Vector3? TeleportGetPosition() {

        Transform gc = teleportIndicatorSprite.Find("GroundCheck");
        Transform fc = teleportIndicatorSprite.Find("FloorCheck");

        LayerMask collisionLayers = GetComponent<PlayerMovement>().physicsLayers;

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
        else { // Autres cas : aucun dispo / seulement mid / top et bot sans mid
            return null;
        }
    }

    // Check if position in boundaries
    private bool ValideCoordinate(Vector3 position) { 
        Vector2[] boundaries = gm.GetBoundaries();
        if (position.x < boundaries[0].x || boundaries[1].x < position.x) return false;
        if (position.y < boundaries[0].y || boundaries[1].y < position.y) return false;
        return true;
    }

    //Lance un rayon entre _position et _finalPosition, compte le nombre d'intersection. => Mathématiquement, si nb intersection impair, on rentre dans une figure (si on y était pas)
    private bool TeleportInWalls(Vector2 _position, Vector2 _finalPosition, LayerMask _layer) {
        if (ValideCoordinate(_finalPosition)) {
            return RaycastIntersection(_position, _finalPosition, _layer) % 2 == 1; //Envoie le rayon et compte le nombre d'interction
        }
        else {
            return true; //Renvoie qu'on est dans un mur si on est en dehors des limites de la caméra
        }
    }

    //Compte le nombre d'intersection entre _position et _finalPosition
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

            _position = new Vector2(_position.x + direction.x * 0.001f, _position.y + direction.y * 0.001f);

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
}
