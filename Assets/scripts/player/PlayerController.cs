using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Max Values")]
    public int maxHP;
    public int maxJumps;
    public float move_speed;
    public float slow_time;
    public float time_hit;
    public float max_chAttkDmg;

    [Header("cur Values")]
    public int curHP;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public bool isSlowed;
    public bool isCharging;

    [Header("mods")]
    public float jump_force;
    public float cur_speed;

    [Header("Audio")]
    public AudioClip[] playerfx;
    // jump 0
    // hit 1
    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource audio;
    [SerializeField]
    private Transform muzzle;
    public GameObject deathEfectprefab;
    public PlayerContainerScript uiContainer;

    [Header("Attacking")]
    [SerializeField]
    private PlayerController curAttacker;
    public float attackRate;
    public float lastAttackTime;
    public float attackSpeed;
    public float attackDmg;
    public GameObject[] attackPrefabs;
    public float chAttkDmg;
    public float chargeRate;


    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        audio.PlayOneShot(playerfx[4]);
        gameManager = GameObject.FindObjectOfType<GameManager>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();
        chAttkDmg = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
        curJumps = maxJumps;
        cur_speed = move_speed;
        uiContainer.updateHealthBar(curHP, maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10||curHP <=0)
        {
            die();
        }
        if (isSlowed)
        {
            if (Time.time - time_hit > slow_time)
            {
                isSlowed = false;
                cur_speed = move_speed;
            }
        }
        if (isCharging)
        {
            chAttkDmg += chargeRate;
            if(chAttkDmg > max_chAttkDmg)
            {
                chAttkDmg = max_chAttkDmg;
            }
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }
    }
    private void FixedUpdate()
    {
        move();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach(ContactPoint2D x in collision.contacts)
        {
            if (x.collider.CompareTag("ground"))
            {
                if(x.point.y < transform.position.y)
                {
                    curJumps = maxJumps;
                    audio.PlayOneShot(playerfx[2]);
                }
                if((x.point.x > transform.position.x || x.point.x < transform.position.x) && (x.point.y < transform.position.y))
                {
                    if(curJumps < 3)
                    {
                        curJumps++;
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void jump()
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        audio.PlayOneShot(playerfx[0]);
        rig.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);
    }

    private void move()
    {
        rig.velocity = new Vector2(curMoveInput*cur_speed,rig.velocity.y);
        if(curMoveInput != 0.0f)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    public void die()
    {
        audio.PlayOneShot(playerfx[1]);
        Destroy(Instantiate(deathEfectprefab, transform.position, Quaternion.identity),2);
        rig.velocity = Vector2.zero;
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            if (score < 0)
            {
                score = 0;
            }
        }
        respawn();
    }

    public void addScore()
    {
        score++;
        uiContainer.updateScoreText(score);
    }

    public void takeDamage(int ammount, PlayerController attacker)
    {
        curHP -= ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(curHP,maxHP);
        if (isCharging)
        {
            chAttkDmg = chAttkDmg / 2;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }
    }
    public void takeDamage(float ammount, PlayerController attacker)
    {
        curHP -= (int)ammount;
        curAttacker = attacker;
        uiContainer.updateHealthBar(curHP, maxHP);
        if (isCharging)
        {
            chAttkDmg = chAttkDmg / 2;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }
    }
    public void takeIceDamage(float ammount, PlayerController attacker)
    {
        time_hit = Time.time;
        curHP -= (int)ammount;
        curAttacker = attacker;
        isSlowed = true;
        cur_speed /= 2;
    }
    private void respawn()
    {
        curHP = maxHP;
        curJumps = maxJumps;
        transform.position = gameManager.spawnpoints[UnityEngine.Random.Range(0, gameManager.spawnpoints.Length)].position;
        curAttacker = null;
        cur_speed = move_speed;
        chAttkDmg = 0;
        isCharging = false;
        uiContainer.updateHealthBar(curHP, maxHP);
        uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
    }

    
    public void spawnStdFireball(float dmg, float speed)
    {
        GameObject fireball = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireball.GetComponent<ProjectileScript>().onSpawn(dmg, speed, this,transform.localScale.x);
        audio.PlayOneShot(playerfx[8]);
    }

    public void spawnIceAttack(float dmg, float speed)
    {
        GameObject iceball = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceball.GetComponent<IceProjectileScript>().onSpawn(dmg, speed, this, transform.localScale.x);
        audio.PlayOneShot(playerfx[9]);
    }
    public void spawnChargAttk(float dmg, float speed)
    {
        audio.PlayOneShot(playerfx[10]);
        GameObject chargeball = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chargeball.GetComponent<ProjectileScript>().onSpawn(dmg, speed, this, transform.localScale.x);
    }
    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            
            if(curJumps > 0)
            {
                curJumps--;
                jump();
            }
        }
    }
    public void onMoveInput(InputAction.CallbackContext context)
    {
         
         float x = context.ReadValue<float>();
        if(x > 0)
        {
            curMoveInput = 1;
        }
        else if(x < 0)
        {
            curMoveInput = -1;
        }
        else
        {
            curMoveInput = 0;
        }
    }
    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
           
        }
    }
    public void onStdAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawnStdFireball(attackDmg, attackSpeed);
            if (isCharging)
            {
                chAttkDmg = chAttkDmg/2;
            }
            
        }
    }
    public void onChrAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isCharging = true;
        }
        if(context.phase == InputActionPhase.Canceled && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            isCharging = false;
            spawnChargAttk(chAttkDmg, attackSpeed);
            chAttkDmg = 0;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }
    }
    public void onIceInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawnStdFireball(attackDmg, attackSpeed);
            if (isCharging)
            {
                chAttkDmg = chAttkDmg / 2;
            }
            
        }
    }
    public void setUIContainer(PlayerContainerScript ContainerUI)
    {
        this.uiContainer = ContainerUI;
    }

    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            
            audio.PlayOneShot(playerfx[3]);
        }
    }
    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            
        }
    }
    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
           
        }
    }
    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            
        }
    }
    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
           
        }
    }
}
