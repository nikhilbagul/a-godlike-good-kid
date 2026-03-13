using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class LoadingManager : MonoBehaviour
{

    public CanvasGroup loadingScreenCanvas;

    private string fileName = "data.sav";
    string path = "";

    public Button loadButton;
    
    void Start()
    {
        if (loadButton == null)
            loadButton = GameObject.Find("LoadGameButton").GetComponent<Button>();
        loadButton.interactable = false;

    #if !UNITY_EDITOR
        path = Application.persistentDataPath + "/";
    #endif
        path += fileName;

        try
        {
            SaveData data = Persistor.Load();
            if (data.levelName != "NULL")
                loadButton.interactable = true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Save data not available yet: " + e.Message);
            // loadButton stays non-interactable, which is fine
        }

        StartCoroutine(DelayedShowCanvas());
    }

    IEnumerator DelayedShowCanvas()
    {
        loadingScreenCanvas.alpha = 0;
        float elapsed = 0;
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            loadingScreenCanvas.alpha += Time.deltaTime;
            yield return null;
        }
        loadingScreenCanvas.alpha = 1;
    }

   public void LoadFromCheckpoint()
    {
        GameManager.Instance.UnsetCamera();
        CheckpointManager.LoadSceneFromCheckpoint();
    }

   public void NewGame()
    {

        //send data
        //DataPoster.instance.SendUserData(new_game_url + GUID + replay_times.ToString() + System.DateTime.Now.ToString());
        GMPersistor.Instance.SetupGUID();

#if UNITY_EDITOR
        URLSetter.NewGameCount(true);
#else
        URLSetter.NewGameCount();
#endif

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        if (loadButton == null)
            loadButton = GameObject.Find("LoadGameButton").GetComponent<Button>();
        loadButton.interactable = false;
        yield return new WaitForSeconds(0.5f);
        CheckpointManager.NewGame();
    }
}
