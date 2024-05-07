using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set the camera position so that it is always above the center of the tilemap and covers all elements of the tilemap
public class CameraPosition : MonoBehaviour
{

// Pointer to the component responsible for creating the tilemap
    public CreateTilemap createTilemap;

// Pointer to the component responsible for managing tilemap elements
    public ManagementTilemap managementTilemap;

// Pointer to the camera
    public Camera cameraView;

// Calculate the correct height of the camera so that it can see the entire tilemap
// size - board size
// returns the height of the camera above the tilemap ensuring view of the entire tilemap
    private float GetH(Vector2 size)
    {
        float res = float.NaN;

        if (cameraView != null)
        {
            float alpha = cameraView.fieldOfView;
            float hx = 2*size.x / (Mathf.Tan(alpha*Mathf.Deg2Rad));
            float hy = 2*size.y / (Mathf.Tan(alpha * Mathf.Deg2Rad));
            res = hx;
            if (hy > hx)
                res = hy;
        }

        return res;
    }

// Setting the camera position so that it is placed centrally over the tilemap and covers it in its entirety
    public void NewPosition()
    {
        if (createTilemap != null && managementTilemap != null)
        {
            Vector2 min = new Vector2(createTilemap.positionTilemap.x, createTilemap.positionTilemap.z);
            Vector2Int size_ = managementTilemap.GetSize();
            Vector2 size = new Vector2(size_.x * createTilemap.tileSize.x, size_.y * createTilemap.tileSize.y);
            Vector2 max = min + size;
            Vector2 mean = (max + min) / 2;
            float h = GetH(size);

            Vector3 pos = transform.position;
            pos.x = mean.x;
            pos.y = createTilemap.positionTilemap.y + h;
            pos.z = mean.y;
            transform.position = pos;

        }
    }

    void Start()
    {
        cameraView = GetComponent<Camera>();
        NewPosition();
    }
}
