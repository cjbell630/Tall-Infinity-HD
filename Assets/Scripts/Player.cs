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
    static readonly float DEG_PER_SEC = 70f; // deg per sec
    static readonly float H_UNITS_PER_SEC = Util.ArcLength(Block.RingRad, DEG_PER_SEC);
    static readonly float V_UNITS_PER_SEC = 2 * H_UNITS_PER_SEC;
    static readonly float G_UNITS_PER_SEC = 0.5f * V_UNITS_PER_SEC;
    static readonly float MAX_TILT = 90;

    static readonly float
        TILT_DEG_PER_SEC = MAX_TILT * V_UNITS_PER_SEC / (Block.Height * 4); // takes 2 blocks to tilt 90 deg

    VerticalSensor verticalSensor;
    HorizontalSensor horizontalSensor;
    PlayerPlane playerPlane;

    Block flippingBlock;

    // Start is called before the first frame update
    void Start() {
        angle = 100;
        layer = 2;
        Block.GoToPosition(transform, angle, layer);
        verticalSensor = GetComponentInChildren<VerticalSensor>();
        horizontalSensor = GetComponentInChildren<HorizontalSensor>();
        playerPlane = GetComponentInChildren<PlayerPlane>();
        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        Block.GoToPosition(verticalSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20),
            Mathf.FloorToInt(layer - 1));
    }

    // Update is called once per frame
    void Update() {
    }

    bool IsEvenlyOnLayer() {
        return layer - Mathf.Floor(layer) == 0;
    }

    public void UpdatePosition(float horizontalInput, bool primaryButton) {
        // TODO make primaryButton ButtonState
        var increaseTilt = false;
        if (!verticalSensor.colliding) {
            // if there is nothing below player
            // TODO player movement should completely overwrite gravity... right?
            Debug.Log("notjing below");
            transform.Translate(new Vector3(0, -G_UNITS_PER_SEC * Time.deltaTime, 0));
        } else if (!IsEvenlyOnLayer()) {
            Debug.Log(layer - Mathf.Floor(layer));
            // if there is something below the player but the palyer is not evenly on a layer
            transform.Translate(new Vector3(0, -Mathf.Min(G_UNITS_PER_SEC * Time.deltaTime, layer - Mathf.Floor(layer)),
                0));
        }
        
        
        /* TODO 
         * flipping not working on negative/positive values of angle
         * base flipping off of player position (player percentage travelled to percentage flipped)
         *  * this is so the camera can track properly and the player angle can be preserved
         * auto move player at normal movement speed (?)
         * 
         */
        if (flippingBlock != null && flippingBlock.IsFlipping()) {
            transform.position = new Vector3(flippingBlock.transform.position.x, transform.position.y,
                flippingBlock.transform.position.z);
        } else if (horizontalInput != 0) {
            var movementDirection = horizontalInput > 0 ? Util.Direction.Right : Util.Direction.Left;

            int angleModUhh = Util.LogicallyCorrectModulus((int)angle, 20); // TODO come on bruh beter name pls
            if (primaryButton && IsEvenlyOnLayer() &&
                ((angleModUhh is 9 && movementDirection is Util.Direction.Right) ||
                 (angleModUhh is 10 && movementDirection is Util.Direction.Left))) {
                if (verticalSensor.colliding) {
                    if (verticalSensor.targetedBlock.CanFlip(movementDirection)) {
                        verticalSensor.targetedBlock.Flip(movementDirection);
                        flippingBlock = verticalSensor.targetedBlock;
                    } else {
                        //TODO inform user they cant flip
                    }
                }
            } else {
                if (horizontalSensor.isColliding /*&& Math.Abs(Util.LogicallyCorrectModulus((int)angle, 20)) == 0*/) {
                    transform.Translate(
                        new Vector3(0, V_UNITS_PER_SEC * Mathf.Abs(horizontalInput) * Time.deltaTime, 0));
                    increaseTilt = true;
                } else {
                    transform.RotateAround(Vector3.zero, Vector3.up, -DEG_PER_SEC * horizontalInput * Time.deltaTime);
                    angle += DEG_PER_SEC * horizontalInput * Time.deltaTime;
                }
            }
        }

        layer = transform.position.y;

        /* TILT */
        if (increaseTilt || tiltAngle != 0) {
            var amountToTilt = increaseTilt
                ? TILT_DEG_PER_SEC * horizontalInput * Time.deltaTime
                : Math.Sign(tiltAngle) * -Mathf.Min(Mathf.Abs(tiltAngle), 2 * TILT_DEG_PER_SEC * Time.deltaTime);
            playerPlane.transform.RotateAround(
                transform.TransformPoint(new Vector3(0, -0.5f, 0)), transform.forward,
                amountToTilt
            );
            tiltAngle += amountToTilt;
        }


        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        // set sensor position to the position of the block with the majority of the player over it
        Block.GoToPosition(verticalSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20), layer - 1);
        Block.GoToPosition(horizontalSensor.transform,
            angle - angleMod + (angleMod < 10 ? 0 : 20) /*+ (horizontalInput > 0 ? 20 : -20)*/,
            Mathf.Floor(layer));


        Debug.DrawLine(transform.position, verticalSensor.transform.position, Color.red);
        Debug.DrawLine(transform.position, horizontalSensor.transform.position, Color.green);
    }
}