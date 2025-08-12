using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject continueButton;
    [SerializeField]
    private GameObject restartButton;

    private bool _isVisible;
    private InputAction _pauseInput;

    void Awake()
    {
        Assert.IsNotNull(continueButton, "Continue button is not assigned in the Menu.");
        Assert.IsNotNull(restartButton, "Restart button is not assigned in the Menu.");

        _pauseInput = new InputAction(binding: "<Keyboard>/escape");
        _pauseInput.performed += OnPausePressed;
        _pauseInput.Enable();
    }

    void Start()
    {
        Hide();
    }

    void OnPausePressed(InputAction.CallbackContext context)
    {
        if (_isVisible)
            OnPressContinue();
        else
            Show();
    }

    public void OnPressContinue()
    {
        GameManager.Instance.Continue();
        Hide();
    }

    public void OnPressRestart()
    {
        GameManager.Instance.Restart();
        Hide();
    }
    
    public void Show()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(true);
            _isVisible = true;
        }
    }

    public void Hide()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            _isVisible = false;
        }
    }
}