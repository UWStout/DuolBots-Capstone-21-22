using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sound playing");
        AkSoundEngine.PostEvent("sfx_bomber_explode", this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
