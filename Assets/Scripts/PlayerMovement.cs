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

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float speed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.01f;

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
    private int m_PlayerHealth = 2000;
    private readonly int m_DamagePerBullet = 200;
    private PhotonView m_PhotonView;


    [PunRPC]
    public void Initialize(Player thePlayer)
    {
        photonPlayer = thePlayer;
        id = thePlayer.ActorNumber;
        GameManager.instance.players[id - 1] = this;
    }

    void Start()
    {
        arCameraGameObject = GameObject.FindGameObjectWithTag("Camera");
        Debug.LogWarning("Player Movement Start");
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_JumpInitHeight = transform.position.y;
        m_PhotonView = GetComponent<PhotonView>();
        // playerNickName.text = photonPlayer.NickName;

        Debug.LogWarning("Jump Initial Height" + m_JumpInitHeight);
    }

    // [PunRPC]
    void FixedUpdate()
    {

        Debug.LogWarning("Current player position " + transform.position);

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

            m_Grounded = transform.position.y == m_JumpInitHeight;
            // m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_CapsuleCollider.bounds.extents.y + groundCheckDistance, groundLayer);
            Debug.LogWarning("Grounded status " + m_Grounded);
        }

    }

    public void CallFire()
    {
        if(photonPlayer.IsLocal)
        {
            m_PhotonView.RPC("Fire", RpcTarget.All);
            Debug.LogWarning("Fire Button pressed by " + photonPlayer.NickName);
        }
    }


    [PunRPC]
    public void Jump()
    {
        
        if(photonPlayer.IsLocal && m_Grounded)
        {
            m_RigidBody.constraints = ~RigidbodyConstraints.FreezePositionY;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
            m_RigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }

    [PunRPC]
    public void Fire()
    {

        GameObject crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        Vector3 crosshairPosition = crosshair.transform.position;

        RaycastHit hit;
        if(Physics.Raycast(arCameraGameObject.transform.position, arCameraGameObject.transform.forward, out hit))
        {
            if(hit.transform.CompareTag("Plane") || hit.transform.CompareTag("Player"))
            {
                Vector3 direction;

                GameObject bullet = Instantiate(Resources.Load<GameObject>("bullet"));
                bullet.name = photonPlayer.NickName;
                // bullet.name = photonPlayer.NickName;
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
                }
                else
                {
                    // Resources.Load<GameObject>("DeathScreenCanvasLost");

                }
            }
        }    
    }
}
