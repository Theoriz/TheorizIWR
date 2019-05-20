using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMrScene : MrScene
{

    public Transform TargetTransform;
    public Transform StartTransform;



    public override void LaunchIntro()
    {
        base.LaunchIntro();
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        var currentTimeTranslation = 0.0f;
        while (true)
        {
            currentTimeTranslation += Time.deltaTime;
            transform.position = Vector3.Lerp(StartTransform.position, TargetTransform.position, introAnimationCurve.Evaluate(currentTimeTranslation / introDuration));
            if (currentTimeTranslation / introDuration > 0.99f)
                break;

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public override void LaunchOutro()
    {
        base.LaunchOutro();
        StartCoroutine(Outro());
    }


    IEnumerator Outro()
    {
        var currentTimeTranslation = 0.0f;
        while (true)
        {
            currentTimeTranslation += Time.deltaTime;
            transform.position = Vector3.Lerp(TargetTransform.position, StartTransform.position, introAnimationCurve.Evaluate(currentTimeTranslation / outroDuration));
            if (currentTimeTranslation / outroDuration > 0.99f)
                break;

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
