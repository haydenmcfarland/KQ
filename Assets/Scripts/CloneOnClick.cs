using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneOnClick : MonoBehaviour {

    private Animator a;
    private Camera MyCamera;

    // Player State Variables
    private bool right = true;
    private bool disabled = false;
    private bool explode = false;
    private bool movement = false;
    private float moveX = 0;
    public Canvas Menu;
    public GameObject Explosion;

    // Player Parameters
    public float maxSpeed = 0.2f;

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

    void Start()
    {
        a = GetComponent<Animator>();
        MyCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Menu.worldCamera = MyCamera;
        Menu.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        audioPlayer = gameObject.GetComponent<AudioSource>();
        clipJump = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump8");
        clipLand = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump16_landing");
        clipDead = Resources.Load<AudioClip>("Sounds/Human/sfx_deathscream_human1");
    }

    private void Update()
    {
        Transform playerTransform = GameObject.Find("Character").transform;

        CheckGround();
        CloneWalkBehavior();

        if (Mathf.Abs(transform.position.x - playerTransform.position.x) > 2.25)
        {
            Menu.enabled = false;
            if (!explode)
            {
                StartCoroutine(Flash());
                explode = true;
                Menu.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            disabled = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(89, 125, 255, 125);
            GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.3f, -0.4f);
            audioPlayer.PlayOneShot(clipDead);
            StartCoroutine(OnFallWait());
        }
        else if (collision.transform.name.Contains("Explosion"))
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            disabled = true;
            StartCoroutine(ChainExplosion(0.3f));
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
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        foreach (Transform child in transform)
        {
            if (child.name != "Shadow")
            {
                child.transform.localScale = scale;
            }
            
        }
            
    }

    void CloneWalkBehavior()
    {
        if (movement)
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, GetComponent<Rigidbody2D>().velocity.y);

        a.SetFloat("speed", Mathf.Abs(moveX));

        if (moveX > 0 && !right)
            Flip();
        else if (moveX < 0 && right)
            Flip();
    }

    IEnumerator OnFallWait()
    {
        yield return new WaitForSeconds(3.0f);
        if (gameObject != null)
        Destroy(gameObject);
    }

    IEnumerator ChainExplosion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Instantiate(Explosion, transform.position, transform.rotation);
        if (gameObject != null)
            Destroy(gameObject);
    }

    IEnumerator Flash()
    {
        Menu.enabled = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.6f);
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(0.4f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.03f);
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(0.025f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.020f);
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(0.015f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.015f);
        movement = false;
        moveX = 0;
        disabled = true;
        yield return new WaitForSeconds(1.0f);
        Instantiate(Explosion, transform.position, transform.rotation);
        if (gameObject != null)
            Destroy(gameObject);
    }

    public void startBomb()
    {
        if (!explode)
        {
            StartCoroutine(Flash());
            explode = true;
        }
    }

    public void moveRight()
    {
        movement = true;
        moveX = 0.5f;
        Menu.enabled = false;

    }

    public void moveLeft()
    {
        movement = true;
        moveX = -0.5f;
        Menu.enabled = false;

    }

    private void OnMouseDown()
    {
        if (!explode)
        {
            if (!disabled)
                Menu.enabled = !Menu.enabled;
        }

    }
}
