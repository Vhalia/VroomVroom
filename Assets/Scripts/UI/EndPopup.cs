using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EndPopup : MonoBehaviour
{
    [SerializeField]
    private Button _rejectButton;
    [SerializeField]
    private Button _acceptButton;

    void Start()
    {
        gameObject.SetActive(false);

        EventBus.Subscribe(EEventType.GOAL_REACHED, OnEventReceived);

        Assert.IsNotNull(_rejectButton, "Reject button is not assigned.");
        Assert.IsNotNull(_acceptButton, "Accept button is not assigned.");

        _rejectButton.onClick.AddListener(OnRejectButtonClick);
        _acceptButton.onClick.AddListener(OnAcceptButtonClick);
    }

    private void OnEventReceived()
    {
        gameObject.SetActive(true);
        GameManager.Instance.Pause();
    }

    private void OnRejectButtonClick()
    {
        gameObject.SetActive(false);
        //TODO: Navigate to menu
        GameManager.Instance.Restart();
    }

    private void OnAcceptButtonClick()
    {
        gameObject.SetActive(false);
        //TODO: Navigate to next scenario/question
        GameManager.Instance.Restart();
    }

}
