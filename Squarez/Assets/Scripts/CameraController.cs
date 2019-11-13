using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public Transform[] targets;

    public float minSizeY = 10f;
    public float edgeBuffer = 0.75f;

    void SetCameraPos()
    {
        Vector3 sumOfTargetsPos = new Vector3();

        foreach (Transform target in targets)
        {
            sumOfTargetsPos = target.position + sumOfTargetsPos;
            Debug.Log(sumOfTargetsPos);
        }

        Vector3 middle = (sumOfTargetsPos) * 0.5f;

        camera.transform.position = new Vector3(
            middle.x,
            middle.y,
            camera.transform.position.z
        );
    }

    void SetCameraSize()
    {
        //horizontal size is based on actual screen ratio
        float minSizeX = minSizeY * Screen.width / Screen.height;

        float farthestTargetPX = 0f;
        float farthestTargetNX = 0f;

        float farthestTargetPY = 0f;
        float farthestTargetNY = 0f;

        foreach (Transform target in targets)
        {
            if(farthestTargetPX < target.position.x)
            {
                farthestTargetPX = target.position.x;
            }
            else if(farthestTargetNX > target.position.x)
            {
                farthestTargetNX = target.position.x;
            }

            if(farthestTargetPY < target.position.y)
            {
                farthestTargetPY = target.position.y;
            }
            else if (farthestTargetNY > target.position.y)
            {
                farthestTargetNY = target.position.y;
            }
        }

        float width = Mathf.Abs(farthestTargetPX - farthestTargetNX) * edgeBuffer;
        float height = Mathf.Abs(farthestTargetPY - farthestTargetNY) * edgeBuffer;

        //computing the size
        float camSizeX = Mathf.Max(width, minSizeX);
        camera.orthographicSize = Mathf.Max(height,
            camSizeX * Screen.height / Screen.width, minSizeY);
    }

    void Update()
    {
        SetCameraPos();
        SetCameraSize();
    }
}
