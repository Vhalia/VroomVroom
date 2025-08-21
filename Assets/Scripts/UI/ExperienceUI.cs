using System.Collections;
using Assets.Scripts;
using Assets.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [SerializeField]
    private TextMeshProUGUI _experienceText;
    [SerializeField]
    private Slider _experienceBar;

    [Header("Animation Settings")]
    [SerializeField] private float _xpAnimationDuration = 2f;

    void Awake()
    {
        EventBus.Subscribe<ExperienceGainedEventData>(
            EEventType.EXPERIENCE_GAINED,
            OnExperienceGained);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ExperienceGainedEventData>(
            EEventType.EXPERIENCE_GAINED,
            OnExperienceGained);
    }

    private void OnExperienceGained(ExperienceGainedEventData eventData)
    {
        int experienceGained = eventData.ExperienceGained;
        int currentExperience = eventData.CurrentExperience;
        int currentLevel = eventData.CurrentLevel;
        int currentLevelThreshold = eventData.CurrentLevelThreshold;

        StartCoroutine(AnimateExperienceGain(
            experienceGained,
            currentExperience,
            currentLevel,
            currentLevelThreshold
        ));
    }

    private IEnumerator AnimateExperienceGain(
        int experienceGained,
        int currentExperience,
        int currentLevel,
        int currentLevelThreshold)
    {
        int startXP = currentExperience;
        int startLevel = currentLevel;
        int maxXP = currentLevelThreshold;

        bool leveledUp = currentExperience + experienceGained >= currentLevelThreshold;

        if (leveledUp)
        {
            yield return AnimateXPToMax(startXP, maxXP);

            UpdateLevelText(currentLevel + 1);

            _experienceBar.value = 0;
            yield return AnimateXPFromZero(currentExperience, currentLevelThreshold);
        }
        else
        {
            yield return AnimateXP(startXP, currentExperience, currentLevelThreshold);
        }

        UpdateUI(currentExperience, currentLevelThreshold, currentLevel);
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
        if (_experienceText != null)
            _experienceText.text = $"{xp} / {maxXP} XP";
    }
    
    private void UpdateUI(int currentExperience, int currentLevelThreshold, int currentLevel)
    {
        UpdateLevelText(currentLevel);
        UpdateExperienceDisplay(currentExperience, currentLevelThreshold);
    }

    private void UpdateLevelText(int currentLevel)
    {
        _levelText.text = $"Niveau {currentLevel}";
    }
}
