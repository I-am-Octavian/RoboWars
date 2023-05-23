using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject battleArena;
    // public GameObject gameObjectToInstantiate;

    // private static GameObject spawnedObject;
    private ARRaycastManager m_ARRaycastManager;
    private static bool s_ArenaPlaced = false;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update
    void Start()
    {
        // spawnedObject = null;
    }

    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!s_ArenaPlaced)
        {
            if (Input.touchCount > 0)
            {
                Vector2 touchPosition = Input.GetTouch(0).position;
                if (m_ARRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    Instantiate(battleArena, hitPose.position, hitPose.rotation);
                    // Vector3 positionToBePlaced = hitPose.position;
                    // battleArena.transform.position = positionToBePlaced;

                    s_ArenaPlaced = true;
                }
            }
            else
            {
                return;
            }
        }
            /*
            else
            {
                // To change the position after placing the object (Not required)
                spawnedObject.transform.position = hitPose.position;
            }*/
    }

    public static bool GetIsArenaPlaced()
    {
        return s_ArenaPlaced;
    }
}
