using System.Collections;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("Player Data")]
        [SerializeField] private PlayerData _playerData;

        [Header("UI Components")]
        [SerializeField] private GameObject _levelCompletePanel;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _experienceText;
        [SerializeField] private Slider _experienceBar;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _nextLevelButton;

        [Header("Animation Settings")]
        [SerializeField] private float _xpAnimationDuration = 2f;

        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
            _levelCompletePanel.SetActive(false);

            EventBus.Subscribe(EEventType.GOAL_REACHED, OnLevelComplete);
        }

        void OnDestroy()
        {
            EventBus.Unsubscribe(EEventType.GOAL_REACHED, OnLevelComplete);
        }

        private void InitializeComponents()
        {
            Assert.IsNotNull(_playerData, "Player Data is not assigned.");
            Assert.IsNotNull(_levelCompletePanel, "Level Complete Panel is not assigned.");
            Assert.IsNotNull(_levelText, "Level Text is not assigned.");
            Assert.IsNotNull(_experienceText, "Experience Text is not assigned.");
            Assert.IsNotNull(_experienceBar, "Experience Bar is not assigned.");
            Assert.IsNotNull(_menuButton, "Menu Button is not assigned.");
            Assert.IsNotNull(_restartButton, "Restart Button is not assigned.");
            Assert.IsNotNull(_nextLevelButton, "Next Level Button is not assigned.");

            UpdateUI();
        }

        private void SetupEventListeners()
        {
            _menuButton.onClick.AddListener(OnMenuButtonClick);
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
        }

        private void OnLevelComplete()
        {
            ShowLevelCompleteUI();
        }

        private void ShowLevelCompleteUI()
        {
            _levelCompletePanel.SetActive(true);
            GameManager.Instance.Pause();
            
            StartCoroutine(AnimateExperienceGain());
        }

        private IEnumerator AnimateExperienceGain()
        {
            int startXP = _playerData.CurrentExperience;
            int startLevel = _playerData.CurrentLevel;
            int maxXP = _playerData.CurrentLevelThreshold;
            
            bool leveledUp = _playerData.AddExperience(_playerData.LevelCompleteXP);
            
            if (leveledUp)
            {
                yield return AnimateXPToMax(startXP, maxXP);
                
                UpdateLevelText();
                
                _experienceBar.value = 0;
                yield return AnimateXPFromZero(_playerData.CurrentExperience, _playerData.CurrentLevelThreshold);
            }
            else
            {
                yield return AnimateXP(startXP, _playerData.CurrentExperience, _playerData.CurrentLevelThreshold);
            }

            UpdateUI();
        }

        private IEnumerator AnimateXPToMax(int startXP, int maxXP)
        {
            float elapsed = 0f;
            float halfDuration = _xpAnimationDuration * 0.5f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / halfDuration;
                
                int currentXP = Mathf.RoundToInt(Mathf.Lerp(startXP, maxXP, progress));
                UpdateExperienceDisplay(currentXP, maxXP);
                
                yield return null;
            }
        }

        private IEnumerator AnimateXPFromZero(int targetXP, int maxXP)
        {
            float elapsed = 0f;
            float halfDuration = _xpAnimationDuration * 0.5f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / halfDuration;
                
                int currentXP = Mathf.RoundToInt(Mathf.Lerp(0, targetXP, progress));
                UpdateExperienceDisplay(currentXP, maxXP);
                
                yield return null;
            }
        }

        private IEnumerator AnimateXP(int startXP, int targetXP, int maxXP)
        {
            float elapsed = 0f;

            while (elapsed < _xpAnimationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / _xpAnimationDuration;
                
                int currentXP = Mathf.RoundToInt(Mathf.Lerp(startXP, targetXP, progress));
                UpdateExperienceDisplay(currentXP, maxXP);
                
                yield return null;
            }
        }

        private void UpdateExperienceDisplay(int xp, int maxXP)
        {
            _experienceBar.value = (float)xp / maxXP;
            _experienceText.text = $"{xp} / {maxXP} XP";
        }

        private void UpdateUI()
        {
            UpdateLevelText();
            UpdateExperienceDisplay(_playerData.CurrentExperience, _playerData.CurrentLevelThreshold);
        }

        private void UpdateLevelText()
        {
            _levelText.text = $"Niveau {_playerData.CurrentLevel}";
        }

        private void OnMenuButtonClick()
        {
            Assert.IsFalse(true, "Not yet implemented");
        }

        private void OnRestartButtonClick()
        {
            _levelCompletePanel.SetActive(false);
            GameManager.Instance.Restart();
        }

        private void OnNextLevelButtonClick()
        {
            Assert.IsFalse(true, "Not yet implemented");
        }

        [ContextMenu("Test Level Complete")]
        private void TestLevelComplete()
        {
            OnLevelComplete();
        }
    }
}