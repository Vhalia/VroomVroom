using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class FlashCardManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TextMeshProUGUI answerAText;
        [SerializeField] private TextMeshProUGUI answerBText;
        [SerializeField] private TextMeshProUGUI answerCText;
        [SerializeField] private TextMeshProUGUI answerDText;
        [SerializeField] private DraggableCard draggableCard;

        [Header("Answer Zones")]
        [SerializeField] private AnswerZone answerZoneA;
        [SerializeField] private AnswerZone answerZoneB;
        [SerializeField] private AnswerZone answerZoneC;
        [SerializeField] private AnswerZone answerZoneD;

        [Header("Feedback")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button nextButton;

        private int _currentQuestionIndex = 0;
        private FlashCardQuestion _currentQuestion;
        private ChapterData _currentChapter;

        private void Awake()
        {
            ValidateReferences();
        }

        private void Start()
        {
            _currentChapter = GameManager.Instance.GetCurrentChapter();
            _currentQuestionIndex = 0;

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextQuestion);
            }

            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(false);
            }

            LoadCurrentQuestion();
        }

        private void ValidateReferences()
        {
            Assert.IsNotNull(titleText, "Title text reference missing");
            Assert.IsNotNull(questionText, "Question text reference missing");
            Assert.IsNotNull(draggableCard, "Draggable card reference missing");
        }

        public void OnAnswerSelected(EAnswerOption selectedAnswer)
        {
            if (_currentQuestion == null) return;

            bool isCorrect = _currentQuestion.IsCorrectAnswer(selectedAnswer);
            ShowFeedback(isCorrect, selectedAnswer);

            draggableCard.ReturnToOriginalPosition();
        }

        private void ShowFeedback(bool isCorrect, EAnswerOption selectedAnswer)
        {
            if (feedbackPanel == null || feedbackText == null) return;

            string feedbackMessage = isCorrect ? "Correct!" : "Incorrect.";

            if (!string.IsNullOrEmpty(_currentQuestion.explanation))
            {
                feedbackMessage += "\n\n" + _currentQuestion.explanation;
            }

            feedbackMessage += $"\nVotre réponse : {selectedAnswer}";
            feedbackMessage += $"\nRéponse correcte : {_currentQuestion.correctAnswer}";

            feedbackText.text = feedbackMessage;
            feedbackPanel.SetActive(true);
        }

        private void LoadCurrentQuestion()
        {
            if (_currentChapter.Questions.Count == 0 || _currentQuestionIndex >= _currentChapter.Questions.Count)
            {
                Debug.LogWarning("No questions available or index out of range");
                return;
            }

            _currentQuestion = _currentChapter.Questions[_currentQuestionIndex];

            if (titleText != null)
                titleText.text = _currentQuestion.title;

            if (questionText != null)
                questionText.text = _currentQuestion.questionText;

            if (answerAText != null)
                answerAText.text = _currentQuestion.answerA;

            if (answerBText != null)
                answerBText.text = _currentQuestion.answerB;

            if (answerCText != null)
                answerCText.text = _currentQuestion.answerC;

            if (answerDText != null)
                answerDText.text = _currentQuestion.answerD;

            if (feedbackPanel != null)
                feedbackPanel.SetActive(false);
        }

        public void NextQuestion()
        {
            if (_currentQuestionIndex == _currentChapter.Questions.Count - 1)
            {
                GameManager.Instance.LoadMainMenu();
                //TODO: Add reward scene
                var playerData = GameManager.Instance.GetPlayerData();
                EventBus.TriggerEvent(
                    EEventType.EXPERIENCE_GAINED,
                    new ExperienceGainedEventData()
                    {
                        ExperienceGained = _currentChapter.ExperienceReward,
                        CurrentExperience = playerData.CurrentExperience,
                        CurrentLevel = playerData.CurrentLevel,
                        CurrentLevelThreshold = playerData.CurrentLevelThreshold
                    }
                );
                return;
            }

            _currentQuestionIndex = Mathf.Min(
                _currentQuestionIndex + 1,
                _currentChapter.Questions.Count - 1);

            LoadCurrentQuestion();
        }

        public void PreviousQuestion()
        {
            _currentQuestionIndex = Mathf.Max(0, _currentQuestionIndex - 1);
            LoadCurrentQuestion();
        }
    }
}