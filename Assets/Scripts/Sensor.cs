using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor<T> : MonoBehaviour {
    public List<T> collidingObjects;

    // Start is called before the first frame update
    void Start() {
        collidingObjects = new List<T>();
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter(Collider other) {
        var otherObject = other.GetComponent<T>();
        if (otherObject != null) {
            OnObjectEnter(otherObject);
        }
    }

    void OnTriggerExit(Collider other) {
        var otherObject = other.GetComponent<T>();
        if (otherObject != null) {
            OnObjectExit(otherObject);
        }
    }

    protected virtual void OnObjectEnter(T obj) {
        collidingObjects.Add(obj);
    }

    protected virtual void OnObjectExit(T obj) {
        collidingObjects.Remove(obj); // TODO add error checking
    }

    public bool IsColliding() {
        return collidingObjects.Count > 0;
    }
}