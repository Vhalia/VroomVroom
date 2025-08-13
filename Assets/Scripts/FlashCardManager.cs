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
        
        [Header("Questions")]
        [SerializeField] private List<FlashCardQuestion> questions = new List<FlashCardQuestion>();
        
        [Header("Feedback")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button nextButton;
        
        private int _currentQuestionIndex = 0;
        private FlashCardQuestion _currentQuestion;
        
        private void Awake()
        {
            ValidateReferences();
        }
        
        private void Start()
        {
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
            if (questions.Count == 0 || _currentQuestionIndex >= questions.Count)
            {
                Debug.LogWarning("No questions available or index out of range");
                return;
            }
            
            _currentQuestion = questions[_currentQuestionIndex];
            
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
            _currentQuestionIndex = (_currentQuestionIndex + 1) % questions.Count;
            LoadCurrentQuestion();
        }
        
        public void PreviousQuestion()
        {
            _currentQuestionIndex = (_currentQuestionIndex - 1 + questions.Count) % questions.Count;
            LoadCurrentQuestion();
        }
        
        [ContextMenu("Add Sample Question")]
        private void AddSampleQuestion()
        {
            FlashCardQuestion sampleQuestion = new FlashCardQuestion
            {
                title = "Règle de Priorité",
                questionText = "Que devez-vous faire à un feu rouge?",
                answerA = "Continuer prudemment",
                answerB = "S'arrêter complètement",
                answerC = "Ralentir seulement",
                answerD = "Klaxonner et passer",
                correctAnswer = EAnswerOption.B,
                explanation = "À un feu rouge, vous devez toujours vous arrêter complètement avant la ligne d'arrêt."
            };
            
            questions.Add(sampleQuestion);
        }
    }
}