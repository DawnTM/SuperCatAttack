using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int damage;
    public PlayerController owner;
    public float lifetime;
    public float speed;
    private Rigidbody2D rig;
    // Start is called before the first frame update
    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeDamage(damage,owner);
        }
        Destroy(gameObject);
    }
    public void onSpawn(float dmg, float speed, PlayerController owner, float dir)
    {
        setOwner(owner);
        setDamage(dmg);
        setSpeed(speed);
        rig.velocity = new Vector2(dir * speed, 0);
    }

    public void setOwner(PlayerController owner)
    {
        this.owner = owner;
    }
    public void setDamage(float damage)
    {
        this.damage = (int)damage;
    }
    public void setDamage(int damage)
    {
        this.damage = damage;
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
}
