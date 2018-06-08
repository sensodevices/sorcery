using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour {

    public Player aPlayer;
    private Animator anim;
    private BoxCollider collider;

    private bool isHitting;
    private float health = 0.0f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float lerpMovement = 0.0f;
    public float speedModifier = 0.5f;
    private bool isHit = false;

    public float MaxHealth = 100.0f;
    public bool IsDead { get { return health <= 0.0f; } }

    public event EventHandler<EnemyInfo> OnDead;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider>();
    }
	
	// Update is called once per frame
	void Update () {
        if (IsDead) return;
        float dist = Vector3.Distance(aPlayer.transform.position, transform.position);
        if (dist <= 6.50)
        {
            if (anim.GetBool("Flying"))
                anim.SetBool("Flying", false);
            if (!isHitting)
            {
                isHitting = true;
                anim.Play("Attack");
            }
        } else
        {
            if (!isHit)
            {
                if (!anim.GetBool("Flying"))
                {
                    anim.Play("Flight");
                    anim.SetBool("Flight", true);
                }
                transform.position = Vector3.Lerp(startPosition, targetPosition, lerpMovement);
                lerpMovement += Time.deltaTime * speedModifier;
            }
        }
        transform.LookAt(aPlayer.transform);


	}

    public void Hit(float damage)
    {
        if (health > 0.0f)
        {
            isHit = true;
            health -= damage;
            if (health <= 0.0f)
            {
                Die();
            }
            StartCoroutine(HitCountdown());
            anim.Play("Get hit");
        }
    }

    public void Spawn(Vector3 spawnPoint)
    {
        isHit = false;
        isHitting = false;

        lerpMovement = 0.0f;
        transform.position = spawnPoint;
        startPosition = spawnPoint;
        targetPosition = aPlayer.transform.position;

        health = MaxHealth;

        if (collider != null)
        {
            collider.enabled = true;
        }

        anim.SetBool("Died", false);
        anim.SetBool("Flying", false);
        anim.Play("Idle");
    }

    private void Die()
    {
        var handler = OnDead;
        if (handler != null)
            handler(this, new EnemyInfo());
        anim.SetBool("Died", true);
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    public void DoHit()
    {
        aPlayer.Hit();
    }

    public void HitEnd()
    {
        isHitting = false;
    }

    private IEnumerator HitCountdown()
    {
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    public class EnemyInfo : EventArgs
    {
        
    }
}
