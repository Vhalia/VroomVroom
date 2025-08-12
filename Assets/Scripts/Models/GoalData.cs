using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class GoalData
    {
        [SerializeField]
        private string _text;
        [SerializeField]
        private int _xpToGive;

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public int XpToGive
        {
            get => _xpToGive;
            set => _xpToGive = value;
        }

        public GoalData()
        {
            _text = string.Empty;
            _xpToGive = 0;
        }

        public GoalData(string text, int xpToGive)
        {
            _text = text;
            _xpToGive = xpToGive;
        }
    }
}