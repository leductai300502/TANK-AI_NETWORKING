using UnityEngine;
using Mirror;

public class NetTankMove : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMovementInputChanged))]
    private float m_MovementInputValue = 0f;

    [SyncVar(hook = nameof(OnTurnInputChanged))]
    private float m_TurnInputValue = 0f;
    public int m_PlayerNumber;
    public float m_Speed = 0.1f;
    public float m_TurnSpeed = 1f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;

    private Rigidbody m_Rigidbody;
    private float m_OriginalPitch;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_OriginalPitch = m_MovementAudio.pitch;

        if (isLocalPlayer)
        {
            m_MovementAudio.spatialBlend = 0f; // Ensures the audio plays for local player only
        }
    }

    private void OnMovementInputChanged(float oldValue, float newValue)
    {
        m_MovementInputValue = newValue;
        Move();
    }

    private void OnTurnInputChanged(float oldValue, float newValue)
    {
        m_TurnInputValue = newValue;
        Turn();
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;

        // Lấy giá trị input từ bàn phím
        float movementInput = Input.GetAxis("Vertical2");
        float turnInput = Input.GetAxis("Horizontal2");

        // Gọi phương thức xử lý input và gửi nó đến server
        HandleInput(movementInput, turnInput);
        Debug.Log("Transform of Tank: " + transform.position);

        //m_CameraControl.target = transform;
    }

    public Transform GetTankTransform()
    {
        return transform;
    }

    private void HandleInput(float movementInput, float turnInput)
    {
        // Cập nhật trạng thái movement và turn local trước khi gửi đến server
        m_MovementInputValue = movementInput;
        m_TurnInputValue = turnInput;

        // Gửi input tới server thông qua Command
        CmdProvideInputToServer(m_MovementInputValue, m_TurnInputValue);

        // Gọi các phương thức xử lý movement và turn ở đây, thay vì gọi trực tiếp từ hook
        Move();
        Turn();
    }

    [Command]
    private void CmdProvideInputToServer(float movementInput, float turnInput)
    {
        // Nhận input từ client và cập nhật trạng thái movement và turn trên server
        m_MovementInputValue = movementInput;
        m_TurnInputValue = turnInput;
    }

    private void EngineAudio()
    {
        // Audio logic remains the same
        // ...
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
