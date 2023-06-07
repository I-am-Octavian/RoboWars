using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject battleArena;
    public Vector3 instantiationPosition;
    public GameObject robotObject;
     
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(battleArena, instantiationPosition, Quaternion.identity);
        Vector3 robotInstantiationPosition = new Vector3(instantiationPosition.x, instantiationPosition.y , instantiationPosition.z + 0.2f);
        Instantiate(robotObject, robotInstantiationPosition, Quaternion.identity );
        Vector3 robotInstantiationPosition2 = new Vector3(instantiationPosition.x, instantiationPosition.y, instantiationPosition.z - 0.2f);
        Instantiate(robotObject, robotInstantiationPosition2, Quaternion.identity );
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
