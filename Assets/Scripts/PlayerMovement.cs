using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun.Demo.Cockpit;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour, IPunObservable
{
    [SerializeField]
    public float speed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance;

    public VariableJoystick variableJoystick;
    public Animator robotAnimator;
    public GameObject arCameraGameObject;

    public int id;
    public Text playerNickName;
    public Player photonPlayer;

    public LayerMask groundLayer;

    private Rigidbody m_RigidBody;
    private CapsuleCollider m_CapsuleCollider;
    private bool m_Grounded = true;
    private float m_JumpInitHeight;
    private const int m_MaxHealth = 2000;
    private int m_PlayerHealth = m_MaxHealth;
    private int m_OtherPlayerHealth = m_MaxHealth;
    private readonly int m_DamagePerBullet = 200;
    private PhotonView m_PhotonView;

    public UnityEngine.UI.Slider CurrentPlayerHealthManager;
    public UnityEngine.UI.Slider OtherPlayerHealthManager;

    private bool m_OtherUIVisible = true;


    [PunRPC]
    public void Initialize(Player thePlayer)
    {
        photonPlayer = thePlayer;
        id = thePlayer.ActorNumber;
        GameManager.instance.players[id - 1] = this;
    }

    void Start()
    {
        CurrentPlayerHealthManager.maxValue = m_MaxHealth;
        OtherPlayerHealthManager.maxValue = m_MaxHealth;

        CurrentPlayerHealthManager.value = m_MaxHealth;
        OtherPlayerHealthManager.value = m_MaxHealth;

        arCameraGameObject = GameObject.FindGameObjectWithTag("Camera");
        Debug.LogWarning("Player Movement Start");
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_JumpInitHeight = transform.position.y;
        m_PhotonView = GetComponent<PhotonView>();
        // playerNickName.text = photonPlayer.NickName;

        Debug.LogWarning("Jump Initial Height" + m_JumpInitHeight);
    }

    void FixedUpdate()
    {
        Debug.LogWarning("Jump Initial Height" + m_JumpInitHeight);
        Debug.LogWarning("Current player position " + transform.position);
        Debug.LogWarning("Player Movement Local status " + photonPlayer.IsLocal);
        if (photonPlayer.IsLocal)
        {
            Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

            if (transform.position.y > m_JumpInitHeight)
            {
                direction = Vector3.zero;
            }
            if (transform.position.y == m_JumpInitHeight)
            {
                m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }


            if (direction == Vector3.zero)
            {
                robotAnimator.enabled = false;
            }
            else
            {
                robotAnimator.enabled = true;
            }
            transform.Translate(speed * Time.fixedDeltaTime * direction);

            // m_Grounded = transform.position.y == m_JumpInitHeight;
            m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_CapsuleCollider.bounds.extents.y + groundCheckDistance, groundLayer);
            Debug.LogWarning("Grounded status " + m_Grounded);

            if(transform.position.y + 5 < m_JumpInitHeight)
            {
                Instantiate(Resources.Load<GameObject>("DeathScreenCanvasLost"));

                object[] eventData = new object[] { true };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCache };
                PhotonNetwork.RaiseEvent(2, eventData, options, SendOptions.SendReliable);
            }
            OtherPlayerHealthManager.value = m_OtherPlayerHealth;
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            Debug.LogWarning("Sending Current Health " + m_PlayerHealth);
            stream.SendNext(m_PlayerHealth);
        }
        else
        {
            // Network player, receive data
            m_OtherPlayerHealth = (int)stream.ReceiveNext();
            Debug.LogWarning("Receiving other player health " + m_OtherPlayerHealth);
            OtherPlayerHealthManager.value = m_OtherPlayerHealth;
        }
    }

    public void CallFire()
    {
        Debug.LogWarning("In CallFireFunction");
        Debug.LogWarning("Player Local status " + photonPlayer.IsLocal);

        if (m_OtherUIVisible && !photonPlayer.IsLocal)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).gameObject.CompareTag("PlayerScreenCanvas"))
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            m_OtherUIVisible = false;
        }

        if (photonPlayer.IsLocal)
        {
            m_PhotonView.RPC("Fire", RpcTarget.All);
            Debug.LogWarning("Fire Button pressed by " + photonPlayer.NickName);
        }
    }


    // [PunRPC]
    public void Jump()
    {
        Debug.LogWarning("In Jump function");
        Debug.LogWarning("Player Locality status " + photonPlayer.IsLocal);
        Debug.LogWarning("Player grounded status " + m_Grounded);

        if (m_OtherUIVisible && !photonPlayer.IsLocal)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.CompareTag("PlayerScreenCanvas"))
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            m_OtherUIVisible = false;
        }

        if (photonPlayer.IsLocal && m_Grounded)
        {
            m_RigidBody.constraints = ~RigidbodyConstraints.FreezePositionY;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            m_RigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }

    [PunRPC]
    public void Fire()
    {
        Debug.LogWarning("In Fire function");

        RaycastHit hit;
        if(Physics.Raycast(arCameraGameObject.transform.position, arCameraGameObject.transform.forward, out hit))
        {
            if(hit.transform.CompareTag("Plane") || hit.transform.CompareTag("Player"))
            {
                Vector3 direction;

                GameObject bullet = Instantiate(Resources.Load<GameObject>("bullet"));
                bullet.name = photonPlayer.NickName;
                Rigidbody bulletRigidBody = bullet.GetComponent<Rigidbody>();

                Debug.Log("Hit Position: " + hit.transform.position);
                Debug.Log("Player Position: " + transform.position);

                
                direction = hit.transform.position - transform.position;
                direction.Normalize();
                
                bullet.transform.localPosition = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
                bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                Debug.Log("Shoot Direction: " + direction);
                bulletRigidBody.AddForce(direction * 300f);
                
                Destroy(bullet, 10);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("bullet"))
        {
            if(other.name != photonPlayer.NickName)
            {
                if(m_PlayerHealth - m_DamagePerBullet > 0)
                {
                    m_PlayerHealth -= m_DamagePerBullet;
                    CurrentPlayerHealthManager.value = m_PlayerHealth;
                }
                else
                {
                    Instantiate(Resources.Load<GameObject>("DeathScreenCanvasWon"));

                    object[] eventData = new object[] { true };
                    RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCache };
                    PhotonNetwork.RaiseEvent(2, eventData, options, SendOptions.SendReliable);
                }
            }
        }    
    }
}
