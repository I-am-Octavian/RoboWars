using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Realtime;


[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float speed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance= 0.2f;
    
    public VariableJoystick variableJoystick;
    public Animator robotAnimator;
    
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

        // if (transform.position.y > m_JumpInitHeight + 0.5)
        // {
        //     direction = Vector3.zero;
        // }
        // if (transform.position.y == m_JumpInitHeight)
        // {
        //     m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // }


        if (direction == Vector3.zero)
        {
            robotAnimator.enabled = false;
        }
        else
        {
            robotAnimator.enabled = true;
        }
        // m_RigidBody.useGravity = false;
        transform.Translate(speed * Time.fixedDeltaTime * direction);
        // m_RigidBody.useGravity = true;
        // transform.rotation = Quaternion.LookRotation(direction);

        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_CapsuleCollider.bounds.extents.y + groundCheckDistance, groundLayer);

    }
    
    public void Jump()
    {
        if(m_Grounded)
        {
            // m_RigidBody.constraints = ~RigidbodyConstraints.FreezePositionY;
            // transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            m_RigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
