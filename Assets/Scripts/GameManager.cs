using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Block block;
    const int num = 18;
    public Camera mainCamera;

    Block[] bruh;

    // Start is called before the first frame update
    void Start() {
        bruh = MakeRing();
    }

    // Update is called once per frame
    void Update() {
        /*
        if (Math.Abs(radius - oldRadius) < 0.001) return;
        oldRadius = radius;
        foreach (var t in bruh) {
            Destroy(t);
        }

        bruh = MakeRing();
        */
    }

    Block[] MakeRing() {
        var ring = new Block[num + 1];
        const int change = 360 / num;
        var i = 0;
        float x, z;
        Block newPrism;
        for (float angle = 0; angle < 360; angle += change) {
            Debug.Log(angle);
            newPrism = Instantiate(block);
            newPrism.angle = angle;
            newPrism.layer = 0;
            newPrism.GoToPosition();
            ring[i] = newPrism;
            i++;
        }


        newPrism = Instantiate(block);
        newPrism.angle = 0;
        newPrism.layer = 1;
        newPrism.GoToPosition();
        return ring;
    }
}