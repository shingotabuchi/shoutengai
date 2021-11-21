using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMovement : MonoBehaviour {

    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;
    public float keyMultiplier;
    public float zoomKeyMultiplier;

    public float xpluslim,xminuslim,ypluslim,yminuslim;
    void Start()
    {
        xpluslim = PersonSpawner.spawnRange + 20;
        xminuslim = -xpluslim;
    }
    void Update ()
    {
        Vector3 newposition;
        float xTrans = 0;
        //drag camera around with Middle Mouse
        if (Input.GetMouseButton(2))
        {
            xTrans = -Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed;
        }
        if(Input.GetKey(KeyCode.A)){
            xTrans = -keyMultiplier * Time.deltaTime * dragSpeed;
        }
        if(Input.GetKey(KeyCode.D)){
            xTrans = keyMultiplier * Time.deltaTime * dragSpeed;
        }

        transform.Translate(xTrans, 0, 0);

        //Zoom in and out with Mouse Wheel
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if(Input.GetKey(KeyCode.W)){
            zoom = zoomKeyMultiplier * zoomSpeed;
        }
        if(Input.GetKey(KeyCode.S)){
            zoom = -zoomKeyMultiplier * zoomSpeed;
        }

        GetComponent<Camera>().orthographicSize += -zoom;
        if(GetComponent<Camera>().orthographicSize<0) GetComponent<Camera>().orthographicSize = 0;
        if(!CheckPosition(transform.position)){
            transform.Translate(-xTrans, 0, 0);
        }
    }

    bool CheckPosition(Vector3 newposition){
        if(newposition.x>xpluslim) return false;
        if(newposition.x<xminuslim) return false;
        return true;
    }

}