using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Photon.Pun;

public class ARPlacementDetection : MonoBehaviour, IPunObservable
{
    private GameObject m_GameManager;

    int debCount = 0;

    PhotonView photonView;

    private bool m_IsOtherPlayerReady = false;

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
        photonView = GetComponent<PhotonView>();
        // m_ARPlacementManager = GetComponent<ARPlacementManager>();
        m_ARTapToPlaceObject = GetComponent<ARTapToPlaceObject>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        // m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(s_StartGame)
        {
            m_GameManager.SetActive(true);
        }
        */

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
        if (ARTapToPlaceObject.GetIsArenaPlaced())
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(s_StartGame)
        {
            return;
        }
        
        Debug.LogWarning("In Plane Detection syncronization script");
        Debug.LogWarning("Other Player Ready status " + m_IsOtherPlayerReady);
        Debug.LogWarning("Current Player Ready status " + ARTapToPlaceObject.GetIsArenaPlaced());

        if (stream.IsWriting)
        {
            if (!(m_IsOtherPlayerReady && ARTapToPlaceObject.GetIsArenaPlaced()))
            {
                stream.SendNext(ARTapToPlaceObject.GetIsArenaPlaced());
                if (m_IsOtherPlayerReady && ARTapToPlaceObject.GetIsArenaPlaced())
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
            if (!(m_IsOtherPlayerReady && ARTapToPlaceObject.GetIsArenaPlaced()))
            {
                m_IsOtherPlayerReady = (bool)stream.ReceiveNext();
                if (m_IsOtherPlayerReady && ARTapToPlaceObject.GetIsArenaPlaced())
                {
                    s_StartGame = true;
                    Debug.LogWarning("StartGame updated");
                }
                return;
            }
            else
                if (!s_StartGame) { s_StartGame = true; Debug.LogWarning("StartGame updated"); }
        }
    }
}
