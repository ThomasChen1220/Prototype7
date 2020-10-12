using UnityEngine;

public class MovingSphere : MonoBehaviour
{

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [SerializeField]
    LayerMask ground;

    Rigidbody2D body;

    Vector3 velocity, desiredVelocity;

    bool desiredJump;

    int groundContactCount;

    bool OnGround => groundContactCount > 0;

    int jumpPhase;

    float minGroundDotProduct;

    Transform sprite;
    Animator anim;
   

    //for cam
    public Transform camFollow;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        anim = GetComponentInChildren<Animator>();
        sprite = transform.Find("Sprites");
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        OnValidate();
    }

    void Update()
    {
        float playerInput = Input.GetAxis("Horizontal");
        desiredVelocity =
            new Vector3(playerInput, 0f,0f) * maxSpeed;

        anim.SetFloat("horizontal", Mathf.Abs(velocity.x));
        Debug.Log(velocity);
        if (velocity.x < -0.01f)
        {
            sprite.localScale = new Vector3(-1, 1);
        }
        else if (velocity.x > 0.01f)
        {
            sprite.localScale = new Vector3(1, 1);
        }

        desiredJump |= Input.GetButtonDown("Jump");

        //update cam follow
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, jumpHeight*2, ground);
        if (hit.collider != null)
        {
            camFollow.position = hit.point;
        }
        else
        {
            camFollow.position = transform.position;
        }
    }
    
    void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
        ClearState();
    }

    void ClearState()
    {
        groundContactCount = 0;
    }

    void UpdateState()
    {
        velocity = body.velocity;
    }

    void AdjustVelocity()
    {

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        velocity.x = newX;
    }

    void Jump()
    {
        if (OnGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, Vector3.up);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += Vector3.up * jumpSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            groundContactCount += 1;
        }
    }
}