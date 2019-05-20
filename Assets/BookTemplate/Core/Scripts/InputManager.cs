using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public UIMaster ui;

    public MrSceneManager SceneManager;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("h"))
            ui.ToggleUI();

        if(Input.GetKeyDown(KeyCode.LeftArrow))
            SceneManager.Previous();

	    if (Input.GetKeyDown(KeyCode.RightArrow))
	        SceneManager.Next();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
	}
}
