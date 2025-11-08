using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class AsyncSceneLoader : MonoBehaviour
{
    [Header("UI")]
    public VideoPlayer VideoPlayer;
    public GameObject loadingPanel;

    private void Start() => StartCoroutine(PlayVideo());
    public void LoadScene(string sceneName)
    {
        if (loadingPanel) loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (op.progress >= 0.9f)
                op.allowSceneActivation = true;

            yield return null;
        }

        if (loadingPanel) loadingPanel.SetActive(false);
    }

private IEnumerator PlayVideo()
    {
        VideoPlayer.Prepare();
        yield return new WaitUntil(() => VideoPlayer.isPrepared);  
        VideoPlayer.Play();
    }
}
