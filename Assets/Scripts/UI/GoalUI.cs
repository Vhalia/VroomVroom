using Assets.Scripts;
using Assets.Scripts.Models;
using Assets.Scripts.ScriptableObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GoalUI : MonoBehaviour
{
    [SerializeField]
    private Goals _goals;
    [SerializeField]
    private GameObject _goalTextPrefab;
    [SerializeField]
    private GameObject _goalsTextContainer;
    [SerializeField]
    private GameObject _goalsContainer;

    [Header("Automatic resize configuration")]
    [SerializeField]
    private float _heightPadding = 1.75f;
    [SerializeField]
    private float _widthPadding = 1f;

    [Header("Checkbox Configuration")]
    [SerializeField]
    private string _uncheckedSymbol = "[ ]";
    [SerializeField]
    private string _checkedSymbol = "[X]";

    private List<GoalGameObject> _goalComponents = new();

    void Start()
    {
        EventBus.Subscribe<int>(EEventType.GOAL_COMPLETED, OnGoalReached);

        Assert.IsNotNull(_goals, "Goals ScriptableObject is not assigned.");

        InstantiateGoals();

        Canvas.ForceUpdateCanvases();

        var goalTextRect = _goalTextPrefab.GetComponent<RectTransform>();
        var goalsContainerRect = _goalsContainer.GetComponent<RectTransform>();

        UpdateContainerHeight(goalTextRect, goalsContainerRect);

        float maxWidth = FindMaxWidth();

        UpdateWidthToTexts(maxWidth);
        UpdateContainerWidth(goalsContainerRect, maxWidth * _widthPadding);
    }

    private void OnGoalReached(int index)
    {
        CompleteGoal(index);
    }

    public void CompleteGoal(int goalIndex)
    {
        if (goalIndex < 0 || goalIndex >= _goalComponents.Count)
        {
            Debug.LogWarning($"Goal index {goalIndex} is out of range. Available goals: 0-{_goalComponents.Count - 1}");
            return;
        }

        var goalGameObject = _goalComponents[goalIndex];
        if (goalGameObject.IsCompleted)
        {
            Debug.LogWarning($"Goal at index {goalIndex} is already completed.");
            return;
        }

        goalGameObject.IsCompleted = true;
        CrossGoalText(goalGameObject);
    }

    public bool IsGoalCompleted(int goalIndex)
    {
        if (goalIndex < 0 || goalIndex >= _goalComponents.Count)
            return false;

        return _goalComponents[goalIndex].IsCompleted;
    }

    public int? GetNextUncompletedGoalIndex()
    {
        for (int i = 0; i < _goalComponents.Count; i++)
        {
            if (!_goalComponents[i].IsCompleted)
            {
                return i;
            }
        }

        return null;
    }

    private void InstantiateGoals()
    {
        foreach (var goalData in _goals.GoalDatas)
        {
            var goalTextObject = Instantiate(
                _goalTextPrefab,
                _goalsTextContainer.transform);

            var textComponent = goalTextObject.GetComponent<TextMeshProUGUI>();
            var rectTransform = goalTextObject.GetComponent<RectTransform>();

            textComponent.text = $"{_uncheckedSymbol} {goalData.Text}";
            _goalComponents.Add(new GoalGameObject(goalData, textComponent, rectTransform));
        }
    }

    private void UpdateWidthToTexts(float maxWidth)
    {
        var totalWidth = maxWidth * _widthPadding;
        foreach (var goalGameObject in _goalComponents)
        {
            goalGameObject.RectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                totalWidth);
        }
    }

    private float FindMaxWidth()
    {
        float maxWidth = 0f;
        foreach (var goalGameObject in _goalComponents)
        {
            if (goalGameObject.TextComponent.preferredWidth > maxWidth)
            {
                maxWidth = goalGameObject.TextComponent.preferredWidth;
            }
        }

        return maxWidth;
    }

    private void UpdateContainerWidth(RectTransform goalsContainerRect, float width)
    {
        var widthDifference = width - goalsContainerRect.sizeDelta.x;
        goalsContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        goalsContainerRect.anchoredPosition = new Vector2(
            goalsContainerRect.anchoredPosition.x + widthDifference / 2f,
            goalsContainerRect.anchoredPosition.y
        );
    }

    private void UpdateContainerHeight(RectTransform goalTextRect, RectTransform goalsContainerRect)
    {
        var totalHeight = goalTextRect.sizeDelta.y * _goals.GoalDatas.Count * _heightPadding;
        goalsContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    private void CrossGoalText(GoalGameObject goalGameObject)
    {
        goalGameObject.TextComponent.text = $"{_checkedSymbol} <s>{goalGameObject.GoalData.Text}</s>";
        goalGameObject.TextComponent.color = new Color(0.6f, 0.6f, 0.6f, 1f);
    }
    
    public class GoalGameObject
    {
        public GoalData GoalData { get; set; }
        public bool IsCompleted { get; set; }
        public TextMeshProUGUI TextComponent { get; set; }
        public RectTransform RectTransform { get; set; }

        public GoalGameObject(GoalData goalData, TextMeshProUGUI textComponent, RectTransform rectTransform)
        {
            GoalData = goalData;
            TextComponent = textComponent;
            RectTransform = rectTransform;
            IsCompleted = false;
        }
    }
}
