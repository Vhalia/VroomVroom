using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private string _currentSceneName;
    [SerializeField]
    private ChapterData _currentChapter;
    [SerializeField]
    private PlayerData _playerData;

    public bool IsPaused { get; private set; }

    private bool _isLoading = false;
    private const string _persistentSceneName = "PersistentScene";
    private const string _chapterSceneName = "FlashCardScene";
    private const string _mainMenuSceneName = "MainMenuScene";

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(_currentSceneName, "Current Scene Name is not assigned.");
        EventBus.Subscribe<ExperienceGainedEventData>(
            EEventType.EXPERIENCE_GAINED,
            OnExperienceGained);
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

    public void LoadChapterScene(ChapterData chapter)
    {
        if (chapter == null)
            Assert.IsNotNull(chapter, "Chapter data cannot be null.");

        _currentChapter = chapter;
        LoadScene(_chapterSceneName);
    }

    public void LoadMainMenu()
    {
        LoadScene(_mainMenuSceneName);
    }

    public ChapterData GetCurrentChapter()
    {
        return _currentChapter;
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
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

    private void OnExperienceGained(ExperienceGainedEventData eventData)
    {
        _playerData.AddExperience(eventData.ExperienceGained);
    }
}
