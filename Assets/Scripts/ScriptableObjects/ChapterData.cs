using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "PermisGame/Chapter Data")]
public class ChapterData : ScriptableObject
{
    [Header("Chapter Information")]
    public EChapterId ChapterId;
    public string Title;
    public bool IsUnlocked = false;
    public bool IsCompleted = false;
    public bool IsRoot = false;
    [Header("Rewards")]
    public int ExperienceReward = 100;

    [Header("Connections")]
    public List<EChapterId> ConnectedChapterIds = new();

    [Header("Visual Settings")]
    public Color Color = Color.white;
    public Sprite Icon;

    [Header("Questions in Chapter")]
    public List<FlashCardQuestion> Questions = new();
}
