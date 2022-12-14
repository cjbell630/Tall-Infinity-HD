using System;
using cakeslice;
using UnityEngine;

public class Block : MonoBehaviour {
    public static readonly float degPerSec = 315;
    public static readonly float SmallEdgeLength = 0.647346038583f; // 1-2tan(10deg)
    public static readonly float RingRad = 2.33564090981f; // (tan(80deg)-1)/2
    public static readonly float Height = 1;

    public float angle;
    public float layer;

    public Outline outline;

    Vector3 frontPoint, backPoint, axis;
    public Renderer blockRenderer;

    public BlockSensor upSensor, leftSensor, downSensor, rightSensor;
    public float flipDeg;
    public bool flipping = false;

    public bool ready = false;
    static readonly int LeftColor = Shader.PropertyToID("_LeftColor");
    static readonly int RightColor = Shader.PropertyToID("_RightColor");
    static readonly int UpColor = Shader.PropertyToID("_UpColor");
    static readonly int DownColor = Shader.PropertyToID("_DownColor");

    bool spawning = false;
    int numToSpawn = 0;
    int targetLayer = -1;

    // Start is called before the first frame update
    void Start() {
        flipDeg = 0;
        outline.enabled = false;
        UpdateShader();
        layer = 0;
    }

    // Update is called once per frame
    void Update() {
        if (flipDeg != 0) {
            Debug.DrawLine(frontPoint, backPoint, Color.red);
            var degToFlip = MathF.Sign(flipDeg) * degPerSec * Time.deltaTime;
            if (Math.Abs(flipDeg) - Math.Abs(degToFlip) < 0) {
                degToFlip = flipDeg;
                flipDeg = 0;
            } else {
                flipDeg -= degToFlip;
            }

            transform.RotateAround(backPoint, axis, degToFlip);
        } else {
            flipping = false; // NOTE this is here bc flipping needs to be set the frame AFTER the flip is done
        }

        if (spawning) {
            if(layer < targetLayer) {
                layer += 0.1f;
                if (layer > targetLayer) {
                    layer = targetLayer;
                    spawning = numToSpawn > 0;
                }
                transform.position = new Vector3(transform.position.x, layer, transform.position.z);
            }
            if (numToSpawn > 0) {
                Block newBlock = Instantiate(this, transform);
                newBlock.RandomizeColors(); // TODO randomize rotation thingy as well
                targetLayer = (int)layer + 1;
                numToSpawn--;
            }
        }

        UpdateShader(); //TODO only do at certain times
    }

    public void GoToPosition() {
        GoToPosition(transform, angle, layer);
    }

    public static void GoToPosition(Transform targetTransform, float angle, float layer) {
        var x = Block.RingRad * Mathf.Cos(angle * Mathf.Deg2Rad);
        var z = Block.RingRad * Mathf.Sin(angle * Mathf.Deg2Rad);
        targetTransform.position = new Vector3(x, layer, z);
        targetTransform.rotation = Quaternion.Euler(0, -angle - 90, 0);
    }

    public static float PositionToAngle(Transform targetTransform) {
        var minX = Mathf.Sign(targetTransform.position.x) *
                   Mathf.Min(Mathf.Abs(targetTransform.position.x), Block.RingRad);
        var minZ = Mathf.Sign(targetTransform.position.z) *
                   Mathf.Min(Mathf.Abs(targetTransform.position.z), Block.RingRad);
        return Mathf.Atan2(minZ, minX) * Mathf.Rad2Deg;
    }

    public void SetOutline(bool on) {
        outline.enabled = on;
        Debug.Log("Outline: " + on);
    }

    public bool CanFlip(Util.Direction direction) {
        // TODO check if directionally blocked
        return direction switch {
            Util.Direction.Right => !rightSensor.IsColliding(),
            Util.Direction.Left => !leftSensor.IsColliding(),
            _ => false
        };
    }

    public void Flip(Util.Direction direction) {
        var flipModifier = (direction == Util.Direction.Right ? 1 : -1);
        frontPoint = transform.TransformPoint(new Vector3(flipModifier * SmallEdgeLength / 2, -0.5f, 0.5f));
        backPoint = transform.TransformPoint(new Vector3(flipModifier * 0.5f, -0.5f, -0.5f));
        axis = frontPoint - backPoint;
        angle += flipModifier * 20;
        GoToPosition();
        CycleColors(direction == Util.Direction.Left);

        flipDeg = flipModifier * -90; //TODO
        transform.RotateAround(backPoint, axis, -flipDeg); //TODO
        flipping = true;
    }

    public bool IsFlipping() {
        return flipDeg != 0;
    }

    public void UpdateShader() {
        blockRenderer.material.SetColor(LeftColor, leftSensor.color);
        blockRenderer.material.SetColor(RightColor, rightSensor.color);
        blockRenderer.material.SetColor(UpColor, upSensor.color);
        blockRenderer.material.SetColor(DownColor, downSensor.color);
    }

    void CycleColors(bool clockwise) {
        //TODO rewrite to make clean
        var upColor = upSensor.color;
        if (clockwise) {
            upSensor.color = leftSensor.color;
            leftSensor.color = downSensor.color;
            downSensor.color = rightSensor.color;
            rightSensor.color = upColor;
        } else {
            upSensor.color = rightSensor.color;
            rightSensor.color = downSensor.color;
            downSensor.color = leftSensor.color;
            leftSensor.color = upColor;
        }

        UpdateShader();
    }

    public void Set() {
        // TODO rest of sensors
        if (downSensor.collidingObjects[0] != null && downSensor.color == downSensor.collidingObjects[0].color) {
            //downSensor.collidingBlock.Set(); TODO
            numToSpawn = 1;
        }
    }


    // TODO make this available for blocks too
    bool IsEvenlyOnLayer() {
        return layer - Mathf.Floor(layer) == 0;
    }


    public void RandomizeColors() {
        this.upSensor.color = Util.RandomColor(new Color[] { });
        this.downSensor.color = Util.RandomColor(new[] { this.upSensor.color });
        this.leftSensor.color = Util.RandomColor(new[] { this.upSensor.color, this.downSensor.color });
        this.rightSensor.color = Util.RandomColor(new[]
            { this.upSensor.color, this.downSensor.color, this.leftSensor.color });
    }
}