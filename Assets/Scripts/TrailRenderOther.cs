using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderOther : MonoBehaviour
{

    public Color color;

    List<GameObject> trailParts = new List<GameObject>();

    public bool trail = true;
    public float delay = 0.2f;
    public float repeat = 0.05f;
    void Start()
    {
        InvokeRepeating("SpawnTrailPart", 0, repeat);
    }

    void SpawnTrailPart()
    {
        if (trail)
        {
            GameObject trailPart = new GameObject();
            trailPart.name = gameObject.name + "_trailrender";
            SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
            trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
            trailPartRenderer.color = color;
            trailPart.transform.position = transform.position;
            trailPart.transform.localScale = transform.localScale;
            trailParts.Add(trailPart);

            StartCoroutine(FadeTrailPart(trailPartRenderer));
            trailParts.Remove(trailPart);
            Destroy(trailPart, delay);
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