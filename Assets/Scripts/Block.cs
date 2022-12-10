using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    const float degPerSec = 240;
    public bool flipping;

    float flipDeg = 0;

    // Start is called before the first frame update
    void Start() {
        flipping = false;
        
    }

    // Update is called once per frame
    void Update() {
        if (flipping) {
            var frontPoint = transform.TransformPoint(new Vector3(-0.6f, 0f, -0.5f));
            var backPoint = transform.TransformPoint(new Vector3(0.6f, 0f, -0.6f));
            var axis = frontPoint - backPoint;
            transform.RotateAround(backPoint, axis, degPerSec * Time.deltaTime);
            //transform.Rotate(degPerSec * Time.deltaTime, 0, 0);
            //flipDeg += degPerSec * Time.deltaTime;
            if(flipDeg >= 90) {
                //flipping = false;
                flipDeg = 0;
            }
        }
    }
}