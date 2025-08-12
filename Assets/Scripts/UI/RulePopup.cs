using UnityEngine;
using UnityEngine.Assertions;
using Assets.Scripts.Models;
using UnityEngine.UI;
using Assets.Scripts.ScriptableObjects;

namespace Assets.Scripts.UI
{
    public class RulePopup : MonoBehaviour
    {
        [Header("UI References")]
        public TMPro.TMP_Text TitleText;
        public TMPro.TMP_Text DescriptionText;
        public Button RestartButton;

        [Header("Configuration")]
        public ScenarioRulesData rulesData;

        void Awake()
        {
            gameObject.SetActive(false);

            RestartButton.onClick.AddListener(OnClickRestartButton);

            EventBus.Subscribe(
                EEventType.RED_LIGHT_VIOLATION,
                () => ShowRule(EScenarioRuleID.RED_LIGHT_VIOLATION));
        }

        public void ShowRule(EScenarioRuleID ruleId)
        {
            Assert.IsNotNull(rulesData, "ScenarioRulesData is not set in ScenarioRulesUI.");

            var rule = rulesData.GetRule(ruleId);
            if (rule == null)
            {
                Debug.LogError($"Message with ID '{ruleId}' not found!");
                return;
            }

            DisplayRule(rule);
        }

        private void DisplayRule(ScenarioRule rule)
        {
            if (TitleText != null)
                TitleText.text = rule.title;

            if (DescriptionText != null)
                DescriptionText.text = rule.description;

            gameObject.SetActive(true);

            if (rule.pauseGame)
                GameManager.Instance.Pause();
        }

        private void OnClickRestartButton()
        {
            GameManager.Instance.Restart();
            gameObject.SetActive(false);
        }
    }
}