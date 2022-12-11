using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class Block : MonoBehaviour {
    const float degPerSec = 240;
    public static readonly float SmallEdgeLength = 0.647346038583f; // 1-2tan(10deg)
    public static readonly float RingRad = 2.33564090981f; // (tan(80deg)-1)/2
    public bool flipping;

    public float angle;
    public int layer;

    public Outline outline;

    Vector3 frontPoint, backPoint, axis;
    float flipDeg = 0;

    // Start is called before the first frame update
    void Start() {
        flipping = false;
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (flipping) {
            if (flipDeg == 0) {
                frontPoint = transform.TransformPoint(new Vector3(SmallEdgeLength / 2, -0.5f, 0.5f));
                backPoint = transform.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f));
                axis = frontPoint - backPoint;
                angle += 20;
                GoToPosition();
                transform.RotateAround(backPoint, axis, 90);
            }

            Debug.DrawLine(frontPoint, backPoint, Color.red);
            var degToFlip = degPerSec * Time.deltaTime;
            if (flipDeg + degToFlip > 90) {
                degToFlip = 90 - flipDeg;
                flipping = false;
                flipDeg = 0;
            } else {
                flipDeg += degToFlip;
            }

            transform.RotateAround(backPoint, axis, -degToFlip);
        }
    }

    public void GoToPosition() {
        GoToPosition(transform, angle, layer);
    }

    public static void GoToPosition(Transform targetTransform, float angle, int layer) {
        var x = Block.RingRad * Mathf.Cos(angle * Mathf.Deg2Rad);
        var z = Block.RingRad * Mathf.Sin(angle * Mathf.Deg2Rad);
        targetTransform.position = new Vector3(x, layer, z);
        targetTransform.rotation = Quaternion.Euler(0, -angle - 90, 0);
    }

    public void SetOutline(bool on) {
        outline.enabled = on;
        Debug.Log("Outline: " + on);
    }
}