using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
    // TODO only these support touch, or only these show touch upoon controller disconnect?
    static readonly RuntimePlatform[] TouchPlatforms = {
        RuntimePlatform.Android, RuntimePlatform.Switch, RuntimePlatform.IPhonePlayer,
    };

    public enum ButtonState {
        Pressed,
        Down, // ^
        Neutral, // no input, no change
        Up, // v
        Released // stayed the same since last frame
    }

    public static float HorizontalInput;
    public static ButtonState PrimaryButton = ButtonState.Neutral;
    public Player player;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        HorizontalInput = 0;
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.gameState == GameManager.GameState.Playing) {
            GetInput();
            player.UpdatePosition(HorizontalInput, PrimaryButton);
        }
    }

    void GetInput() {
        var hasController = Input.GetJoystickNames().Length > 0;
        // TODO keyboard
        if (hasController) {
            // controller
            HorizontalInput = Input.GetAxis("Horizontal");
            PrimaryButton = GetNewState(PrimaryButton, Input.GetButton("Submit"));
        } else if (Array.Exists(TouchPlatforms, p => p == Application.platform)) {
            // touch
        } else {
            // keyboard
            HorizontalInput = Input.GetAxis("Horizontal");
            PrimaryButton = GetNewState(PrimaryButton, Input.GetButton("Submit"));
        }
    }

    static ButtonState GetNewState(ButtonState previous, bool currentFrame) {
        switch (previous) {
            case ButtonState.Down:
            case ButtonState.Pressed:
                return currentFrame ? ButtonState.Pressed : ButtonState.Up;
            case ButtonState.Up:
            case ButtonState.Released:
                return currentFrame ? ButtonState.Down : ButtonState.Released;
            case ButtonState.Neutral:
                return currentFrame ? ButtonState.Down : ButtonState.Up;
            default:
                throw new ArgumentOutOfRangeException(nameof(previous), previous, null);
        }
    }
}