using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    public float angle;
    float tiltAngle = 0;
    public float layer;
    static readonly float DEG_PER_SEC = 70f; // deg per sec
    static readonly float H_UNITS_PER_SEC = Util.ArcLength(Block.RingRad, DEG_PER_SEC);
    static readonly float V_UNITS_PER_SEC = 2 * H_UNITS_PER_SEC;
    static readonly float G_UNITS_PER_SEC = 0.5f * V_UNITS_PER_SEC;
    static readonly float MAX_TILT = 90;

    static readonly float
        TILT_DEG_PER_SEC = MAX_TILT * V_UNITS_PER_SEC / (Block.Height * 4); // takes 2 blocks to tilt 90 deg

    float flipAngPerSec = 0;

    public Sensor<Block> verticalSensor;
    public Sensor<Block> horizontalSensor;
    public GravitySensor gravitySensor;
    PlayerPlane playerPlane;

    Block flippingBlock;

    // Start is called before the first frame update
    void Start() {
        angle = 100;
        layer = 2;
        Block.GoToPosition(transform, angle, layer);
        playerPlane = GetComponentInChildren<PlayerPlane>();
        int angleMod = Util.LogicallyCorrectModulus((int)angle, 20);
        Block.GoToPosition(verticalSensor.transform, angle - angleMod + (angleMod < 10 ? 0 : 20),
            Mathf.FloorToInt(layer - 1));
        Block.GoToPosition(gravitySensor.transform, angle,
            layer - 0.5f);
    }

    // Update is called once per frame
    void Update() {
    }
    
    // TODO make this available for blocks too
    bool IsEvenlyOnLayer() {
        return layer - Mathf.Floor(layer) == 0;
    }

    public void UpdatePosition(float horizontalInput, Controls.ButtonState primaryButton) {
        // TODO make primaryButton ButtonState
        var increaseTilt = false;
        
        /* SET  BLOCKS */
        if (flippingBlock != null && // there is a block
            !flippingBlock.flipping && // it is not flipping
            primaryButton == Controls.ButtonState.Up) { // and the button has just been released
            // set the block
            flippingBlock.Set();
        }
        
        if (!gravitySensor.IsColliding()) {
            // if there is nothing below player
            // TODO player movement should completely overwrite gravity... right?
            // TODO need to be able to be pushed up by block
            Debug.Log("notjing below");
            transform.Translate(new Vector3(0, -G_UNITS_PER_SEC * Time.deltaTime, 0));
        } else if (!IsEvenlyOnLayer()) {
            //TODO causes issues with horizontal collision
            Debug.Log(layer - Mathf.Floor(layer));
            layer = gravitySensor.HighestCollidingObject().transform.position.y + 1;
            transform.position = new Vector3(transform.position.x, layer, transform.position.z);
            // if there is something below the player but the palyer is not evenly on a layer
        }


        /* TODO 
         * flipping not working on negative/positive values of angle
         * base flipping off of player position (player percentage travelled to percentage flipped)
         *  * this is so the camera can track properly and the player angle can be preserved
         * auto move player at normal movement speed (?)
         * 
         */
        if (flippingBlock != null && flippingBlock.IsFlipping()) {
            angle += flipAngPerSec * Time.deltaTime;
            Block.GoToPosition(transform, angle, layer);
        } else if (horizontalInput != 0) {
            var movementDirection = horizontalInput > 0 ? Util.Direction.Right : Util.Direction.Left;

            int angleModUhh = Util.LogicallyCorrectModulus((int)angle, 20); // TODO come on bruh better name pls
            if (primaryButton < Controls.ButtonState.Neutral && IsEvenlyOnLayer() &&
                ((angleModUhh is >= 7 and <= 9 && movementDirection is Util.Direction.Right) ||
                 (angleModUhh is >= 10 and <= 12 && movementDirection is Util.Direction.Left) ||
                 (flippingBlock != null && flippingBlock.flipping) // TODO allows immediate direction change
                )) {
                Debug.Log("Good angle mod: " + angleModUhh);
                TryFlip(movementDirection);
            } else {
                if (horizontalSensor.IsColliding() /*&& Math.Abs(Util.LogicallyCorrectModulus((int)angle, 20)) == 0*/) {
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
        Block.GoToPosition(gravitySensor.transform, angle,
            layer - 0.5f);
        Block.GoToPosition(horizontalSensor.transform,
            angle - angleMod + (angleMod < 10 ? 0 : 20) /*+ (horizontalInput > 0 ? 20 : -20)*/,
            Mathf.Floor(layer));


        Debug.DrawLine(transform.position, verticalSensor.transform.position, Color.red);
        Debug.DrawLine(transform.position, horizontalSensor.transform.position, Color.green);
        Debug.DrawLine(transform.position, gravitySensor.transform.position, Color.blue);
    }

    bool TryFlip(Util.Direction movementDirection) {
        if (!(verticalSensor.IsColliding() && verticalSensor.collidingObjects[0].CanFlip(movementDirection))) return false;

        verticalSensor.collidingObjects[0].Flip(movementDirection);
        flippingBlock = verticalSensor.collidingObjects[0];
        // TODO var angleToTranslate = (flippingBlock.angle - angle);
        var angleToTranslate = flippingBlock.angle - angle;
        var secToFlip = Mathf.Abs(flippingBlock.flipDeg / Block.degPerSec);
        flipAngPerSec = angleToTranslate / secToFlip;
        return true;
    }
}