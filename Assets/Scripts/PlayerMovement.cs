using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;

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

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (direction == Vector3.zero)
        {
            robotAnimator.enabled = false;
        }
        else
        {
            robotAnimator.enabled = true;
        }
        transform.Translate(direction * speed * Time.fixedDeltaTime);
        // transform.rotation = Quaternion.LookRotation(direction);

        m_Grounded = Physics.Raycast(transform.position, Vector3.down, m_CapsuleCollider.bounds.extents.y + groundCheckDistance, groundLayer);

    }
    
    public void Jump()
    {
        if(m_Grounded)
        {
            m_RigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
