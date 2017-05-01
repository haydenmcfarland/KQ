using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    // Other
    private Animator a;

    // Player State Variables
    private bool right = true;
    private bool dash = true;
    private bool disabled = false;

    // Player Parameters
    public float maxSpeed = 0.2f;
    public float walkSpeed = 0.2f;
    public float dashSpeed = 0.2f;
    public bool trail;
    // Sounds
    private AudioSource audioPlayer;
    private AudioClip clipJump;
    private AudioClip clipLand;
    private AudioClip clipDead;
    private AudioClip clipImpact;

    // Ground and Jumping Check/Parameters for Player
    private bool ground = false;
    public Transform grCheck;
    float grRadius = 0.05f;
    public float jForce = 700f;
    public LayerMask wiGround;

    // Enemy Target
    public Transform playerTransform;

    void Start()
    {
        a = GetComponent<Animator>();
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        audioPlayer = gameObject.GetComponent<AudioSource>();
        StartCoroutine(SoundDelay());
        clipJump = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump8");
        clipLand = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump16_landing");
        clipDead = Resources.Load<AudioClip>("Sounds/Human/sfx_deathscream_human1");        
    }


    private void Update()
    {
        JumpCheck();
        CheckGround();
        EnemyWalkBehavior();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            disabled = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(89, 125, 255, 125);
            GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x*0.3f, -0.4f);
            audioPlayer.PlayOneShot(clipDead);
            StartCoroutine(OnFallWait());
        }
        else if (collision.transform.name.Contains("Explosion"))
        {
            StartCoroutine(FadeDeath());
        }
    }


    void CheckGround()
    {
        bool ground_past = ground;
        ground = Physics2D.OverlapCircle(grCheck.position, grRadius, wiGround);
        if (!ground_past && ground)
        {
            if (!audioPlayer.isPlaying)
                audioPlayer.PlayOneShot(clipLand);
        }
        
        a.SetBool("Ground", ground);
        a.SetFloat("verticalSpeed", GetComponent<Rigidbody2D>().velocity.y);
    }


    void Flip()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    void EnemyWalkBehavior()
    {
        float step = maxSpeed * Time.fixedDeltaTime;

        if (step > maxSpeed)
            step = maxSpeed;

        if (Mathf.Abs(transform.position.x - playerTransform.position.x) < 2)
        {
            a.SetFloat("speed", Mathf.Abs(step));
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, playerTransform.position, step);
        }
        else
            a.SetFloat("speed", 0);

        float xEnemy = gameObject.transform.position.x;
        float xPlayer = playerTransform.position.x;

        if (xEnemy - xPlayer > 0 && right)
            Flip();

        else if (xEnemy - xPlayer < 0 && !right)
            Flip();
    }
    void FixedUpdate()
    {

    }


    private void JumpCheck()
    {
        float pVelocity = playerTransform.GetComponent<Rigidbody2D>().velocity.y;

        bool inRangeJump = (Mathf.Abs(transform.position.x - playerTransform.position.x) < 0.25) && pVelocity != 0;
        bool canMove = ground && !disabled && inRangeJump;

        if (canMove && inRangeJump)
        {
            a.SetBool("Ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 20));
            if (!audioPlayer.isPlaying)
                audioPlayer.PlayOneShot(clipJump);
        }
    }


    IEnumerator OnFallWait()
    {
        yield return new WaitForSeconds(3.0f);
        if (gameObject != null)
            Destroy(gameObject);
    }

    IEnumerator FadeDeath()
    {
        disabled = true;
        audioPlayer.PlayOneShot(clipDead);
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0;
        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = false;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForEndOfFrame();
        if (gameObject != null)
            Destroy(gameObject, 0.9f);
    }

    IEnumerator SoundDelay()
    {
        audioPlayer.volume = 0.0f;
        yield return new WaitForSeconds(0.5f);
        audioPlayer.volume = 1.0f;
    }
}