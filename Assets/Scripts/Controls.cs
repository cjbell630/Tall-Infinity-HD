using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
    // TODO only these support touch, or only these show touch upoon controller disconnect?
    static readonly RuntimePlatform[] TouchPlatforms = {
        RuntimePlatform.Android, RuntimePlatform.Switch, RuntimePlatform.IPhonePlayer,
    };

    public static float HorizontalInput;
    public static bool PrimaryButton; //TODO make ButtonState
    public Player player;

    // Start is called before the first frame update
    void Start() {
        HorizontalInput = 0;
    }

    // Update is called once per frame
    void Update() {
        // TODO if game being played
        GetInput();
        player.UpdatePosition(HorizontalInput, PrimaryButton);
    }

    void GetInput() {
        // TODO keyboard
        if (
            Array.Exists(TouchPlatforms, p => p == Application.platform) &&
            false
        ) {
            // TODO if touch platform and no controller connected
            // TODO touch controls
        } else {
            HorizontalInput = Input.GetAxis("Horizontal");
            PrimaryButton = Input.GetButton("Submit");
        }
    }
}