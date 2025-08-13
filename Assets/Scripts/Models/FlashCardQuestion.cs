using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public enum EAnswerOption
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3
    }
    
    [Serializable]
    public class FlashCardQuestion
    {
        [Header("Question Content")]
        public string title;
        [TextArea(3, 5)]
        public string questionText;
        
        [Header("Answer Options")]
        public string answerA;
        public string answerB;
        public string answerC;
        public string answerD;
        
        [Header("Correct Answer")]
        public EAnswerOption correctAnswer;
        
        [Header("Feedback")]
        [TextArea(2, 4)]
        public string explanation;
        
        public string GetAnswerText(EAnswerOption option)
        {
            switch (option)
            {
                case EAnswerOption.A: return answerA;
                case EAnswerOption.B: return answerB;
                case EAnswerOption.C: return answerC;
                case EAnswerOption.D: return answerD;
                default: return string.Empty;
            }
        }
        
        public bool IsCorrectAnswer(EAnswerOption selectedOption)
        {
            return selectedOption == correctAnswer;
        }
    }
}