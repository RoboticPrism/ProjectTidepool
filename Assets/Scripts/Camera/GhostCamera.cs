using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCamera : MonoBehaviour {

    float speed = 1f;

    float zoomLevel = 20;
    public float zoomMin = 10;
    public float zoomMax = 30;
    public float zoomSpeed = 10f;
    Camera thisCamera;
	// Use this for initialization
	void Start () {
        thisCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(
            transform.position.x + Input.GetAxis("Horizontal") * speed,
            transform.position.y + Input.GetAxis("Vertical") * speed,
            transform.position.z
        );
        zoomLevel += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (zoomLevel > zoomMax)
        {
            zoomLevel = zoomMax;
        }
        else if (zoomLevel < zoomMin)
        {
            zoomLevel = zoomMin;
        }
        thisCamera.orthographicSize = zoomLevel;
	}
}
