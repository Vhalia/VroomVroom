using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Assets.Scripts.UI
{
    public class DraggableCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Drag Settings")]
        [SerializeField] private float dragScale = 1.1f;
        [SerializeField] private float dragAlpha = 0.8f;
        
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _originalPosition;
        private Vector3 _originalScale;
        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
            _graphicRaycaster = _canvas.GetComponent<GraphicRaycaster>();
            
            Assert.IsNotNull(_rectTransform, "RectTransform component missing");
            Assert.IsNotNull(_canvas, "Canvas component missing in parent");
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        private void Start()
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _originalScale = _rectTransform.localScale;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = dragAlpha;
            _canvasGroup.blocksRaycasts = false;
            _rectTransform.localScale = _originalScale * dragScale;
            
            transform.SetAsLastSibling();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, 
                eventData.position, 
                _canvas.worldCamera, 
                out localPointerPosition))
            {
                _rectTransform.anchoredPosition = localPointerPosition;
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.localScale = _originalScale;
            
            bool droppedOnAnswerZone = false;
            
            if (eventData.pointerEnter != null)
            {
                AnswerZone answerZone = eventData.pointerEnter.GetComponentInParent<AnswerZone>();
                if (answerZone != null)
                {
                    droppedOnAnswerZone = true;
                }
            }
            
            if (!droppedOnAnswerZone)
            {
                ReturnToOriginalPosition();
            }
        }
        
        public void ReturnToOriginalPosition()
        {
            _rectTransform.anchoredPosition = _originalPosition;
        }
        
        public void SetOriginalPosition(Vector2 position)
        {
            _originalPosition = position;
            _rectTransform.anchoredPosition = position;
        }
    }
}