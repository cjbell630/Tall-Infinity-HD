using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSensor : MonoBehaviour {
    public Block targetedBlock;
    public bool colliding = false;

    // Start is called before the first frame update
    void Start() {
        targetedBlock = null;
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("player sensor ttrigger enter");
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null) {
            Debug.Log("is block");
            if (targetedBlock != null) {
                targetedBlock.SetOutline(false);
            }

            targetedBlock = otherBlock;
            colliding = true;
            targetedBlock.SetOutline(true);
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("ttrigger exit");
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null && targetedBlock == otherBlock) {
            targetedBlock.SetOutline(false);
            targetedBlock = null;
            colliding = false;
        }
    }
}