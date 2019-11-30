using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {

    public Transform angle;

    public List<Transform> characters = new List<Transform>();

    Transform p1;
    Transform p2;

    Vector3 v1;


    public float Min = 2;
    public float Max = 6;

    float targetZ;
    public float Min2 = 5;
    public float Max2 = 10;

    Camera cam;

    public CameraType cType;

    public enum CameraType
    {
        ortho,
        persp
    }

	void Start () {
        cam = Camera.main;
        angle = cam.transform.parent;
        cType = (cam.orthographic) ? CameraType.ortho : CameraType.persp;

	}
	

	void FixedUpdate () 
    {
        float distance = Vector3.Distance(characters[0].position, characters[1].position);
        float half = (distance / 2);

        v1 = (characters[1].position - characters[0].position).normalized * half;
        v1 += characters[0].position;

        switch(cType)
        {
            case CameraType.ortho:

                cam.orthographicSize = 2 * (half/2);
                    
                if (cam.orthographicSize > Max)
                {
                    cam.orthographicSize = Max;
                }

                if (cam.orthographicSize < Min)
                {
                    cam.orthographicSize = Min;
                }

                break;
            case CameraType.persp:

                targetZ = -(2 * (half / 2));

                if (Mathf.Abs(targetZ) < Mathf.Abs(Min2))
                    targetZ = Min2;

                if (Mathf.Abs(targetZ) > Mathf.Abs(Max2))
                    targetZ = Max2;

                cam.transform.localPosition = new Vector3(0,
                    0.5f,
                    targetZ);

                break;
        }

        angle.transform.position = Vector3.Lerp(angle.transform.position, v1, Time.deltaTime * 5);
	}

    public static CameraManager instance;
    
    public static CameraManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
}
