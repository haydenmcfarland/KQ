using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // Other
    private Animator a;
    public Canvas WinCanvas;
    //Layers of Interest

    private int Water = 4;
    private int Enemy = 10;

    //EXTRA
    private AudioSource audioPlayer;
    private AudioClip clipHit;
    private AudioClip clipClone;
    private AudioClip clipShieldStart;
    private AudioClip clipShieldEnd;
    private AudioClip clipJump;
    private AudioClip clipLand;
    private AudioClip clipDead;
    private AudioClip clipCoin;

    //DEBUG
    public bool GODMODE = true;

    // Player Sliders
    public Slider health_meter;
    public Slider dash_meter;
    public Slider shield_meter;
    public Slider clone_meter;
    public Text death_message;
    public Image treasure_one;
    public Image treasure_two;
    public Image treasure_three;

    // Player Health Slider Parameters
    private float dash_percent = 100;
    private float dash_drain = 45;
    private float dash_recover = 15;
    public float health_percent = 100;
    private float shield_percent = 100;
    private float shield_drain = 20;
    private float shield_recover = 15;
    private float clone_percent = 100;
    private float clone_recover = 5;
    private float clone_limit = 3;
    public GameObject kingClone;
    private List<GameObject> kingCloneEntities = new List<GameObject>();


    // Player State Variables
    private bool right = true;
    private bool dash = false;
    private bool shield = false;
    private bool clone = false;
    private bool hit = false;
    private bool disabled = false;
    private bool death = false;
    private bool drowned = false;
    private bool movement = true;
    private bool death_sound = false;
    private bool win_state = false;
    private bool complete = false;

    // Player Parameters
    private float maxSpeed = 2;
    public float walkSpeed = 2;
    public float dashSpeed = 4;
    Color originalColor;
    float moveX = 0;
    float moveY = 0;

    // Ground and Jumping Check/Parameters for Player
    private bool ground = false;
    public Transform grCheck;
    float grRadius = 0.05f;
    public float jForce = 700f;
    public LayerMask wiGround;

    // Non-player Elements
    public Camera gc;
    public ParticleSystem shieldParticles;

	// Use this for initialization
	void Start () {

        WinCanvas.enabled = false;
        treasure_one.enabled = false;
        treasure_two.enabled = false;
        treasure_three.enabled = false;
        death_message.enabled = false;

        a = GetComponent<Animator>();
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;
        shieldParticles.Stop();
        audioPlayer = gameObject.GetComponent<AudioSource>();

        // LOAD AUDIO
        clipHit = Resources.Load<AudioClip>("Sounds/Simple Damage Sounds/sfx_damage_hit1");
        clipClone = Resources.Load<AudioClip>("Sounds/Weird Sounds/sfx_sound_nagger2");
        clipShieldStart = Resources.Load<AudioClip>("Sounds/Interactions/sfx_sounds_interaction20");
        clipShieldEnd = Resources.Load<AudioClip>("Sounds/Neutral Sounds/sfx_sound_neutral7");
        clipJump = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump8");
        clipLand = Resources.Load<AudioClip>("Sounds/Jumping and Landing/sfx_movement_jump16_landing");
        clipDead = Resources.Load<AudioClip>("Sounds/Human/sfx_deathscream_human1");
        clipCoin = Resources.Load<AudioClip>("Sounds/Simple Bleeps/coin1");

        if (GODMODE)
        {
            shield_recover = 10000;
            dash_recover = 10000;
            clone_recover = 10000;
            shield_drain = 0;
        }
    }


    private void Update()
    {
        if (!win_state)
        {
            MovementCheck();
            OnDeath();
            GroundCheck();
            ShieldMeter();
            CloneMeter();
            DashMeter();
            ColorCheck();
        }
        else if (win_state && !complete)
        {
            shield = true;
            WinCanvas.enabled = true;
            complete = true;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.name.Contains("Explosion") || collision.transform.name.Contains("Fire") && !hit)
        {
            bool explosion = collision.transform.name.Contains("Explosion");

            if (!shield)
            {
                if (explosion)
                    health_percent -= Random.Range(10.0f, 15.0f);
                else
                    health_percent -= Random.Range(10.0f, 20.0f);
                health_meter.value = health_percent;
            }

            Transform explosionTransform = collision.gameObject.GetComponent<Transform>();
            float xDiff = (transform.position.x - explosionTransform.position.x);
            float yDiff = (transform.position.y - explosionTransform.position.y);

            if (collision.transform.name.Contains("Explosion"))
                gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, yDiff).normalized * 50);
            else if (!shield)
                gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, yDiff).normalized * 150);

            if (health_percent <= 0)
                death = true;
            else if (!shield)
            {
                hit = true;
                a.SetBool("hit", hit);
                StartCoroutine(OnHitWait());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Water)
        {
            gc.GetComponent<CameraFollow>().setDeath();
            disabled = true;
            GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -0.4f);
            death = true;
            drowned = true;
            moveX *= 0.3f;
            
            if (shieldParticles.isPlaying)
            {
                shieldParticles.Stop();
                audioPlayer.PlayOneShot(clipShieldEnd);
            }
            audioPlayer.PlayOneShot(clipDead);
        }
    }
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioPlayer.PlayOneShot(clipLand);

        if (collision.transform.name.Contains("Treasure"))
        {
            audioPlayer.PlayOneShot(clipCoin);
            string name = collision.transform.name;
            if (name[name.Length - 1] == '1')
                treasure_one.enabled = true;
            else if (name[name.Length - 1] == '2')
                treasure_two.enabled = true;
            else
                treasure_three.enabled = true;

            Destroy(collision.gameObject);
        }
        else if (collision.transform.name.Contains("Coin"))
        {
            if (collision.gameObject != null)
                Destroy(collision.gameObject);
            StartCoroutine(QuickWait());
        }



    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Enemy && !hit)
        {
            Transform enemyTransform = collision.gameObject.GetComponent<Transform>();
            if (!shield)
            {
                float xDiff = (transform.position.x - enemyTransform.position.x);

                gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDiff, Mathf.Abs(xDiff)).normalized * 200);

                health_percent -= 10;
                health_meter.value = health_percent;
                if (health_percent <= 0)
                    death = true;
                else
                {
                    hit = true;
                    a.SetBool("hit", hit);
                    StartCoroutine(OnHitWait());
                }
            }
        }
    }


    void processJump()
    {
        ground = Physics2D.OverlapCircle(grCheck.position, grRadius, wiGround);

        a.SetBool("Ground", ground);
        a.SetFloat("verticalSpeed", GetComponent<Rigidbody2D>().velocity.y);

    }


    void processMovement()
    {
        if (movement)
        {
            moveX = Input.GetAxis("Horizontal") * maxSpeed;
            moveY = GetComponent<Rigidbody2D>().velocity.y;
            a.SetFloat("speed", Mathf.Abs(moveX));
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveX, moveY);
        }
            

        if (moveX > 0 && !right && !hit)
            Flip();
        else if (moveX < 0 && right && !hit)
            Flip();
    }


    void Flip()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        processJump();
        processMovement();
        if (death)
        {
            if (!death_message.enabled)
                death_message.enabled = true;
        }
    }


    void MovementCheck()
    {
        movement = !hit && !disabled;
    }

    private void DashMeter()
    {
        if (!disabled && Input.GetKey(KeyCode.LeftShift))
        {
            dash = true;
            dash_percent -= Time.fixedDeltaTime * dash_drain;
            dash_meter.value = dash_percent;

            if (dash_percent <= 0)
            {
                dash_percent = 0;
                dash = false;
                maxSpeed = walkSpeed;
            }
            else
                maxSpeed = dashSpeed;
                
        }
        else if (!disabled)
        {
            dash_percent += Time.fixedDeltaTime * dash_recover;
            dash_meter.value = dash_percent;

            if (dash_percent > 100)
                dash_percent = 100;

            dash = false;
            maxSpeed = walkSpeed;
        }
    }


    private void ShieldMeter()
    { 
        if (!disabled && shield_percent >= 100 && Input.GetKeyDown(KeyCode.E) && !shield)
        {
            shieldParticles.Play();
            shield = true;
            audioPlayer.PlayOneShot(clipShieldStart);
        }

        if (shield)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                shield = false;
                audioPlayer.PlayOneShot(clipShieldEnd);
                shieldParticles.Stop();
            }

            shield_percent -= Time.fixedDeltaTime * shield_drain;
            shield_meter.value = shield_percent;

            if (shield_percent <= 0)
            {
                audioPlayer.PlayOneShot(clipShieldEnd);
                shield_percent = 0;
                shield = false;
                shieldParticles.Stop();
            }
        }
        else if (!disabled)
        {
            shield_percent += Time.fixedDeltaTime * shield_recover;
            if (shield_percent > 100)
                shield_percent = 100;
            shield_meter.value = shield_percent;
        }
    }


    private void CloneMeter()
    {
        float percent_threshold = 100;

        for (int i = kingCloneEntities.Count - 1; i >= 0; i--)
        {
            if(kingCloneEntities[i] == null)
                kingCloneEntities.RemoveAt(i);
        }

        if (kingCloneEntities.Count != 0)
            percent_threshold = 100 - (34*kingCloneEntities.Count);

        if (percent_threshold > 100)
            percent_threshold = 100;

        float clones_active = kingCloneEntities.Count;

        if (!disabled && (clone_percent >= percent_threshold || clone_meter.value >= 32) && Input.GetKeyDown(KeyCode.Z) && clones_active != 3)
            clone = true;
            
        if (clone)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                audioPlayer.PlayOneShot(clipClone);
                kingClone.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 150);
                Vector3 temp = Input.mousePosition;
                temp.z = 19.0f;
                kingCloneEntities.Add((GameObject)Instantiate(kingClone, Camera.main.ScreenToWorldPoint(temp), transform.rotation));
            }

            clones_active += 1;
            clone_percent -= Mathf.Ceil(100/clone_limit);
            clone_meter.value = clone_percent;

            if (clone_percent <= 0)
                clone_percent = 0;

            clone = false;
        }
        else if (!disabled)
        {
            if (kingCloneEntities.Count != clone_limit)
            {
                clone_percent += Time.fixedDeltaTime * clone_recover;
                clone_meter.value = clone_percent;

                if (clone_percent > percent_threshold)
                    clone_percent = percent_threshold;
            }
        }
    }


    private void GroundCheck()
    {
        if (ground && !disabled && Input.GetKeyDown(KeyCode.Space))
        {
            audioPlayer.PlayOneShot(clipJump);
            a.SetBool("Ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jForce));
        }
    }


    private void ColorCheck()
    {
        if (death && drowned)
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(89, 125, 255, 125);
        else if (death)
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(32, 43, 66, 255);
        else if (hit)
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        else if (dash && shield)
            gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        else if (dash)
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        else if (shield)
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(89,125,206,200);
        else
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }
    
    private void OnDeath()
    {
        if (death)
        {
            gc.GetComponent<CameraFollow>().setDeath();
            disabled = true;
            GetComponent<Rigidbody2D>().freezeRotation = false;
            if (!death_sound)
            {
                audioPlayer.PlayOneShot(clipDead);
                death_sound = true;
            }
                
        }
        
        if (death && !drowned)
        {
            foreach (Collider2D c in GetComponents<Collider2D>())
                c.enabled = false;
        }
        
    }
    // Public Functions

    public bool canDash()
    {
        return dash;
    }

    public bool isGrounded()
    {
        return ground;
    }

    public bool isUsingShield()
    {
        return shield;
    }

    public bool getWinState()
    {
        return win_state;
    }


    IEnumerator QuickWait()
    {
        audioPlayer.PlayOneShot(clipCoin);
        yield return new WaitForSeconds(1.0f);
        win_state = true;
    }

        IEnumerator OnHitWait()
    {
        audioPlayer.PlayOneShot(clipHit);
        yield return new WaitForSeconds(0.5f);

        hit = false;
        a.SetBool("hit", hit);
    }

    IEnumerator DrainDelay()
    {
        yield return new WaitForSeconds(2.0f);
        shield_drain += 20;
    }

}
