using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    public GameObject Explosion;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("Gem"))
        {
            foreach (Collider2D c in GetComponents<Collider2D>())
                c.enabled = false;

            StartCoroutine(explosion());
            if (collision.gameObject != null)
                Destroy(collision.gameObject);
        }
    }

    IEnumerator explosion()
    {
        Vector2 exp1 = new Vector2(transform.position.x - 0.2f, transform.position.y + 0.5f);
        Vector2 exp2 = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.3f);
        Vector2 exp3 = new Vector2(transform.position.x - 0.1f, transform.position.y + 0.2f);
        GetComponent<ParticleSystem>().Stop();
        Instantiate(Explosion, exp1, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(Explosion, exp2, transform.rotation);
        yield return new WaitForSeconds(0.2f);
        Instantiate(Explosion, exp3, transform.rotation);
        yield return new WaitForSeconds(1.0f);
        if (gameObject != null)
            Destroy(gameObject);
    }
}
