using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager main;
    private void Awake() { if(main!=null) Destroy(this.gameObject); else main=this; }

    [Header("Configuration")]
    public string SceneLoader_Name;
    [Range(0,10)]public float timeWaitCharge = 1;

    [Header("Read")]
    public string currentScene;
    public string nextScene;
    public bool isLoading;
    public float progress;
    public bool input_LoadAsyncEnd = false;
    AsyncOperation loadAsync;

    public void LoadScene (string nameScene) { 
        if(nameScene==currentScene) return; 
        SceneManager.LoadScene(nameScene); 
        currentScene=nameScene; 
        nextScene=""; 
    }
    public void LoadScene_Asyncronic (string nameScene, bool Reload) { StartCoroutine(CoroutineLoadScene(nameScene, Reload)); }
    
    public IEnumerator CoroutineLoadScene (string nameToLoad, bool Reload){
        if(nameToLoad!=currentScene || Reload){

            nextScene = nameToLoad; 
            currentScene = nameToLoad; 
            
            isLoading=true;

            yield return new WaitForSecondsRealtime(1f);
            
            SceneManager.LoadScene(SceneLoader_Name);

            yield return new WaitForSecondsRealtime(0.1f);

            loadAsync = SceneManager.LoadSceneAsync(nameToLoad);
            loadAsync.allowSceneActivation = false;

            input_LoadAsyncEnd = false;

            while(!loadAsync.isDone){
                progress=loadAsync.progress*100/0.9f;

                if(progress>=100) {
                    yield return new WaitForSecondsRealtime(timeWaitCharge);

                    GameObject objetLoading_Finded = null;
                    CanvasController canvasController = null;

                    if(ScreenFlow.main.GetScreenData("Loading").objCanvas != null) {
                        objetLoading_Finded = GameObject.Find(ScreenFlow.main.GetScreenData("Loading").objCanvas.name);
                        if(objetLoading_Finded!=null) canvasController = objetLoading_Finded.GetComponent<CanvasController>();
                    }

                    if(currentScene == "Scene_Gameplay") {

                        if(LoadingController.main!=null) LoadingController.main.ReadyToLoad();

                        if(canvasController!=null) canvasController.SetSubScreen("Loaded");

                        while(!input_LoadAsyncEnd) { yield return null; }
                        
                    }

                    if(canvasController!=null) canvasController.SetSubScreen("Close");

                    yield return new WaitForSecondsRealtime(timeWaitCharge);
                    
                    loadAsync.allowSceneActivation = true;
                }

                yield return null;
            }

            isLoading = false;
            
            nextScene="";
        }
    }
}
