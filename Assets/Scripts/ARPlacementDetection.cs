using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;
using System.Xml.Serialization;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class ARPlacementDetection : MonoBehaviour //, IPunObservable
{
    private GameObject m_GameManager;

    int debCount = 0;

    PhotonView photonView;

    private bool m_IsOtherPlayerReady = false;
    private bool m_IsCurrentPlayerReady = false;

    private bool m_IsCurrentPlayerReadyPrev = true;

    public static bool s_StartGame = false;

    // private ARPlacementManager m_ARPlacementManager;
    private ARTapToPlaceObject m_ARTapToPlaceObject;
    private ARPlaneManager m_ARPlaneManager;
    // private ARRaycastManager m_ARRaycastManager;
    // Start is called before the first frame update

    void Awake()
    {
        m_GameManager = GameObject.Find("Game Manager");
        m_GameManager.SetActive(false);
        Debug.LogWarning("Game Manager off");
    }

    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        photonView = GetComponent<PhotonView>();
        // m_ARPlacementManager = GetComponent<ARPlacementManager>();
        m_ARTapToPlaceObject = GetComponent<ARTapToPlaceObject>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        // m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        m_IsCurrentPlayerReady = ARTapToPlaceObject.GetIsArenaPlaced();

        if (m_IsCurrentPlayerReady != m_IsCurrentPlayerReadyPrev)
        {
            // Raise a custom event to send the boolean value
            object[] eventData = new object[] { m_IsCurrentPlayerReady };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCache };
            PhotonNetwork.RaiseEvent(1, eventData, options, SendOptions.SendReliable);
            m_IsCurrentPlayerReadyPrev = m_IsCurrentPlayerReady;
        }

        if(m_IsOtherPlayerReady && m_IsCurrentPlayerReady)
        {
            s_StartGame = true;
        }

        if (s_StartGame)
        {
            m_GameManager.SetActive(true);
        }
           

        // if(ARPlacementManager.GetIsArenaPlaced())
        // {
        //     foreach(var plane in m_ARPlaneManager.trackables)
        //     {
        //         plane.gameObject.SetActive(false);
        //     }
        // 
        //     m_ARPlaneManager.enabled = false;
        //     m_ARPlacementManager.enabled = false;
        //     m_ARRaycastManager.enabled = false;
        // }
        // else
        // {
        //     m_ARPlaneManager.enabled = true;
        //     m_ARPlacementManager.enabled = true;
        // }
        if (m_IsCurrentPlayerReady)
        {
            foreach (var plane in m_ARPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            m_ARPlaneManager.enabled = false;
            m_ARTapToPlaceObject.enabled = false;
            // m_ARRaycastManager.enabled = false;
        }
        // else
        // {
        //     m_ARPlaneManager.enabled = true;
        //     m_ARTapToPlaceObject.enabled = true;
        // }
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            object[] eventData = (object[])photonEvent.CustomData;

            // Get the boolean value from the event data
            bool receivedValue = (bool)eventData[0];

            // Update the boolean value if it's different from the current value
            if (receivedValue != m_IsOtherPlayerReady)
            {
                m_IsOtherPlayerReady = receivedValue;
                // Handle the updated boolean value
                Debug.Log("Other Player ready value changed: " + m_IsOtherPlayerReady);
            }
        }
        else if (photonEvent.Code == 2)
        {
            object[] eventData = (object[])photonEvent.CustomData;

            // Get the boolean value from the event data
            bool receivedValue = (bool)eventData[0];

            if (receivedValue)
            {
                Instantiate(Resources.Load<GameObject>("DeathScreenCanvasLost"));
            }
        }
    }
/*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(s_StartGame)
        {
            return;
        }
        
        Debug.LogWarning("In Plane Detection syncronization script");
        Debug.LogWarning("Other Player Ready status " + m_IsOtherPlayerReady);
        Debug.LogWarning("Current Player Ready status " + m_IsCurrentPlayerReady);

        if(stream == null)
        {
            Debug.LogWarning("Stream is Null");
        }

        if (stream.IsWriting)
        {
            if (!(m_IsOtherPlayerReady && m_IsCurrentPlayerReady))
            {
                if(m_IsCurrentPlayerReady != m_IsCurrentPlayerReadyPrev)
                {
                    stream.SendNext(m_IsCurrentPlayerReady);
                    m_IsCurrentPlayerReadyPrev = m_IsCurrentPlayerReady;
                }
                if (m_IsOtherPlayerReady && m_IsCurrentPlayerReady)
                {
                    s_StartGame = true;
                    Debug.LogWarning("StartGame updated");
                }
                return;
            }
            else
                if (!s_StartGame) { s_StartGame = true; Debug.LogWarning("StartGame updated"); }
        }
        else // Read
        {
            if (!(m_IsOtherPlayerReady && m_IsCurrentPlayerReady))
            {
                bool receivedValue = (bool)stream.ReceiveNext();
                if(m_IsOtherPlayerReady != receivedValue)
                {
                    m_IsOtherPlayerReady = receivedValue;
                }

                if (m_IsOtherPlayerReady && m_IsCurrentPlayerReady)
                {
                    s_StartGame = true;
                    Debug.LogWarning("StartGame updated");
                }
                return;
            }
            else
                if (!s_StartGame) { s_StartGame = true; Debug.LogWarning("StartGame updated"); }
        }
    }*/
}
