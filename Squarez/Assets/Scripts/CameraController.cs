using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public Transform[] targets;

    public float minSizeY = 10f;
    public float edgeBuffer = 0.75f;
    public float smoothMoveSpeed = 1f;
    public float smoothZoomSpeed = 0.125f;

    void SetCameraPos()
    {
        Vector3 farthestTargetP = new Vector3();
        Vector3 farthestTargetN = new Vector3();

        foreach (Transform target in targets)
        {
            if (farthestTargetP.x < target.position.x)
            {
                farthestTargetP = target.position;
            }
            else if (farthestTargetN.x > target.position.x)
            {
                farthestTargetN = target.position;
            }
        }

        Vector3 middle = new Vector3();

        if (targets.Length > 1) {
            middle = (farthestTargetP + farthestTargetN) * 0.5f;
        }
        else if(targets.Length == 1)
        {
            middle = targets[0].position;
        }
        
        Vector3 smoothedPosition = Vector3.Lerp(camera.transform.position, middle, smoothMoveSpeed * Time.deltaTime);

        camera.transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, camera.transform.position.z);
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

        if (targets.Length > 1)
        {
            float width = Mathf.Abs(farthestTargetPX - farthestTargetNX) * edgeBuffer;
            float height = Mathf.Abs(farthestTargetPY - farthestTargetNY) * edgeBuffer;

            //computing the size
            float camSizeX = Mathf.Max(width, minSizeX);
            float targetSize = Mathf.Max(height, camSizeX * Screen.height / Screen.width, minSizeY);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, smoothZoomSpeed * Time.deltaTime);
        }
        else
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 10f, smoothZoomSpeed * Time.deltaTime); ;
        }
        
    }

    void Update()
    {
        SetCameraPos();
        SetCameraSize();
    }
}
