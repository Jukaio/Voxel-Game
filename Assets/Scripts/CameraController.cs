using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] RoomGenerator RG;
    [SerializeField] Vector3 offset = new Vector3(0.0f, 10.0f, 0.0f);

    // Piratux start
    public Camera player_camera;
    private Vector2 camera_rot_x = new Vector2(45, 65);
    private Vector2 camera_rot_y = new Vector2(-15, 15);
    public bool change_rotation = true;
    // Piratux end

    private void Awake()
    {   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Piratux start
        Vector2Int grid_cell = RG.world.world_to_grid(transform.position);
        Vector3 vec = RG.world.grid_to_world(grid_cell, true);
        player_camera.transform.position = new Vector3(vec.x, 0, vec.z - RG.world.CellSize / 2) + offset;

        if (change_rotation)
        {
            float rot_x = ((camera_rot_x.y - camera_rot_x.x) / RG.world.CellSize) * (RG.world.CellSize - (transform.position.z - grid_cell.y * RG.world.CellSize)) + camera_rot_x.x;
            float rot_y = ((camera_rot_y.y - camera_rot_y.x) / RG.world.CellSize) * (transform.position.x - grid_cell.x * RG.world.CellSize) + camera_rot_y.x;

            player_camera.transform.localRotation = Quaternion.Euler(rot_x, rot_y, 0);
        }
        // Piratux end
    }
}
