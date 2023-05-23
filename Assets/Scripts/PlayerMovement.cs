using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float speed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance= 0.2f;
    
    public VariableJoystick variableJoystick;
    public Animator robotAnimator;
    public Player photonPlayer;
    public GameObject arCameraGameObject;
    
    public LayerMask groundLayer;

    private Rigidbody m_RigidBody;
    private CapsuleCollider m_CapsuleCollider;
    private bool m_Grounded = true;
    private float m_JumpInitHeight;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();

        m_JumpInitHeight = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (transform.position.y > m_JumpInitHeight)
        {
            direction = Vector3.zero;
        }
        if (transform.position.y == m_JumpInitHeight)
        {
            m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }


        if (direction == Vector3.zero)
        {
            robotAnimator.enabled = false;
        }
        else
        {
            robotAnimator.enabled = true;
        }
        // m_RigidBody.isKinematic = true;
        transform.Translate(speed * Time.fixedDeltaTime * direction);
        // m_RigidBody.isKinematic = false;
        // transform.rotation = Quaternion.LookRotation(direction);

        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_CapsuleCollider.bounds.extents.y + groundCheckDistance, groundLayer);

    }
    
    public void Jump()
    {
        if(m_Grounded)
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
            if(hit.transform.CompareTag("Plane"))
            {
                Vector3 direction;

                GameObject bullet = Instantiate(Resources.Load<GameObject>("bullet"));
                // bullet.name = photonPlayer.NickName;
                Rigidbody bulletRigidBody = bullet.GetComponent<Rigidbody>();

                Debug.Log(hit.transform.position);
                Debug.Log(transform.position);

                
                direction = hit.transform.position - transform.position;
                direction.Normalize();
                
                bullet.transform.localPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                Debug.Log(direction);
                bulletRigidBody.AddForce(direction * 300f);
                
                Destroy(bullet, 10);
            }
        }

    }
}
