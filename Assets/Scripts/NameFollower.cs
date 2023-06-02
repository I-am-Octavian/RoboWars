using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARCore;

public class NameFollower : MonoBehaviour
{
    private Transform m_Camera;
    private Transform m_WorldCanvas;
    private Transform m_Unit;

    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        m_WorldCanvas = GameObject.Find("PlayerNameCanvas").transform;
        m_Camera = GameObject.Find("AR Camera").transform;
        m_Unit = transform.parent;

        transform.SetParent(m_WorldCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation( m_Unit.position + offset, 
            Quaternion.LookRotation(transform.position - m_Camera.transform.position) );
    }
}
