using UnityEngine;
using UnityEngine.InputSystem;

public class MobilePlayerController : MonoBehaviour
{
    [Header("Car Physics")]
    public float maxSpeed = 15f;
    public float acceleration = 8f;
    public float deceleration = 12f;
    public float turnSpeed = 120f;
    public float minSpeedForTurning = 0.5f;
    
    [Header("Touch Settings")]
    public float touchSensitivity = 2f;
    
    [Header("Steering Wheel")]
    public Transform steeringWheel;
    public float steeringWheelMaxRotation = 45f;
    
    [Header("Car Behavior")]
    public float dragCoefficient = 0.98f;
    
    private Rigidbody rb;
    private bool isTouching = false;
    private Vector2 touchStartPosition;
    private Vector2 currentTouchPosition;
    private float horizontalInput = 0f;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("MobilePlayerController requires a Rigidbody component!");
        }
        
        // Configure Rigidbody for car-like behavior
        if (rb != null)
        {
            rb.linearDamping = 0.3f;
            rb.angularDamping = 3f;
            rb.mass = 1000f; // Heavier for car-like feel
            rb.centerOfMass = new Vector3(0, -0.5f, 0.2f); // Lower center of mass
        }
        
        // Auto-find steering wheel
        if (steeringWheel == null)
        {
            Transform foundWheel = transform.Find("SteeringWheel");
            if (foundWheel != null)
            {
                steeringWheel = foundWheel;
            }
        }
    }
    
    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
        UpdateSteeringWheel();
    }
    
    void FixedUpdate()
    {
        HandleCarMovement();
    }
    
    void HandleCarMovement()
    {
        // Determine target speed based on input
        if (isTouching)
        {
            targetSpeed = maxSpeed;
        }
        else
        {
            targetSpeed = 0f;
        }
        
        // Smooth acceleration/deceleration
        if (currentSpeed < targetSpeed)
        {
            // Accelerating
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, targetSpeed);
        }
        else if (currentSpeed > targetSpeed)
        {
            // Decelerating
            currentSpeed -= deceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Max(currentSpeed, targetSpeed);
        }
        
        // Apply drag when not accelerating
        if (!isTouching && currentSpeed > 0)
        {
            currentSpeed *= dragCoefficient;
            if (currentSpeed < 0.1f) currentSpeed = 0f;
        }
        
        // Move and steer
        if (currentSpeed > 0.1f)
        {
            Vector3 moveDirection = transform.forward * currentSpeed;
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
            
            // Handle steering (only when moving fast enough)
            if (currentSpeed > minSpeedForTurning && Mathf.Abs(horizontalInput) > 0.1f)
            {
                // Turn rate based on speed (slower turning at higher speeds)
                float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
                float adjustedTurnSpeed = turnSpeed * (1f - speedFactor * 0.3f);
                
                float turnAmount = horizontalInput * adjustedTurnSpeed * Time.fixedDeltaTime;
                transform.Rotate(0, turnAmount, 0);
            }
        }
        else
        {
            // Stop the car completely
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    
    void UpdateSteeringWheel()
    {
        if (steeringWheel != null)
        {
            float wheelRotation = horizontalInput * steeringWheelMaxRotation;
            
            if (currentSpeed > maxSpeed * 0.7f)
            {
                float shake = Mathf.Sin(Time.time * 20f) * 1.1f * (currentSpeed / maxSpeed);
                wheelRotation += shake;
            }
            
            steeringWheel.localRotation = Quaternion.Euler(-wheelRotation, -90f, -90f);
        }
    }
    
    void HandleTouchInput()
    {
        var touchscreen = Touchscreen.current;
        
        if (touchscreen != null && touchscreen.added && touchscreen.touches.Count > 0)
        {
            var touch = touchscreen.touches[0];
            
            if (touch != null && touch.press != null)
            {
                bool touchPressed = touch.press.isPressed;
                
                if (touchPressed)
                {
                    Vector2 touchPosition = touch.position.ReadValue();
                    
                    if (!isTouching)
                    {
                        isTouching = true;
                        touchStartPosition = touchPosition;
                        currentTouchPosition = touchPosition;
                        horizontalInput = 0f;
                    }
                    else
                    {
                        currentTouchPosition = touchPosition;
                        Vector2 touchDelta = currentTouchPosition - touchStartPosition;
                        horizontalInput = (touchDelta.x / Screen.width) * touchSensitivity;
                        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
                    }
                }
                else
                {
                    isTouching = false;
                    horizontalInput = 0f;
                }
            }
        }
        else
        {
            // Gradually return steering to center when no input
            horizontalInput = Mathf.Lerp(horizontalInput, 0f, Time.deltaTime * 5f);
        }
    }
    
    void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                isTouching = true;
                touchStartPosition = mouse.position.ReadValue();
                currentTouchPosition = mouse.position.ReadValue();
                horizontalInput = 0f;
            }
            else if (mouse.leftButton.isPressed)
            {
                currentTouchPosition = mouse.position.ReadValue();
                Vector2 mouseDelta = currentTouchPosition - touchStartPosition;
                horizontalInput = (mouseDelta.x / Screen.width) * touchSensitivity;
                horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                isTouching = false;
                horizontalInput = 0f;
            }
        }
        
        // Gradually return steering to center when no input
        if (!isTouching)
        {
            horizontalInput = Mathf.Lerp(horizontalInput, 0f, Time.deltaTime * 5f);
        }
    }
    
    // Public methods for accessing car data
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    public float GetSpeedPercentage()
    {
        return currentSpeed / maxSpeed;
    }
}