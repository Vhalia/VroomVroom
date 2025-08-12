using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

public enum ETrafficLightState
{
    GREEN,
    ORANGE,
    RED
}

public class TrafficLight : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float greenDuration = 5f;
    [SerializeField] private float orangeDuration = 2f;
    [SerializeField] private float redDuration = 5f;

    [Header("Red light detection")]
    [SerializeField] private float rayLength = 10f;
    [SerializeField] private string tagDetection;
    [SerializeField] private bool drawGuizmos = true;
    [SerializeField] private Color rayColor = Color.yellow;

    [Header("Current State")]
    [SerializeField] private ETrafficLightState currentState = ETrafficLightState.GREEN;

    [Header("Light Objects")]
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject orangeLight;
    [SerializeField] private GameObject redLight;


    private Color _inactiveColor = Color.darkGray;
    private Color _greenColor = new(0f, 2f, 0f, 1f);
    private Color _orangeColor = new(3f, 1.5f, 0f, 1f);
    private Color _redColor = new(3f, 0f, 0f, 1f);

    private Renderer _greenRenderer;
    private Renderer _orangeRenderer;
    private Renderer _redRenderer;

    private float _stateTimer;

    private Vector3 _rayPosition => new Vector3(
        transform.position.x,
        0.5f,
        transform.position.z);
    private Vector3 _rayDirection => -1f * transform.right;

    void Start()
    {
        Assert.IsNotNull(greenLight, "Green light is not assigned!");
        Assert.IsNotNull(orangeLight, "Orange light is not assigned!");
        Assert.IsNotNull(redLight, "Red light is not assigned!");

        _greenRenderer = greenLight.GetComponent<Renderer>();
        _orangeRenderer = orangeLight.GetComponent<Renderer>();
        _redRenderer = redLight.GetComponent<Renderer>();

        SetLightState(currentState);
        ResetTimer();
    }

    void Update()
    {
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0f)
        {
            CycleToNextState();
            ResetTimer();
        }

        CastRedLightViolationDetection();
    }

    private void CycleToNextState()
    {
        switch (currentState)
        {
            case ETrafficLightState.GREEN:
                currentState = ETrafficLightState.ORANGE;
                break;
            case ETrafficLightState.ORANGE:
                currentState = ETrafficLightState.RED;
                break;
            case ETrafficLightState.RED:
                currentState = ETrafficLightState.GREEN;
                break;
        }

        SetLightState(currentState);
    }

    private void SetLightState(ETrafficLightState state)
    {
        _greenRenderer.material.color = _inactiveColor;
        _orangeRenderer.material.color = _inactiveColor;
        _redRenderer.material.color = _inactiveColor;

        switch (state)
        {
            case ETrafficLightState.GREEN:
                _greenRenderer.material.color = _greenColor;
                break;
            case ETrafficLightState.ORANGE:
                _orangeRenderer.material.color = _orangeColor;
                break;
            case ETrafficLightState.RED:
                _redRenderer.material.color = _redColor;
                break;
        }
    }

    private void ResetTimer()
    {
        switch (currentState)
        {
            case ETrafficLightState.GREEN:
                _stateTimer = greenDuration;
                break;
            case ETrafficLightState.ORANGE:
                _stateTimer = orangeDuration;
                break;
            case ETrafficLightState.RED:
                _stateTimer = redDuration;
                break;
        }
    }

    private void CastRedLightViolationDetection()
    {
        if (GameManager.Instance.IsPaused ||
            currentState != ETrafficLightState.RED)
        {
            return;        
        }
            
        if (Physics.Raycast(
                _rayPosition,
                _rayDirection,
                out var hit,
                rayLength))
        {
            if (hit.collider.CompareTag(tagDetection))
            {
                // EventBus.TriggerEvent(EEventType.RED_LIGHT_VIOLATION);
                EventBus.TriggerEvent(EEventType.GOAL_COMPLETED, 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGuizmos)
        {
            Gizmos.color = rayColor;
            Gizmos.DrawRay(_rayPosition, _rayDirection * rayLength);
        }
    }
}