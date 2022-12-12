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
    float tiltAngle = 0;
    public float layer;
    static readonly float DEG_PER_SEC = 40f; // deg per sec
    static readonly float H_UNITS_PER_SEC = Util.ArcLength(Block.RingRad, DEG_PER_SEC);
    static readonly float V_UNITS_PER_SEC = 2 * H_UNITS_PER_SEC;
    static readonly float G_UNITS_PER_SEC = 0.5f * V_UNITS_PER_SEC;
    static readonly float MAX_TILT = 90;

    static readonly float
        TILT_DEG_PER_SEC = MAX_TILT * V_UNITS_PER_SEC / (Block.Height * 2); // takes 2 blocks to tilt 90 deg

    PlayerSensor playerSensor;
    HorizontalSensor horizontalSensor;
    PlayerPlane playerPlane;


    // Start is called before the first frame update
    void Start() {
        angle = 90;
        layer = 2;
        transform.position = new Vector3(0, layer, Block.RingRad);
        playerSensor = GetComponentInChildren<PlayerSensor>();
        horizontalSensor = GetComponentInChildren<HorizontalSensor>();
        playerPlane = GetComponentInChildren<PlayerPlane>();
        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        Block.GoToPosition(playerSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20),
            Mathf.FloorToInt(layer - 1));
    }

    // Update is called once per frame
    void Update() {
    }

    public void UpdatePosition(float horizontalInput) {
        var increaseTilt = false;
        if (!playerSensor.colliding) {
            // if there is nothing below player
            // TODO player movement should completely overwrite gravity... right?
            transform.Translate(new Vector3(0, -G_UNITS_PER_SEC * Time.deltaTime, 0));
        } else if (layer - Mathf.Floor(layer) > 0) {
            Debug.Log(layer - Mathf.Floor(layer));
            // if there is something below the player but the palyer is not evenly on a layer
            transform.Translate(new Vector3(0, -Mathf.Min(G_UNITS_PER_SEC * Time.deltaTime, layer - Mathf.Floor(layer)),
                0));
        }

        if (horizontalInput != 0) {
            if (horizontalSensor.isColliding /*&& Math.Abs(Util.LogicallyCorrectModulus((int)angle, 20)) == 0*/) {
                transform.Translate(new Vector3(0, V_UNITS_PER_SEC * Mathf.Abs(horizontalInput) * Time.deltaTime, 0));
                increaseTilt = true;
            } else {
                transform.RotateAround(Vector3.zero, Vector3.up, -DEG_PER_SEC * horizontalInput * Time.deltaTime);
                angle += DEG_PER_SEC * horizontalInput * Time.deltaTime;
            }
        }

        layer = transform.position.y;

        /* TILT */
        if (increaseTilt) {
            playerPlane.transform.RotateAround(
                transform.TransformPoint(new Vector3(0, -0.5f, 0)), transform.forward,
                -TILT_DEG_PER_SEC * horizontalInput * Time.deltaTime
            );
            tiltAngle -= TILT_DEG_PER_SEC * horizontalInput * Time.deltaTime;
        } else {
            var amountToTilt = (Math.Sign(tiltAngle)) *
                               -Mathf.Min(Mathf.Abs(tiltAngle), 2 * TILT_DEG_PER_SEC * Time.deltaTime);
            playerPlane.transform.RotateAround(
                transform.TransformPoint(new Vector3(0, -0.5f, 0)), transform.forward,
                amountToTilt
            );
            tiltAngle += amountToTilt;
        }
        
        
        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        // set sensor position to the position of the block with the majority of the player over it
        Block.GoToPosition(playerSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20), layer - 1);
        Block.GoToPosition(horizontalSensor.transform,
            angle - angleMod + (angleMod < 10 ? 0 : 20) /*+ (horizontalInput > 0 ? 20 : -20)*/,
            Mathf.Floor(layer));
        

        Debug.DrawLine(transform.position, playerSensor.transform.position, Color.red);
        Debug.DrawLine(transform.position, horizontalSensor.transform.position, Color.green);
    }
}