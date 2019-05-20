using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitation : MonoBehaviour {

    public float Intensity;
    public float Frequency;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x, transform.position.y + Intensity * Mathf.Sin(2*Mathf.PI*Frequency * Time.time), transform.position.z);
	}
}
