using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;

    public SpriteRenderer spriteRenderer;

    private Transform target;
    private int destPoint = 0;

    void Start()
    {
        target = waypoints[destPoint];
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        Flip(dir.x);
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 0.3f){
            destPoint = (destPoint+1)%waypoints.Length;
            target = waypoints[destPoint];
        }
    }

    void Flip(float _velocity){
        if (_velocity > 0.1f){
            spriteRenderer.flipX = false;
        }else if (_velocity < -0.1f){
            spriteRenderer.flipX = true;
        }
    }
}
