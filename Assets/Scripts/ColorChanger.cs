using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{

    public Light spotLight;

    private Color lightColor;

	// Use this for initialization
	void Start ()
	{
	    lightColor = spotLight.GetComponent<Light>().color;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float H, V, S;
	    lightColor = spotLight.GetComponent<Light>().color;
        Color.RGBToHSV(lightColor, out H, out V, out S);
        //Debug.Log("H : " + H);
	    H+=0.005f;
	    if (H > 1.0f)
	        H = 0.0f;

	    spotLight.GetComponent<Light>().color = Color.HSVToRGB(H, V, S); ;
    }
}
