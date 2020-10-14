using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;

    Rigidbody2D rb;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = transform.right;
        Destroy(gameObject, 4f);
    }
    private void Update()
    {
        rb.velocity = direction * speed;
    }
    /*private void ApplyDamage(Collider2D collider)
    {

    }*/
	private void ApplyDamage(Collision2D collision) {

	}
	/*private void OnTriggerEnter2D(Collider2D collision)
    {
        ApplyDamage(collision);
		Destroy(gameObject);
    }*/
	private void OnCollisionEnter2D(Collision2D collision) {
		ApplyDamage(collision);
		Destroy(gameObject);
	}
}
