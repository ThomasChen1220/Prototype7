using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public Sprite damaged;
    public int life = 2;

    public void Damage()
    {
        life--;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
        if (life < 2)
        {
            GetComponent<SpriteRenderer>().sprite = damaged;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
