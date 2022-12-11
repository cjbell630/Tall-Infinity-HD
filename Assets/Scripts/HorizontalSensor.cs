using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSensor : MonoBehaviour {
    
    public bool isColliding;
    // Start is called before the first frame update
    void Start() {
        isColliding = false;
    }

    // Update is called once per frame
    void Update() {
    }
    
    void OnTriggerEnter(Collider other) {
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null) {
            isColliding = true;
        }
    }
    void OnTriggerExit(Collider other) {
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null) {
            isColliding = false;
        }
    }
}