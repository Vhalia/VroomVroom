using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Goals", menuName = "Scenario/Goals")]
    public class Goals : ScriptableObject
    {
        [SerializeField]
        private string _title;

        [SerializeField]
        private List<GoalData> _goalDatas = new();

        public string Title => _title;

        public List<GoalData> GoalDatas => _goalDatas;
    }
}
