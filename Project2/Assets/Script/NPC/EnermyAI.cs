using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class EnermyAI : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 100;
    int health = 0;

    [SerializeField]
    float moveSpeedMax;

    [SerializeField]
    float moveSpeedMin;

    [SerializeField]
    float attackSpeed = 1f;

    [SerializeField]
    int attackDamage;

    [SerializeField]
    float checkRadius;

    [SerializeField]
    float attackRadius;
    float canAttack;

    bool isDead = false;

    [SerializeField]
    LayerMask whatIsPlayer;

    SpriteRenderer spriteRenderer;

    GameObject target;
    Rigidbody2D rb;
    Animator anim;
    Vector3 dir;

    private bool isInChaseRange;
    private bool isInAttackRange;

    [SerializeField]
    HealthBar healthBar;

    [SerializeField]
    GameObject CoinPrefab;

    private void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
        {
            anim.SetBool("isRunning", isInChaseRange);
            isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, whatIsPlayer);
            isInAttackRange = Physics2D.OverlapCircle(
                transform.position,
                attackRadius,
                whatIsPlayer
            );
            dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            anim.SetFloat("X", dir.x);
            anim.SetFloat("Y", dir.y);
            // Don't flip the scale of the object, it's gonna mess up the UI, use "SpriteRenderer.Flip" instead.
            //Vector3 xxx = transform.localScale;
            if ((target.transform.position.x - transform.position.x) < 0)
            {
                spriteRenderer.flipX = true;
            }
            if ((target.transform.position.x - transform.position.x) > 0)
            {
                spriteRenderer.flipX = false;
            }
            if (isInAttackRange)
            {
                anim.SetBool("isRunning", false);
                if (!isDead)
                {
                    if (attackSpeed <= canAttack)
                    {
                        GameObject
                            .FindGameObjectWithTag("Player")
                            .gameObject.GetComponent<PlayerControl>()
                            .UpdateHealth(-attackDamage);
                        canAttack = 0;
                    }
                    else
                    {
                        canAttack += Time.deltaTime;
                    }
                }
            }
        }
    }

    // FixedUpdate is called every frame at the physic engine fps (50)
    private void FixedUpdate()
    {
        if (isInChaseRange && !isInAttackRange)
        {
            rb.MovePosition(
                transform.position
                    + (dir.normalized * Random.Range(moveSpeedMin, moveSpeedMax) * Time.deltaTime)
            );
        }
        if (isInAttackRange)
        {
            rb.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Update health amount on change
    /// </summary>
    /// <param name="change"></param>
    public void UpdateHealth(int change)
    {
        health += change;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            health = 0;
            anim.SetBool("isDead", true);
            if (!isDead)
            {
                AudioManager.Play(AudioFileName.ZombieDeath);
            }
            isDead = true;
            StartCoroutine(Dead());
        }
        healthBar.SetHealth(health);
    }

    private void OnCollisionEnter2D(Collision2D collision) { }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        // Instantiate a new coin prefab.
        GameObject obj = Instantiate(CoinPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
