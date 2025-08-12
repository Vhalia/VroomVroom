using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PermisGame/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Level Settings")]
        [SerializeField] private int _currentLevel = 1;
        [SerializeField] private int _currentExperience = 0;
        [SerializeField] private int _baseExperienceThreshold = 100;
        [SerializeField] private int _levelMultiplier = 20;

        [Header("Experience Rewards")]
        [SerializeField] private int _levelCompleteXP = 50;

        public int CurrentLevel => _currentLevel;
        public int CurrentExperience => _currentExperience;
        public int CurrentLevelThreshold => _baseExperienceThreshold + (_currentLevel - 1) * _levelMultiplier;
        public int LevelCompleteXP => _levelCompleteXP;

        public bool AddExperience(int amount)
        {
            _currentExperience += amount;
            bool leveledUp = false;

            while (_currentExperience >= CurrentLevelThreshold)
            {
                _currentExperience -= CurrentLevelThreshold;
                _currentLevel++;
                leveledUp = true;
            }

            return leveledUp;
        }

        public float GetExperienceProgress()
        {
            return (float)_currentExperience / CurrentLevelThreshold;
        }

        public void ResetProgress()
        {
            _currentLevel = 1;
            _currentExperience = 0;
        }

        [ContextMenu("Add Test Experience")]
        private void AddTestExperience()
        {
            AddExperience(25);
        }

        [ContextMenu("Reset Player Progress")]
        private void ResetPlayerProgress()
        {
            ResetProgress();
        }
    }
}