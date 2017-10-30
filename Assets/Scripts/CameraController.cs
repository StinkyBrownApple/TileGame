using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y/2, 2, 40);
		cam.transform.position = new Vector3(Mathf.Clamp((cam.transform.position.x + Input.GetAxis("Horizontal")), 0 + (cam.orthographicSize * cam.aspect), 200), Mathf.Clamp((cam.transform.position.y + Input.GetAxis("Vertical")), 0 + (cam.orthographicSize), 200), -10);
	}
}
