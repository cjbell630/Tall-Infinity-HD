using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    float angle;
    float layer;
    static readonly float DEG_PER_SEC = 40f; // deg per sec
    static readonly float UNITS_PER_SEC = Util.ArcLength(Block.RingRad, DEG_PER_SEC);

    Block targetedBlock;
    PlayerSensor playerSensor;
    HorizontalSensor horizontalSensor;

    // Start is called before the first frame update
    void Start() {
        angle = 90;
        layer = 1;
        transform.position = new Vector3(0, layer, Block.RingRad);
        targetedBlock = null;
        playerSensor = GetComponentInChildren<PlayerSensor>();
        horizontalSensor = GetComponentInChildren<HorizontalSensor>();
    }

    // Update is called once per frame
    void Update() {
        var horizontalInput = Input.GetAxis("Horizontal"); // -1 to 1
        if (horizontalInput != 0) {
            int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
            if (horizontalSensor.isColliding && angleMod == 0) {
                transform.Translate(new Vector3(0, UNITS_PER_SEC * horizontalInput * Time.deltaTime, 0));
                layer += UNITS_PER_SEC * horizontalInput * Time.deltaTime;
            } else {
                layer = Mathf.Floor(layer); //TODO make sure works when falling
                transform.RotateAround(Vector3.zero, Vector3.up, -DEG_PER_SEC * horizontalInput * Time.deltaTime);
                angle += DEG_PER_SEC * horizontalInput * Time.deltaTime;
            }

            // set sensor position to the position of the block with the majority of the player over it
            Block.GoToPosition(playerSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20),
                Mathf.FloorToInt(layer - 1));
            Block.GoToPosition(horizontalSensor.transform,
                angle - angleMod + (angleMod < 10 ? 0 : 20) + (horizontalInput > 0 ? 20 : -20),
                Mathf.FloorToInt(layer));
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