using System.Collections.Generic;
using Assets.Scripts.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScenarioRules", menuName = "Scenario/Rules")]
    public class ScenarioRulesData : ScriptableObject
    {
        [SerializeField]
        private TextAsset _jsonFile;
        
        private Dictionary<EScenarioRuleID, ScenarioRule> rules;
        
        public void LoadRules()
        {
            if (_jsonFile == null)
                Assert.IsNotNull(_jsonFile, "JSON file is not set for ScenarioRules.");

            var rulesJson = JsonConvert.DeserializeObject<List<ScenarioRule>>(_jsonFile.text);

            rules = new Dictionary<EScenarioRuleID, ScenarioRule>();
            foreach (var rule in rulesJson)
            {
                rules[rule.id] = rule;
            }
        }
        
        public ScenarioRule GetRule(EScenarioRuleID id)
        {
            if (rules == null)
                LoadRules();
                
            return rules.TryGetValue(id, out var rule) ? rule : null;
        }
        
        public void SetJsonFile(TextAsset json)
        {
            _jsonFile = json;
            LoadRules();
        }
    }   
}

