using UnityEngine;
using DG.Tweening;

public class MovingSphere : MonoBehaviour
{

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAirAcceleration = 1f;

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
   

    [Header("cam")]
    public Transform camFollow = null;

    [Header ("shooting")]
    public Transform shootPoint;
    ShootEffect shootEffect;
    public GameObject bullet;
    [Range(0, 25)]
    public int bulletPerSecond=4;

    public KickBack mGun;

    private float lastShot;
    private float shotInterval => 1f /bulletPerSecond;

	private AudioManager _audio;
	private float _footstepTimer = 0f;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        anim = GetComponentInChildren<Animator>();
        sprite = transform.Find("Sprites");
        lastShot = 0;
        shootEffect = shootPoint.GetComponent<ShootEffect>();
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        OnValidate();
    }

    void Update()
    {
        float playerInput = Input.GetAxisRaw("Horizontal");
        desiredVelocity =
            new Vector3(playerInput, 0f,0f) * maxSpeed;

        anim.SetFloat("horizontal", Mathf.Abs(velocity.x));

        if (velocity.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1);
        }
        else if (velocity.x > 0.01f)
        {
            transform.localScale = new Vector3(1, 1);
        }

		// footstep sounds
		if ((anim.GetFloat("horizontal") > 0.01f)  && (OnGround)) {
			_footstepTimer += anim.GetFloat("horizontal") * Time.deltaTime;
			if (_footstepTimer > 1f) {
				_audio.PlayFootstep();
				_footstepTimer = 0f;
			}
		}
		else {
			_footstepTimer = 1f;
		}

        //update jump
        desiredJump |= Input.GetButtonDown("Jump")||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow);

        //update shooting
        if((lastShot+shotInterval)<Time.time && Input.GetMouseButton(0))
        {
            GameObject b = Instantiate(bullet, shootPoint.position, Quaternion.identity) as GameObject;
			// play sound of laser
			_audio.PlayGunshot();
            if (transform.localScale.x < 0)
            {
                Projectile p = b.GetComponent<Projectile>();
                p.direction.x = -1;

                mGun.Kick(-1);
            }
            else
            {
                mGun.Kick();
            }
            PlayShootEffect();

            lastShot = Time.time;
        }

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

    void PlayShootEffect()
    {
        shootEffect.Play();
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

        float acceleration = OnGround ? -1 : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX;
        if (acceleration != -1)
        {
            newX =
            Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        }
        else
        {
            newX = desiredVelocity.x;
        }
        
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
			// play jump sound
			_audio.PlayJump();
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