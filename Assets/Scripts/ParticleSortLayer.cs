using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSortLayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sortingOrder = 4;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
