using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetTankShoot : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnLaunchForceChanged))]
    private float m_CurrentLaunchForce = 15f;

    public int m_PlayerNumber = 1;
    //public Rigidbody m_Shell;
    [Header("Fire2")]
    public GameObject m_ShellPrefab;
    public Transform m_FireTransform;

    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;

    private string m_FireButton;
    private float m_ChargeSpeed;
    private bool m_Fired;

    private void OnEnable()
    {
        if (!isLocalPlayer) return;

        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        m_FireButton = "Fire";

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        m_AimSlider.value = m_CurrentLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            CmdFire(m_CurrentLaunchForce);
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            CmdFire(m_CurrentLaunchForce);
        }
    }
    /*
    [Command]
    private void CmdFire(float launchForce)
    {
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        shellInstance.velocity = launchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;

        NetworkServer.Spawn(shellInstance.gameObject);
    }
    */
    [Command]
    private void CmdFire(float launchForce)
    {
        m_Fired = true;

        // Instantiate viên đạn trên server
        GameObject shellInstance = Instantiate(m_ShellPrefab, m_FireTransform.position, m_FireTransform.rotation);
        /*
        Rigidbody shellRigidbody = shellInstance.GetComponent<Rigidbody>();

        if (shellRigidbody != null)
        {
            // Thiết lập vận tốc cho viên đạn
            shellRigidbody.velocity = launchForce * m_FireTransform.forward;
        }

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
        //UnityEngine.Debug.Log("Player heeh!");
        */

        NetworkServer.Spawn(shellInstance); // Đồng bộ hóa viên đạn trên cả client và server
        RpcOnFire(shellInstance, launchForce);
    }


    [ClientRpc]
    void RpcOnFire(GameObject shellInstance, float launchForce)
    {
        Rigidbody shellRigidbody = shellInstance.GetComponent<Rigidbody>();

        if (shellRigidbody != null)
        {
            // Thiết lập vận tốc cho viên đạn
            shellRigidbody.velocity = launchForce * m_FireTransform.forward;
        }

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
        //UnityEngine.Debug.Log("Player heeh!");

    }



    private void OnLaunchForceChanged(float oldForce, float newForce)
    {
        m_CurrentLaunchForce = newForce;
    }
}
