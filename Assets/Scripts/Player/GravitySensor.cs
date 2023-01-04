using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravitySensor : Sensor<Block> {
    public Block HighestCollidingObject() {
        if (collidingObjects.Count == 0) {
            return null;
        }

        var highest = collidingObjects[0];
        foreach (
            var currBlock in collidingObjects.Where(
                currBlock => currBlock.transform.position.y > highest.transform.position.y
            )
        ) {
            highest = currBlock;
        }

        return highest;
    }
}