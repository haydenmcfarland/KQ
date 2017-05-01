using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Modified from original source:
 * https://forum.unity3d.com/threads/how-to-create-ghost-or-after-image-on-an-animated-sprite.334079/
 * 
 */

public class TrailRender : MonoBehaviour {

    List<GameObject> trailParts = new List<GameObject>();

    void Start()
    {
        InvokeRepeating("SpawnTrailPart", 0, 0.05f);
    }

    void SpawnTrailPart()
    {
        if (gameObject.GetComponent<PlayerController>().canDash())
        {
            GameObject trailPart = new GameObject();
            trailPart.name = gameObject.name + "_trailrender";
            SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
            trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
            trailPart.transform.position = transform.position;
            trailPart.transform.localScale = transform.localScale;
            trailParts.Add(trailPart);

            StartCoroutine(FadeTrailPart(trailPartRenderer));
            trailParts.Remove(trailPart);
            Destroy(trailPart, 0.2f);
        }

        
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color color = trailPartRenderer.color;
        color.a -= 0.8f;
        trailPartRenderer.color = color;

        yield return new WaitForEndOfFrame();
    }
}
