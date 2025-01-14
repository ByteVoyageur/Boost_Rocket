using UnityEngine;
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float thrustStrength = 100f;
    [SerializeField] private float rotationStrength = 100f;

    [Header("Platform Specific Settings")]
    [SerializeField] private float pcThrustStrength = 3000f;
    [SerializeField] private float pcRotationStrength = 100f;
    [SerializeField] private float androidThrustStrength = 400f;
    [SerializeField] private float androidRotationStrength = 10f;
    [SerializeField] private float iosThrustStrength = 400f;
    [SerializeField] private float iosRotationStrength = 10f;

    [Header("Audio & Particles")]
    [SerializeField] private AudioClip mainEngineSFX;
    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem rightThrustParticles;
    [SerializeField] private ParticleSystem leftThrustParticles;

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool isThrusting = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool hasStarted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        // Set platform-specific values
        SetPlatformSpecificValues();
    }

    private void SetPlatformSpecificValues()
    {
        #if UNITY_EDITOR
            thrustStrength = pcThrustStrength;
            rotationStrength = pcRotationStrength;
            Debug.Log($"Platform: Unity Editor - Thrust: {thrustStrength}, Rotation: {rotationStrength}");
        #elif UNITY_STANDALONE_WIN
            thrustStrength = pcThrustStrength;
            rotationStrength = pcRotationStrength;
            Debug.Log($"Platform: Windows - Thrust: {thrustStrength}, Rotation: {rotationStrength}");
        #elif UNITY_ANDROID
            thrustStrength = androidThrustStrength;
            rotationStrength = androidRotationStrength;
            Debug.Log($"Platform: Android - Thrust: {thrustStrength}, Rotation: {rotationStrength}");
        #elif UNITY_IOS
            thrustStrength = iosThrustStrength;
            rotationStrength = iosRotationStrength;
            Debug.Log($"Platform: iOS - Thrust: {thrustStrength}, Rotation: {rotationStrength}");
        #else
            Debug.Log($"Platform: Other - Using default values - Thrust: {thrustStrength}, Rotation: {rotationStrength}");
        #endif
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void FixedUpdate()
    {
        if (isThrusting)
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }

        if (isRotatingLeft)
        {
            RotateLeft();
        }
        else if (isRotatingRight)
        {
            RotateRight();
        }
        else
        {
            StopRotating();
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isThrusting = true;
        }
        else
        {
            isThrusting = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isRotatingLeft = true;
            isRotatingRight = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            isRotatingRight = true;
            isRotatingLeft = false;
        }
        else
        {
            isRotatingLeft = false;
            isRotatingRight = false;
        }
    }

    public void StartThrusting()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            // Example: Start scoring or timer
            ScoreManager.Instance.StartTimer();
        }

        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }

        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    public void RotateLeft()
    {
        ApplyRotation(rotationStrength);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    public void RotateRight()
    {
        ApplyRotation(-rotationStrength);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void StopRotating()
    {
        rightThrustParticles.Stop();
        leftThrustParticles.Stop();
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true; // Disable physics-driven rotation
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false; // Re-enable physics-driven rotation
    }
}
