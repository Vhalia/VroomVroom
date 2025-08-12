using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private const string _persistentSceneName = "PersistentScene";
    private string _currentSceneName;
    public bool IsPaused { get; private set; }
    private bool _isLoading = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        _currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        if (!SceneManager.GetSceneByName(_persistentSceneName).isLoaded)
        {
            SceneManager.LoadScene(_persistentSceneName, LoadSceneMode.Additive);
        }
    }

    public void Continue()
    {
        if (IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            Time.timeScale = 0f;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading)
            return;

        StartCoroutine(LoadGameplaySceneCoroutine(sceneName));
    }

    public void Restart()
    {
        if (string.IsNullOrEmpty(_currentSceneName))
            return;

        LoadScene(_currentSceneName);
    }

    private IEnumerator LoadGameplaySceneCoroutine(string sceneName)
    {
        _isLoading = true;

        if (!string.IsNullOrEmpty(_currentSceneName))
        {
            var unloadOperation = SceneManager.UnloadSceneAsync(_currentSceneName);
            yield return unloadOperation;
        }

        var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loadOperation;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        _currentSceneName = sceneName;

        _isLoading = false;
        Continue();
    }
}
