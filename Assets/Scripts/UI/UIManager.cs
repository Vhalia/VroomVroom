using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class UIManager : Singleton<UIManager>
{
    public MenuUI MenuUI { get; private set; }
    public RulePopup RulePopup { get; private set; }
    public LevelCompleteUI LevelCompleteUI { get; private set; }

    void Start()
    {
        MenuUI = GetComponentInChildren<MenuUI>();
        RulePopup = GetComponentInChildren<RulePopup>();
        LevelCompleteUI = GetComponentInChildren<LevelCompleteUI>();
    }
}
