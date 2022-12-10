using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    Player player;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        transform.position = 2f * player.transform.position;
        transform.LookAt(player.transform);
    }
}