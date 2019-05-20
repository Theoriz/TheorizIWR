using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrScene : MonoBehaviour
{
    [Header("MrScene Settings")]
    public string id;
    public float introDuration;
    public AnimationCurve introAnimationCurve;

    public float outroDuration;
    public AnimationCurve outroAnimationCurve;
    private Coroutine outroCoroutine;

    public int sceneIndex { get; set; }
    public GameObject rootGameObject { get; set; }
    public MrSceneManager sceneManager { get; set; }

    public List<Component> camComponents = new List<Component>();
    
    // Use this for initisalization
    void Awake()
    {
        // get scene manager in hierarchy
        GameObject go = GameObject.Find("Core");

        if (go != null)
        {
            sceneManager = go.GetComponent<MrSceneManager>();
        }
        else
        {
            Debug.LogWarning("Couldn't find a scene manager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Launch()
    {
        sceneManager.launchingIntro = true; 

        // launch intro of scene
        LaunchIntro();

        yield return new WaitForSeconds(introDuration);

        // intro is finished
        if (sceneManager != null)
        {
            sceneManager.launchingIntro = false;
            sceneManager.playingScene = true;
        }        
    }

    public virtual void LaunchIntro()
    {
        if (sceneManager != null)
        {
            this.transform.position = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void End()
    {
        // launch outro of scene
        LaunchOutro();
    }

    public virtual void LaunchOutro()
    {
        // automatically look for DoTweenAnimations in the children of the scene
        // this method can be overriden by all children to code a custom outro
        //foreach (DOTweenAnimation anim in rootGameObject.GetComponentsInChildren<DOTweenAnimation>())
        //{
        //    if (anim.id == "outro")
        //    {
        //        anim.DOPlayAllById("outro");
        //    }
        //}
    }
}
