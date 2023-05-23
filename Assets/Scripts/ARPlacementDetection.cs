using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlacementDetection : MonoBehaviour
{
    // private ARPlacementManager m_ARPlacementManager;
    private ARTapToPlaceObject m_ARTapToPlaceObject;
    private ARPlaneManager m_ARPlaneManager;
    // private ARRaycastManager m_ARRaycastManager;
    // Start is called before the first frame update
    void Start()
    {
        // m_ARPlacementManager = GetComponent<ARPlacementManager>();
        m_ARTapToPlaceObject = GetComponent<ARTapToPlaceObject>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        // m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
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
            foreach(var plane in m_ARPlaneManager.trackables)
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
}
