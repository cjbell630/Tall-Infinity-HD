using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
    // TODO only these support touch, or only these show touch upoon controller disconnect?
    static readonly RuntimePlatform[] TouchPlatforms = { 
        RuntimePlatform.Android, RuntimePlatform.Switch, RuntimePlatform.IPhonePlayer,
    };

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}