using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    float angle;
    int layer;
    static readonly float SPEED = -40f; // deg per sec

    Block targetedBlock;
    PlayerSensor playerSensor;

    // Start is called before the first frame update
    void Start() {
        angle = 0;
        layer = 1;
        transform.position = new Vector3(0, layer, Block.RingRad);
        targetedBlock = null;
        playerSensor = GetComponentInChildren<PlayerSensor>();
    }

    // Update is called once per frame
    void Update() {
        float horizontalInput = Input.GetAxis("Horizontal"); // -1 to 1
        if (horizontalInput != 0) {
            transform.RotateAround(Vector3.zero, Vector3.up, SPEED * horizontalInput * Time.deltaTime);
            Block.GoToPosition(playerSensor.gameObject, angle - (angle % 20), layer - 1);
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("ttrigger enter");
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null) {
            if (targetedBlock != null) {
                targetedBlock.SetOutline(false);
            }

            targetedBlock = otherBlock;
            targetedBlock.SetOutline(true);
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("ttrigger exit");
        var otherBlock = other.GetComponent<Block>();
        if (otherBlock != null && targetedBlock == otherBlock) {
            targetedBlock.SetOutline(false);
            targetedBlock = null;
        }
    }
}