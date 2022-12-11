using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    // for negative numbers
    public static int LogicallyCorrectModulus(int number, int range) {
        return (range + (number % range)) % range;
    }
    
    public static float ArcLength(float radius, float deg) {
        return radius * deg * Mathf.Deg2Rad;
    }
}