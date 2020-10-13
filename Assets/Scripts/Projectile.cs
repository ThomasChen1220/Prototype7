using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, 5f);
    }
    private void ApplyDamage(Collider2D collision)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ApplyDamage(collision);
    }
}
