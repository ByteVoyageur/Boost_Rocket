using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float thrustStrength = 100f;
    [SerializeField] private float rotationStrength = 100f;

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
