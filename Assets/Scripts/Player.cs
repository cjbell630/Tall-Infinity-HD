using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    float angle;
    public float layer;
    static readonly float DEG_PER_SEC = 40f; // deg per sec
    static readonly float UNITS_PER_SEC = Util.ArcLength(Block.RingRad, DEG_PER_SEC);

    PlayerSensor playerSensor;
    HorizontalSensor horizontalSensor;

    // Start is called before the first frame update
    void Start() {
        angle = 90;
        layer = 2;
        transform.position = new Vector3(0, layer, Block.RingRad);
        playerSensor = GetComponentInChildren<PlayerSensor>();
        horizontalSensor = GetComponentInChildren<HorizontalSensor>();
        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        Block.GoToPosition(playerSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20),
            Mathf.FloorToInt(layer - 1));
    }

    // Update is called once per frame
    void Update() {
        float verticalChange = 0;
        float layerChange = 0;

        if (!playerSensor.colliding) {
            // if there is nothing below player
            // TODO do this bc player movement should completely overwrite gravity... right?
            verticalChange = -UNITS_PER_SEC * Time.deltaTime;
            layerChange = -UNITS_PER_SEC * Time.deltaTime;
        } else if (layer - Mathf.Floor(layer) > 0) {
            Debug.Log(layer - Mathf.Floor(layer));
            // if there is something below the player but the palyer is not evenly on a layer
            verticalChange = -Mathf.Min(UNITS_PER_SEC * Time.deltaTime, layer-Mathf.Floor(layer));
            layerChange = -Mathf.Min(UNITS_PER_SEC * Time.deltaTime, layer-Mathf.Floor(layer));
        }


        var horizontalInput = Input.GetAxis("Horizontal"); // -1 to 1
        if (horizontalInput != 0) {
            int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
            if (horizontalSensor.isColliding && angleMod == 0) {
                verticalChange = UNITS_PER_SEC * Mathf.Abs(horizontalInput) * Time.deltaTime;
                layerChange = UNITS_PER_SEC * Mathf.Abs(horizontalInput) * Time.deltaTime;
            } else {
                transform.RotateAround(Vector3.zero, Vector3.up, -DEG_PER_SEC * horizontalInput * Time.deltaTime);
                angle += DEG_PER_SEC * horizontalInput * Time.deltaTime;
            }

            UpdateVertical(verticalChange, layerChange);

            // set sensor position to the position of the block with the majority of the player over it
            Block.GoToPosition(playerSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20), layer - 1);
            Block.GoToPosition(horizontalSensor.transform,
                angle - angleMod + (angleMod < 10 ? 0 : 20) + (horizontalInput > 0 ? 20 : -20),
                Mathf.Floor(layer));
        } else {
            UpdateVertical(verticalChange, layerChange);
        }


        Debug.DrawLine(transform.position, playerSensor.transform.position, Color.red);
        Debug.DrawLine(transform.position, horizontalSensor.transform.position, Color.green);
    }

    void UpdateVertical(float verticalChange, float layerChange) {
        if (verticalChange != 0) {
            transform.Translate(new Vector3(0, verticalChange, 0));
        }

        layer += layerChange;
    }
}