using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.ARFoundation;

public class MySynchronisationScript : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;
    Vector3 networkPosition;
    Quaternion networkRotation;

    GameObject planeObject;


    public bool synchronisedVelocity = true;
    public bool synchronisedAngularVelocity = true;
    public float distance;
    public float angle;

    public bool isTeleportEnabled = true;
    public float teleportDistanceGreaterThan = 5.0f;
   
    private void Awake()
    {
        Debug.LogWarning("Syncronization Script Awake");
        planeObject = GameObject.FindGameObjectWithTag("Plane");
        Debug.LogWarning("Location of plane object = " + planeObject.transform.position);
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        networkPosition = new Vector3();
        networkRotation = new Quaternion();
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogWarning("Syncronization Script Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!ARPlacementDetection.s_StartGame)
        {
            return;
        }
        if (!photonView.IsMine) // Receive position
        {
            rb.position = Vector3.MoveTowards(rb.position, networkPosition,distance *(1.0f/PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
            Debug.LogWarning("Other player position = " + rb.position);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // if photon view is mine then I'm the one who control this player 
            // I'll send my position, velocity data to other players
            if(!ARPlacementDetection.s_StartGame)
            {
                return;
            }

            stream.SendNext(rb.position - planeObject.transform.position);
            stream.SendNext(rb.rotation);
            if (synchronisedVelocity)
            {
                stream.SendNext(rb.velocity);
            }
            if (synchronisedAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
            Debug.LogWarning("Sending Current Position" + rb.position);
        }
        else
        {
            // this will call on my instance that is there on remote player
            Debug.LogWarning("Sending Current Position");
            if (!ARPlacementDetection.s_StartGame)
            {
                return;
            }
            networkPosition = (Vector3) stream.ReceiveNext() + planeObject.transform.position;
            networkRotation = (Quaternion) stream.ReceiveNext();
            // teleport if the remote player has gone gone to far due to connectivity issues
            if (isTeleportEnabled)
            {
                if (Vector3.Distance(rb.position, networkPosition) > teleportDistanceGreaterThan)
                {
                    rb.position = networkPosition;
                }
            }
            // This checks the time between the server to transfer the data and receiving the data
            if (synchronisedAngularVelocity || synchronisedVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                if (synchronisedVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();
                    networkPosition += rb.velocity * lag;
                    distance = Vector3.Distance(rb.position, networkPosition); 

                }
                if (synchronisedAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkRotation;
                    angle = Quaternion.Angle(rb.rotation, networkRotation);
                }
            }
        }
    }
}
