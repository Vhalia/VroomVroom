using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ScenarioRule
    {
        public EScenarioRuleID id;
        public string title;
        public string description;
        public EMessageType type;
        public bool pauseGame;
        public float displayDuration;
    }

    public enum EScenarioRuleID
    {
        [JsonProperty("RED_LIGHT_VIOLATION")]
        RED_LIGHT_VIOLATION
    }

    public enum EMessageType
    {
        [JsonProperty("SUCCESS")]
        SUCCESS,
        [JsonProperty("INFO")]
        INFO,
        [JsonProperty("WARNING")]
        WARNING,
        [JsonProperty("ERROR")]
        ERROR
    }
}