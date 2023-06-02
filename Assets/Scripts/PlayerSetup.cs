using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        // if this is local player
        if (photonView.IsMine)
        {
            transform.GetComponent<PlayerMovement>().variableJoystick.gameObject.SetActive(true);
            transform.GetComponent<PlayerMovement>().enabled = true;
        }
        // not local player
        else
        {
            transform.GetComponent<PlayerMovement>().variableJoystick.gameObject.SetActive(false);
            transform.GetComponent<PlayerMovement>().enabled = false;
        }
        SetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPlayerName()
    {
        if (playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
        }
    }
}
