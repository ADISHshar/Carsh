using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    public Light leftBrakeLight;
    public Light rightBrakeLight;
    public float brakeLightIntensity = 5f;

    public float maxMotorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public float brakeForce = 3000f;

    public Text speedometerText; // Reference to the UI Text element for speed

    public AudioSource engineSound; // Reference to the AudioSource for constant engine sound
    public AudioSource accelerationSound; // Reference to the AudioSource for acceleration sound
    public AudioSource coastingSound; // Reference to the AudioSource for coasting sound
    public AudioSource brakingSound; // Reference to the AudioSource for braking sound
    public float maxEnginePitch = 2.0f;

    private Rigidbody rb;
    private Terrain terrain;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.9f, 0);

        terrain = Terrain.activeTerrain;

        // Play the constant engine sound
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.Play();
        }
    }

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteerAngle * Input.GetAxis("Horizontal");

        ApplySteering(steering);

        // If both forward and back buttons are pressed, stop the car
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            ApplyMotorTorque(0);
            ApplyBrakes(brakeForce);
            SetBrakeLights(true);
            UpdateBrakingSound(true);
        }
        else
        {
            ApplyMotorTorque(motor);

            if (Input.GetKey(KeyCode.Space) || (Input.GetKey(KeyCode.S) && motor >= 0))
            {
                ApplyBrakes(brakeForce);
                SetBrakeLights(true);
                UpdateBrakingSound(true);
            }
            else
            {
                ApplyBrakes(0);
                SetBrakeLights(false);
                UpdateBrakingSound(false);
            }

            // Turn on brake lights when reversing
            if (motor < 0)
            {
                SetBrakeLights(true);
            }
        }

        UpdateWheelPoses();
        UpdateSpeedometer();
        UpdateSound();
    }

    void ApplySteering(float steering)
    {
        frontLeftWheel.steerAngle = steering;
        frontRightWheel.steerAngle = steering;
    }

    void ApplyMotorTorque(float motor)
    {
        frontLeftWheel.motorTorque = motor;
        frontRightWheel.motorTorque = motor;
    }

    void ApplyBrakes(float brakeTorque)
    {
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;
    }

    void SetBrakeLights(bool isBraking)
    {
        float intensity = isBraking ? brakeLightIntensity : 0f;
        leftBrakeLight.intensity = intensity;
        rightBrakeLight.intensity = intensity;
    }

    void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateWheelPose(frontRightWheel, frontRightTransform);
        UpdateWheelPose(rearLeftWheel, rearLeftTransform);
        UpdateWheelPose(rearRightWheel, rearRightTransform);
    }

    void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        transform.position = position;
        transform.rotation = rotation;
    }

    void AdjustFrictionForTerrain(WheelCollider wheel)
    {
        WheelHit hit;
        if (wheel.GetGroundHit(out hit))
        {
            TerrainCollider terrainCollider = hit.collider as TerrainCollider;
            if (terrainCollider != null)
            {
                TerrainData terrainData = terrain.terrainData;
                Vector3 terrainPosition = hit.point - terrain.transform.position;
                float[,,] alphas = terrainData.GetAlphamaps(
                    (int)(terrainPosition.x / terrainData.size.x * terrainData.alphamapWidth),
                    (int)(terrainPosition.z / terrainData.size.z * terrainData.alphamapHeight), 
                    1, 1);
                
                float grip = alphas[0, 0, 0]; // Assuming first texture layer is the primary surface
                WheelFrictionCurve forwardFriction = wheel.forwardFriction;
                forwardFriction.stiffness = Mathf.Lerp(0.5f, 1.5f, grip);
                wheel.forwardFriction = forwardFriction;

                WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
                sidewaysFriction.stiffness = Mathf.Lerp(0.5f, 1.5f, grip);
                wheel.sidewaysFriction = sidewaysFriction;
            }
        }
    }

    void Update()
    {
        AdjustFrictionForTerrain(frontLeftWheel);
        AdjustFrictionForTerrain(frontRightWheel);
        AdjustFrictionForTerrain(rearLeftWheel);
        AdjustFrictionForTerrain(rearRightWheel);
    }

    void UpdateSpeedometer()
    {
        float speed = rb.velocity.magnitude * 3.6f; // Convert m/s to km/h
        speedometerText.text = Mathf.RoundToInt(speed) + " km/h";
    }

    void UpdateSound()
    {
        float speed = rb.velocity.magnitude;

        // Update acceleration sound
        if (Input.GetKey(KeyCode.W))
        {
            if (accelerationSound != null)
            {
                accelerationSound.pitch = Mathf.Lerp(1.0f, maxEnginePitch, speed / 100f);
                if (!accelerationSound.isPlaying)
                {
                    accelerationSound.Play();
                }
            }
            if (coastingSound.isPlaying)
            {
                coastingSound.Stop();
            }
        }
        else
        {
            if (accelerationSound.isPlaying)
            {
                accelerationSound.Stop();
            }
            if (speed > 0.1f && !coastingSound.isPlaying)
            {
                coastingSound.Play();
            }
            else if (speed <= 0.1f)
            {
                coastingSound.Stop();
            }
        }

        // Update engine sound
        if (speed <= 0.1f && !Input.GetKey(KeyCode.W))
        {
            if (!engineSound.isPlaying)
            {
                engineSound.Play();
            }
        }
        else
        {
            if (engineSound.isPlaying)
            {
                engineSound.Stop();
            }
        }
    }

    void UpdateBrakingSound(bool isBraking)
    {
        if (brakingSound != null)
        {
            float speed = rb.velocity.magnitude;
            if (isBraking && speed > 0.1f) // Play braking sound only if the car is moving
            {
                if (!brakingSound.isPlaying)
                {
                    brakingSound.Play();
                }
            }
            else
            {
                if (brakingSound.isPlaying)
                {
                    brakingSound.Stop();
                }
            }
        }
    }
}
