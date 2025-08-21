using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Models;
using System.Linq;

namespace Assets.Scripts.UI
{
    public class ChapterManager : MonoBehaviour
    {
        [Header("Current Section")]
        [SerializeField] private SectionData CurrentSection;

        [Header("Prefabs")]
        [SerializeField] private GameObject chapterUIPrefab;
        [SerializeField] private GameObject chapterPathUIPrefab;

        [Header("UI Container")]
        [SerializeField] private RectTransform chapterContainer;

        [Header("Layout Settings")]
        [SerializeField] private float verticalSpacing = 200f;
        [SerializeField] private float horizontalSpacing = 150f;
        [SerializeField] private Vector2 startPosition = new(0f, 0f);

        private RectTransform chapterRect;
        private Dictionary<EChapterId, ChapterData> _chapterIdToData = new();
        private readonly Dictionary<EChapterId, GameObject> _chapterIdToGameObject = new();
        private readonly Dictionary<EChapterId, Vector2> _chapterIdToPosition = new();
        private readonly List<GameObject> _pathUIObjects = new();

        private void Awake()
        {
            Assert.IsNotNull(CurrentSection, "ChapterData is required");
            Assert.IsNotNull(chapterUIPrefab, "ChapterUI prefab is required");
            Assert.IsNotNull(chapterPathUIPrefab, "ChapterPathUI prefab is required");
            Assert.IsNotNull(chapterContainer, "Chapter container is required");

            _chapterIdToData = CurrentSection.chapters.ToDictionary(c => c.ChapterId);
        }

        private void Start()
        {
            chapterRect = chapterUIPrefab.GetComponent<RectTransform>();
            GenerateChapterUI();
        }

        [ContextMenu("Regenerate Chapter UI")]
        public void GenerateChapterUI()
        {
            ClearExistingUI();
            CalculateChapterPositions();
            CreateChapterButtons();
            CreateChapterPaths();
        }
 
        private void ClearExistingUI()
        {
            foreach (var chapterUI in _chapterIdToGameObject.Values)
            {
                if (chapterUI != null)
                    DestroyImmediate(chapterUI);
            }
            _chapterIdToGameObject.Clear();
            _chapterIdToPosition.Clear();

            foreach (var pathUI in _pathUIObjects)
            {
                if (pathUI != null)
                    DestroyImmediate(pathUI);
            }
            _pathUIObjects.Clear();
        }

        private void CalculateChapterPositions()
        {
            if (CurrentSection.chapters == null || CurrentSection.chapters.Count == 0)
                return;

            var levels = BuildChapterLevels();

            for (int levelIndex = 0; levelIndex < levels.Count; levelIndex++)
            {
                List<ChapterData> chaptersInLevel = levels[levelIndex];
                float yPosition = startPosition.y + chapterRect.sizeDelta.y + (levelIndex * verticalSpacing);

                float totalWidth = (chaptersInLevel.Count - 1) * horizontalSpacing;
                float startX = startPosition.x - (totalWidth * 0.5f);

                for (int chapterIndex = 0; chapterIndex < chaptersInLevel.Count; chapterIndex++)
                {
                    var chapter = chaptersInLevel[chapterIndex];
                    float xPosition = startX + (chapterIndex * horizontalSpacing);

                    _chapterIdToPosition[chapter.ChapterId] = new Vector2(xPosition, yPosition);
                }
            }
        }

        private List<List<ChapterData>> BuildChapterLevels()
        {
            var rootChapters = CurrentSection.chapters.FindAll(c => c.IsRoot);
            Assert.IsTrue(rootChapters.Count > 0, "No root chapters found.");

            var levels = new List<List<ChapterData>>();
            var processedChapters = new HashSet<EChapterId>();
            var currentLevel = new List<ChapterData>(rootChapters);

            while (currentLevel.Count > 0)
            {
                levels.Add(new List<ChapterData>(currentLevel));

                foreach (var chapter in currentLevel)
                {
                    processedChapters.Add(chapter.ChapterId);
                }

                var nextLevel = new List<ChapterData>();

                foreach (var chapter in currentLevel)
                {
                    foreach (var connectedId in chapter.ConnectedChapterIds)
                    {
                        if (!processedChapters.Contains(connectedId))
                        {
                            var connectedChapter = _chapterIdToData.GetValueOrDefault(connectedId);
                            if (connectedChapter != null && !nextLevel.Contains(connectedChapter))
                            {
                                nextLevel.Add(connectedChapter);
                            }
                        }
                    }
                }

                currentLevel = nextLevel;
            }

            return levels;
        }

        private void CreateChapterButtons()
        {
            if (CurrentSection.chapters == null)
                return;

            foreach (var chapter in CurrentSection.chapters)
            {
                if (chapter.ChapterId == EChapterId.UNKNOWN)
                    continue;

                var chapterUI = Instantiate(chapterUIPrefab, chapterContainer);
                chapterUI.name = $"Chapter_{chapter.ChapterId}";
                _chapterIdToGameObject[chapter.ChapterId] = chapterUI;

                SetupChapterUI(chapterUI, chapter);
            }
        }

        private void SetupChapterUI(GameObject chapterUI, ChapterData chapter)
        {
            var rectTransform = chapterUI.GetComponent<RectTransform>();
            if (_chapterIdToPosition.TryGetValue(chapter.ChapterId, out Vector2 position))
            {
                rectTransform.anchoredPosition = position;
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }

            var titleText = chapterUI.GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = chapter.Title;
            }

            var backgroundImage = chapterUI.GetComponentInChildren<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = chapter.Color;

                if (chapter.Icon != null)
                {
                    backgroundImage.sprite = chapter.Icon;
                }
            }

            var button = chapterUI.GetComponent<Button>();

            button.interactable = chapter.IsUnlocked;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LoadChapter(chapter.ChapterId));
        }

        private void CreateChapterPaths()
        {
            if (CurrentSection.chapters == null)
                return;

            foreach (var chapter in CurrentSection.chapters)
            {
                if (chapter.ConnectedChapterIds == null || chapter.ConnectedChapterIds.Count == 0)
                    continue;

                // Get connected chapters and sort them by horizontal position
                var connectedChapters = new List<ChapterData>();
                foreach (var connectedId in chapter.ConnectedChapterIds)
                {
                    var connectedChapter = _chapterIdToData.GetValueOrDefault(connectedId);
                    if (connectedChapter != null)
                    {
                        connectedChapters.Add(connectedChapter);
                    }
                }

                foreach (var connectedChapter in connectedChapters)
                {
                    CreatePath(chapter, connectedChapter);
                }
            }
        }

        private void CreatePath(ChapterData fromChapter, ChapterData toChapter)
        {
            if (!_chapterIdToPosition.TryGetValue(fromChapter.ChapterId, out Vector2 fromPosition) ||
                !_chapterIdToPosition.TryGetValue(toChapter.ChapterId, out Vector2 toPosition))
            {
                return;
            }

            var pathUI = Instantiate(chapterPathUIPrefab, chapterContainer);
            pathUI.name = $"Path_{fromChapter.ChapterId}_to_{toChapter.ChapterId}";
            _pathUIObjects.Add(pathUI);

            var rectTransform = pathUI.GetComponent<RectTransform>();

            Vector2 direction = toPosition - fromPosition;

            Vector2 startPoint = new(
                fromPosition.x + direction.x / 2,
                fromPosition.y + chapterRect.sizeDelta.y);
            Vector2 endPoint = new(
                toPosition.x,
                toPosition.y);
            rectTransform.anchoredPosition = startPoint;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            direction = endPoint - startPoint;
            float distance = direction.magnitude;

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, distance);

            pathUI.transform.SetSiblingIndex(0);
        }

        public void UnlockChapter(EChapterId chapterId)
        {
            var chapter = _chapterIdToData.GetValueOrDefault(chapterId);
            if (chapter != null)
            {
                chapter.IsUnlocked = true;

                if (_chapterIdToGameObject.TryGetValue(chapterId, out var chapterUI))
                {
                    SetupChapterUI(chapterUI, chapter);
                }
            }
        }

        public void CompleteChapter(EChapterId chapterId)
        {
            var chapter = _chapterIdToData.GetValueOrDefault(chapterId);
            if (chapter != null)
            {
                chapter.IsCompleted = true;

                foreach (var connectedId in chapter.ConnectedChapterIds)
                {
                    UnlockChapter(connectedId);
                }
            }
        }

        private void LoadChapter(EChapterId chapterId)
        {
            GameManager.Instance.LoadChapterScene(_chapterIdToData.GetValueOrDefault(chapterId));
        }
    }
}