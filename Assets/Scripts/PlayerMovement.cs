using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float speed = 2f;
    public VariableJoystick variableJoystick;
    public Rigidbody rigidBody;
    public Animator robotAnimator;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        // rigidBody.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        if(direction == Vector3.zero)
        {
            robotAnimator.enabled = false;
        }
        else
        {
            robotAnimator.enabled = true;
        }
        transform.Translate(direction * speed * Time.fixedDeltaTime);
        // transform.rotation = Quaternion.LookRotation(direction);
    }
}
