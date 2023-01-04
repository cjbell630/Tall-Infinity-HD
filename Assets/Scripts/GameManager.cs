using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public Block blockPrefab;
    const int num = 18;
    public Camera mainCamera;
    int numBlocks = 0;

    Block[] bruh;

    public enum GameState {
        Loading,
        Playing
    };

    public GameState gameState = GameState.Loading;

    float loadTime = 1f; //TODO

    // Start is called before the first frame update
    void Start() {
        bruh = MakeRing();
    }

    // Update is called once per frame
    void Update() {
        if (loadTime > 0) {
            loadTime -= Time.deltaTime;
        } else {
            gameState = GameState.Playing;
        }
    }

    Block[] MakeRing() {
        var ring = new Block[num + 1];
        const int change = 360 / num;
        var i = 0;
        Block newPrism;

        for (float angle = 0; angle < 360; angle += change) {
            Debug.Log(angle);
            newPrism = Instantiate(blockPrefab);
            newPrism.angle = angle;
            newPrism.layer = 0;
            newPrism.GoToPosition();
            newPrism.name = "Block " + i;
            newPrism.RandomizeColors();
            newPrism.UpdateShader();
            numBlocks++;
            ring[i] = newPrism;
            i++;
        }

        newPrism = Instantiate(blockPrefab);
        newPrism.angle = 0;
        newPrism.layer = 1;
        newPrism.GoToPosition();
        newPrism.RandomizeColors();
        newPrism.UpdateShader();
        return ring;
    }

}