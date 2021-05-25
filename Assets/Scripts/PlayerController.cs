using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private float speed = 5f;
    [SerializeField] 
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thresterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;
    public bool isJumping = false;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    [Header("Sounds:")]
    [SerializeField]
    private AudioSource flySound;

    // Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    // Visual
    private bool isFullScreen;
    private bool isVisableCursor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        // Setting target position for spring
        // This make the physics acr right when it comes to applying gravity when flying over objects
        RaycastHit _hit;
        float _range = 100f;

        if (Physics.Raycast(transform.position, Vector3.down, out _hit, _range))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        
        // Calculate movement velocity as a 3D vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // Final Movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        // Animate Movement
        animator.SetFloat("ForwardVelocity", _zMov);

        // Vocalize Movement
        PlayMoveEffect(_velocity);

        // Apply movement
        motor.Move(_velocity);

        // Calculate rotation as a 3D vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        // Apply rotation
        motor.Rotate(_rotation);

        // Calculate camera rotation as a 3D vector (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity;

        // Apply camera rotation
        motor.RotateCamera(_cameraRotationX);

        // Calculate the thrusterforce based on player input
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump") &&
            thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
        }
        else
        {
                thrusterFuelAmount += thresterFuelRegenSpeed * Time.deltaTime;
                SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        // Apply the thruster force
        motor.ApplyThruster(_thrusterForce);

        // Show/Hide cursor
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
        //else if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //}
        //else if (Input.GetKeyDown(KeyCode.F3))
        //{
        //    Screen.fullScreen = true;
        //}
        //else if (Input.GetKeyDown(KeyCode.F4))
        //{
        //    Screen.fullScreen = false;
        //}
        if (Input.GetKeyUp(KeyCode.F1))
        {
            if (isFullScreen)
            {
                Screen.SetResolution(800, 600, FullScreenMode.Windowed);
            }
            else
            {
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            }
            isFullScreen = !isFullScreen;

        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            if (isVisableCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            isVisableCursor = !isVisableCursor;
        }
    }

    /// <summary>
    /// Set the force of gravity
    /// </summary>
    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive 
        {
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce 
        };
    }

    private void PlayMoveEffect(Vector3 _velocity)
    {
        if (!flySound.isPlaying)
        {
            if (_velocity != Vector3.zero)
            {
                flySound.Play();
            }
        }
    }

    public void Jump(float _force)
    {
        Vector3 _thrusterForce = Vector3.zero;
        _thrusterForce = Vector3.up * _force;
        SetJointSettings(0f);
        motor.ApplyThruster(_thrusterForce);
    }
}
