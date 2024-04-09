using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool horizontal;
    public float changeTime;

    public gameObject smokeEffect
    public ParticleSystem fixedEffect;
    
    Rigidbody2D rigidbody2D;
    float remainingTimeToChange;
    Vector2 direction =Vector2.right;
    bool repaired = false;

    //Animation
    Animator animator;

    //Sound
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        remainingTimeToChange = timeToChange;

        direction = horizontal ? Vector2.right : Vector2.down;

        animator = GetComponent<Animator>();
        
        audioSource = GetComponent<AudioSource>();
        //this line of code finds the RubyController script by looking for a "RubyController" tag on Ruby
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!repaired)
        {
            return;
        }
        
        remainingTimeToChange -= Time.deltaTime;

        if (remainingTimeToChange <= 0)
        {
            remainingTimeToChange += timeToChange;
            direction *= -1;
        }

        animator.SetFloat("ForwardX", direction.x);
        animator.SetFloat("ForwardY", direction.y);
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        
        rigidbody2D.MovePosition(position);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if(repaired)
            return;

        RubyController player = other.collider.GetComponent<RubyController >();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
    
    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        animator.SetTrigger("Fixed");
		repaired = true;
		
		smokeParticleEffect.SetActive(false);

		Instantiate(fixedParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);

		//we don't want that enemy to react to the player or bullet anymore, remove its reigidbody from the simulation
		rigidbody2d.simulated = false;
		
		audioSource.Stop();
		audioSource.PlayOneShot(hitSound);
		audioSource.PlayOneShot(fixedSound);
    }
}