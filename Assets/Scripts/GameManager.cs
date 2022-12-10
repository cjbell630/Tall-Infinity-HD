using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject prism;
    const int num = 18;

    const float radius = 2.8f;

    //float oldRadius;
    GameObject[] bruh;

    // Start is called before the first frame update
    void Start() {
        //oldRadius = radius;
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

    GameObject[] MakeRing() {
        var ring = new GameObject[num];
        const int change = 360 / num;
        var i = 0;
        for (float angle = 0; angle < 360; angle += change) {
            Debug.Log(angle);
            var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            var z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            var newPrism = Instantiate(prism, new Vector3(x, 0, z), Quaternion.identity);
            newPrism.transform.Rotate(0, 360 - angle, 0);
            ring[i] = newPrism;
            i++;
        }

        return ring;
    }
}