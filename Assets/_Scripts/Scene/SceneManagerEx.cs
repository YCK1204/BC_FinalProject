using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SceneManagerEx : MonoBehaviour
{
    private void Start()
    {
        if (Manager.Scene != null)
            Object.Destroy(Manager.Scene.gameObject);
        else
            Manager.Scene = this;
    }
    public Action OnSceneChanged = null;
    /// <summary>
    /// 동기 씬 전환
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        OnSceneChanged?.Invoke();
    }
    /// <summary>
    /// 비동기 씬 전환
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    /// <param name="onCompleted">씬 전환 완료 콜백</param>
    public void LoadSceneAsync(string sceneName, Action onCompleted = null)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += (op) =>
        {
            onCompleted?.Invoke();
            OnSceneChanged?.Invoke();
        };
    }
    /// <summary>
    /// 로딩 UI용 비동기 씬 전환
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    /// <param name="onCompleted">씬 전환 완료 콜백</param>
    /// <param name="progressCallback">로딩 진행 상황 콜백</param>
    public void LoadSceneAsyncWithAdditive(string sceneName, Action onCompleted = null, Action<float> progressCallback = null)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        operation.allowSceneActivation = false;
        StartCoroutine(LoadAsync(operation, onCompleted, progressCallback));
    }
    IEnumerator LoadAsync(AsyncOperation oper, Action callback, Action<float> progressCallback)
    {
        while (oper.progress < .9f)
        {
            progressCallback?.Invoke(oper.progress);
            yield return new WaitForSeconds(.1f);
        }
        progressCallback?.Invoke(1f);
        yield return null;
        var curScene = SceneManager.GetActiveScene();
        var unloadOper = SceneManager.UnloadSceneAsync(curScene.name);
        yield return unloadOper;
        callback?.Invoke();
        oper.allowSceneActivation = true;
        OnSceneChanged?.Invoke();
    }
    /// <summary>
    /// OnSceneChanged 델리게이트 초기화
    /// </summary>
    public void ClearOnSceneChanged()
    {
        OnSceneChanged = null;
    }
}
