using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MrSceneManager : MonoBehaviour
{
    public bool AutoSwitchScene;
    public float AutoSwitchTimer = 10;
    public bool Loop;
    private float _sceneStartTime;

    public List<GameObject> ObjectsToKeepBetweenScenes;
    public string sceneLayer;
    public int sceneCount { get; set; }

    public List<string> scenesName;
    public string activeScene;

    public int currSceneIndex;

    public bool inUpdate { get; set; }
    public bool loadingScene { get; set; }
    public bool launchingIntro { get; set; }
    public bool launchingOutro { get; set; }
    public bool playingScene { get; set; }

    private MrScene currMrScene;
    private Dictionary<string, int> scenesInBuild;

    void Awake()
    {
        scenesName = new List<string>();
        scenesInBuild = new Dictionary<string, int>();

        DontDestroyOnLoad(this);
        foreach (var item in ObjectsToKeepBetweenScenes)
        {
            DontDestroyOnLoad(item);
        }
    
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)   //Starts at 1 to remove Core which is 0
        {
            var sceneName =
                System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility
                    .GetScenePathByBuildIndex(i));
            scenesName.Add(sceneName);
            scenesInBuild.Add(sceneName, i);
        }

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Use this for initialization
    void Start()
    {
        // get scene count
        sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;


        // init variables
        loadingScene = false;
        launchingIntro = false;
        launchingOutro = false;
        playingScene = false;

        // load first scene
        currSceneIndex = 1;

        var lastIndex = PlayerPrefs.GetInt("lastScene", -1);
        if (lastIndex == -1)
        {
            PlayerPrefs.SetInt("lastScene", currSceneIndex);
        }
        else
        {
            Debug.Log("Count build : " + scenesInBuild.Count + " | strange count : " + lastIndex);
            if(lastIndex <= scenesInBuild.Count)
                currSceneIndex = lastIndex;
        }

        SceneManager.LoadScene(currSceneIndex);
        activeScene = SceneManager.GetActiveScene().name;//SceneManager.GetSceneByBuildIndex(currSceneIndex).name;
    }

    void Update()
    {
        inUpdate = true;

        if(AutoSwitchScene)
        {
            if(Time.time - _sceneStartTime > AutoSwitchTimer )
            {
                Next();
            }
        }
        else
        {
            _sceneStartTime = Time.time;
        }

        if (activeScene != SceneManager.GetActiveScene().name && !loadingScene)
        {
            LoadSceneWithName(activeScene);
            //edge.SetCurrentScene();
            //SceneStreamer.SetCurrent(activeScene);
        }
    }

    public void Next()
    {
        if(!loadingScene)
            StartCoroutine(loadScene(currSceneIndex + 1));
    }

    public void Previous()
    {
        if (!loadingScene)
            StartCoroutine(loadScene(currSceneIndex - 1));
    }

    public void Reload()
    {
        if (!loadingScene)
            StartCoroutine(loadScene(currSceneIndex));
    }

    public void LoadSceneWithName(string name)
    {
        if (!loadingScene)
        {
            Debug.Log("Build index : " + scenesInBuild[name] + " | Current index : " + currSceneIndex);
            loadingScene = true;
            StartCoroutine(loadScene(scenesInBuild[name]));
        }
    }

    public IEnumerator loadScene(int nextSceneIndex)
    {
        if (nextSceneIndex < 1)
        {
            //nextSceneIndex = 1;
            yield break;
        }
        if (nextSceneIndex > sceneCount)
        {
            if (Loop)
                nextSceneIndex = 1;
            else
            {
                //nextSceneIndex = sceneCount;
                yield break;
            }
        }

        // a new scene is loading
        _sceneStartTime = Time.time;
        loadingScene = true;
        if (currMrScene != null)
        {
            // launch scene outro
            currMrScene.End();
            playingScene = false;
            launchingOutro = true;

            // wait appropriate amount of time
            yield return new WaitForSeconds(currMrScene.outroDuration);

            // scene outro is done
            launchingOutro = false;
        }

        currSceneIndex = nextSceneIndex;

        
        yield return new WaitForFixedUpdate(); //To allow SharedTextureControllable to disable spout;
        Debug.Log("Loading scene index " + currSceneIndex);
        UnityEngine.SceneManagement.SceneManager.LoadScene(currSceneIndex);
        activeScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt("lastScene", currSceneIndex);
        loadingScene = false;

        //if (inUpdate)
//StartCoroutine(LoadYourAsyncScene());
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeScene = SceneManager.GetActiveScene().name;
        // do not process Main scene
        if (scene.name == "Core") return;

        // fetch parent object of scene
        //GameObject go = scene.GetRootGameObjects()[0];

        bool foundMrScene = false;
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            if (go.GetComponent<MrScene>() != null)
            {
                // get components in object and set variables
                currMrScene = go.GetComponent<MrScene>();
                currMrScene.rootGameObject = go;
                currMrScene.sceneIndex = currSceneIndex;

                foundMrScene = true;
            }
        }

        if (!foundMrScene)
        {
            Debug.LogWarning("Couldn't find MrScene component in scene.");
            return;
        }

        Debug.Log("Level " + scene.name + " loaded with id " + currMrScene.id + " and build index " + currSceneIndex);

        ControllableMaster.LoadEveryPresets();

        // scene has succesfully loaded
        loadingScene = false;

        // launch intro of loaded scene
        StartCoroutine(currMrScene.Launch());
        playingScene = false;
        launchingIntro = true;

        //launch preloading of next scene
       // if(inUpdate)
         //   StartCoroutine(LoadAsyncScene());
    }

    //IEnumerator LoadYourAsyncScene()
    //{
    //    // The Application loads the Scene in the background at the same time as the current Scene.
    //    //This is particularly good for creating loading screens. You could also load the Scene by build //number.
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Cube2");//, LoadSceneMode.Additive);
    //    asyncLoad.allowSceneActivation = false;
    //    //Wait until the last operation fully loads to return anything
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //    //asyncLoad.allowSceneActivation = true;
    //}

}
