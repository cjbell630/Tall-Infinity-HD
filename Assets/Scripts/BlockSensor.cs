using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSensor : MonoBehaviour {
    public BlockSensor collidingBlock;
    // TODO public color

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    //TODO copied from PlayerSensor; abstractifuy
    void OnTriggerEnter(Collider other) {
        Debug.Log("ttrigger enter");
        var otherBlock = other.GetComponent<BlockSensor>();
        if (otherBlock != null) {
            if (collidingBlock != null) {
                //collidingBlock.SetOutline(false);
            }

            collidingBlock = otherBlock;
            //targetedBlock.SetOutline(true);
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("ttrigger exit");
        var otherBlock = other.GetComponent<BlockSensor>();
        if (otherBlock != null && collidingBlock == otherBlock) {
            //targetedBlock.SetOutline(false);
            collidingBlock = null;
        }
    }
}