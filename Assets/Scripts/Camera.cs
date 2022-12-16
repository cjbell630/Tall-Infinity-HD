using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    public Player player;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.Scale(new Vector3(4, 1, 4), player.transform.position);
        transform.Translate(new Vector3(0, 2, 0));
        transform.LookAt(player.transform);
    }
}