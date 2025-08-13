using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Models;

namespace Assets.Scripts.UI
{
    public class AnswerZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Zone Settings")]
        [SerializeField] private EAnswerOption answerOption;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private float highlightAlpha = 0.3f;
        
        private Image _backgroundImage;
        private FlashCardManager _flashCardManager;
        
        public EAnswerOption AnswerOption => answerOption;
        
        private void Awake()
        {
            _backgroundImage = GetComponent<Image>();
            if (_backgroundImage == null)
            {
                _backgroundImage = GetComponentInChildren<Image>();
            }
            
            _flashCardManager = FindObjectOfType<FlashCardManager>();
        }
        
        private void Start()
        {
            if (_backgroundImage != null)
            {
                Color color = normalColor;
                _backgroundImage.color = color;
            }
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            DraggableCard card = eventData.pointerDrag?.GetComponent<DraggableCard>();
            if (card != null && _flashCardManager != null)
            {
                _flashCardManager.OnAnswerSelected(answerOption);
                ResetHighlight();
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<DraggableCard>() != null)
            {
                HighlightZone();
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<DraggableCard>() != null)
            {
                ResetHighlight();
            }
        }
        
        private void HighlightZone()
        {
            if (_backgroundImage != null)
            {
                Color color = highlightColor;
                color.a = highlightAlpha;
                _backgroundImage.color = color;
            }
        }
        
        private void ResetHighlight()
        {
            if (_backgroundImage != null)
            {
                Color color = normalColor;
                _backgroundImage.color = color;
            }
        }
        
        [ContextMenu("Set Answer A")]
        private void SetAnswerA() => answerOption = EAnswerOption.A;
        
        [ContextMenu("Set Answer B")]
        private void SetAnswerB() => answerOption = EAnswerOption.B;
        
        [ContextMenu("Set Answer C")]
        private void SetAnswerC() => answerOption = EAnswerOption.C;
        
        [ContextMenu("Set Answer D")]
        private void SetAnswerD() => answerOption = EAnswerOption.D;
    }
}