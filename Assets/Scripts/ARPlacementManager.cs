using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementManager : MonoBehaviour
{
    ARRaycastManager m_ARRayCastManager;
    static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit>();
    public Camera arCamera;
    public GameObject battleArenaGameObject;

    private static bool s_ArenaPlaced;

    private void Awake()
    {
        s_ArenaPlaced = false;
        m_ARRayCastManager = GetComponent<ARRaycastManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!s_ArenaPlaced)
        {
            Vector3 centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2);
            Ray ray = arCamera.ScreenPointToRay(centerOfScreen);
            if (m_ARRayCastManager.Raycast(ray, raycast_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Intersection
                Pose hitPose = raycast_Hits[0].pose;
                Vector3 positionToBePlaced = hitPose.position;
                // battleArenaGameObject.transform.position = positionToBePlaced;

                Instantiate(battleArenaGameObject, positionToBePlaced, hitPose.rotation);

                s_ArenaPlaced = true;
            }
        }

    }
    /*
    public static bool GetIsArenaPlaced()
    {
        return s_ArenaPlaced;
    }
    */
}
