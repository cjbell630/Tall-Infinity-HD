using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VerticalSensor : Sensor<Block> {
    protected override void OnObjectEnter(Block block) {
        base.OnObjectEnter(block);
        block.outline.enabled = true;
    }

    protected override void OnObjectExit(Block block) {
        base.OnObjectEnter(block);
        block.outline.enabled = false;
    }
}