using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{

    public Transform centerStage;
    public GameObject Explosion;
    public GameObject Enemy;
    public GameObject Coin;
    public Transform[] SpawnLocations;
    List<GameObject> enemies = new List<GameObject>();

    // Other
    private Animator a;
    public float health_percent = 100;

    // Player State Variables
    private bool initial_target = false;
    private bool right = true;
    private bool death = false;
    private bool damageTaken = false;
    private bool targetPlayer = false;
    private bool centerFlag = false;
    private bool triggerFinal = false;

    // Player Parameters
    public float maxSpeed = 0.4f;

    // Sounds
    private AudioSource audioPlayer;
    private AudioClip clipDead;

    // Player Transform
    public Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        a = GetComponent<Animator>();
        audioPlayer = gameObject.GetComponent<AudioSource>();
        clipDead = Resources.Load<AudioClip>("Sounds/Human/sfx_deathscream_human1");
    }

    private void Update()
    {
        targetPlayer = ((enemies.Count == 0) && initial_target);

        if (!death)
        {
            if (health_percent <= 0)
                death = true;
            else if (health_percent < 60)
            {
                if (Random.Range(0.0f, 10.0f) < .01f && !centerFlag)
                    StartCoroutine(CenterStage());
            }
            else if (health_percent < 30)
            {
                if (Random.Range(0.0f, 10.0f) < .05f && !centerFlag)
                    StartCoroutine(CenterStage());
            }
        }
        else if (!triggerFinal)
        {
            triggerFinal = true;
            StartCoroutine(explosion());
        }


        

     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("Clone") )
        {
            float xDiff = (collision.transform.position.x - transform.position.x);

            if (health_percent >= 60)
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, Mathf.Abs(xDiff)).normalized * 300);
            else
            {
                StartCoroutine(KillClone(collision.gameObject));
            }
        }
        else if (collision.transform.name.Contains("Character"))
        {
            if (!collision.gameObject.GetComponent<PlayerController>().isUsingShield())
            {
                float xDiff = (transform.position.x - collision.transform.position.x);
                gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, Mathf.Abs(xDiff)).normalized * 400);
                StartCoroutine(HitPlayer());
            }
            
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name.Contains("Explosion") && !damageTaken)
        {

            float xDiff = (transform.position.x - collision.transform.position.x);

            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, Mathf.Abs(xDiff)).normalized * 600);
            StartCoroutine(TakeDamage());
        }
           
    }

    void FixedUpdate()
    {
        if (!death)
        {
            if (playerTransform == null)
                playerTransform = transform;

            float step = maxSpeed * Time.fixedDeltaTime;

            if (!targetPlayer && Mathf.Abs(transform.position.x - playerTransform.position.x) < 2)
                initial_target = true;

            if (targetPlayer && initial_target)
                gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, playerTransform.position, step);

            float xEnemy = gameObject.transform.position.x;
            float xPlayer = playerTransform.position.x;

            if (xEnemy - xPlayer > 0 && right)
                Flip();

            else if (xEnemy - xPlayer < 0 && !right)
                Flip();
        }

    }

    void Flip()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator TakeDamage()
    {
        damageTaken = true;
        audioPlayer.PlayOneShot(clipDead);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
        health_percent -= Random.Range(25.0f,35.0f);
        a.SetFloat("Health", health_percent);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        damageTaken = false;
    }

    IEnumerator HitPlayer()
    {
        yield return new WaitForSeconds(1.2f);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        damageTaken = false;
    }

    IEnumerator KillClone(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        if (obj != null)
            Destroy(obj);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    IEnumerator CenterStage()
    {
        centerFlag = true;
        GetComponent<Collider2D>().enabled = false;
        targetPlayer = false;
        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 255;
        GetComponent<SpriteRenderer>().color = Color.magenta;
        transform.position = centerStage.position;
        for(int i = 0; i < 3; i++)
        {
            enemies.Add((GameObject)Instantiate(Enemy, SpawnLocations[i].position, transform.rotation));
            enemies[i].GetComponent<EnemyController>().playerTransform = playerTransform;
        }

        while(enemies.Count != 0)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] == null)
                    enemies.RemoveAt(i);
            }
            yield return null;
        }
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = Color.white;
        centerFlag = false;
    }

    IEnumerator explosion()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.position = centerStage.position;
        Vector2 exp1 = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.5f);
        Vector2 exp2 = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f);
        Vector2 exp3 = new Vector2(transform.position.x - 0.1f, transform.position.y + 0.2f);
        Vector2 exp4 = new Vector2(transform.position.x - 0.4f, transform.position.y - 0.5f);
        Vector2 exp5 = new Vector2(transform.position.x + 0.4f, transform.position.y - 0.4f);
        Vector2 exp6 = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f);
        Vector2 exp7 = new Vector2(transform.position.x - 0.1f, transform.position.y + 0.2f);
        Instantiate(Explosion, exp1, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Explosion, exp2, transform.rotation);
        yield return new WaitForSeconds(0.4f);
        Instantiate(Explosion, exp3, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Explosion, exp4, transform.rotation);
        yield return new WaitForSeconds(0.2f);
        Instantiate(Explosion, exp5, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Explosion, exp6, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(Explosion, exp7, transform.rotation);
        Instantiate(Coin, transform.position, transform.rotation);
        if (gameObject != null)
            Destroy(gameObject);
    }
}