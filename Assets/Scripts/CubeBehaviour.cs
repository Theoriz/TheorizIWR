using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour {

    [Tooltip("Number of seconds for a 360° rotation. Negative values rotate counter-clockwise.")]
    public float Speed;

    public GameObject Doom;

    private bool _doomMode;

    public bool DoomMode {
        get
        {
            return _doomMode;
        }
        set
        {
            _doomMode = value;
            if(Doom != null)
                Doom.SetActive(_doomMode);
            else
                Debug.Log("No Doom mode in this scene :/");
        }
    }

    public Transform Pivot;
	
	// Update is called once per frame
	void Update () {

        if(Speed == 0.0f) return;

        gameObject.transform.RotateAround(Pivot.position, Vector3.up, Time.deltaTime * Speed);
    }
}
