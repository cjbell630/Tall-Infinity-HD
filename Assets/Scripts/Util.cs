using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Util : MonoBehaviour {
    public enum Direction {
        Up,
        Down,
        Left,
        Right,
        None
    };

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
    
    
    public static Color RandomColor(Color[] exclude) {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };

        if (Random.Range(0, 2) != 1) return new Color(0, 0, 0, 0);
        Color color;
        do {
            color = colors[Random.Range(0, colors.Length)];
        } while (Array.Exists(exclude, color1 => color1 == color));
        
        return color;

    }
}