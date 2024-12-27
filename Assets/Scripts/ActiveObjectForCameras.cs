using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObjectForCameras : MonoBehaviour
{
    public int cameraPosition;

    // Update is called once per frame
    void Update()
    {
        if(cameraPosition != GameManager.Instance.cameraPosition)
        {
            this.gameObject.SetActive(false);
        }
    }
}
