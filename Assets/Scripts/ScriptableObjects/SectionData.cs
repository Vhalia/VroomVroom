using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Models;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SectionData", menuName = "PermisGame/Section Data")]
    public class SectionData : ScriptableObject
    {
        [Header("Section Information")]
        public string Title;

        [Header("Chapters in section")]
        public List<ChapterData> chapters = new();
    }
}