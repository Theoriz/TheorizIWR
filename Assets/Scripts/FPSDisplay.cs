using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    private bool showFPS;

    public void ToggleFPSDisplay()
    {
        showFPS = !showFPS;
    }
    void Update()
    {
        if (Input.GetKeyDown("f"))
            ToggleFPSDisplay();
    }
    void OnGUI()
    {
        if(showFPS)
            GUI.Label(new Rect(0, 0, 100, 100), "FPS : " + (1.0f / Time.smoothDeltaTime).ToString("F1"));
    }
}