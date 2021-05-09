using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
    }
}
