using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using QuickType;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] static_environment;
    [SerializeField] private GameObject[] voxel_model_prototypes;

    private Pool rooms = new Pool();
    private LDtk content;
    private World world;

    private class RoomData : Grid3D<GameObjectData>, Copyable<RoomData>
    {
        public RoomData()
            : base()
        {

        }
        private RoomData(RoomData other)
            : base(other)
        {

        }
        public RoomData(Vector3 position, Vector2Int count, float cell_size)
            : base(position, count, cell_size)
        {
        }
        public new void set(int x, int y, GameObjectData that)
        {
            base.set(x, y, that);
        }
        public new GameObjectData get(int x, int y)
        {
            return base.get(x, y);
        }

        public RoomData create_copy()
        {
            return new RoomData(this);
        }
    }
    private class Pool
    {
        private List<RoomData> data = new List<RoomData>();
        public int Length { get { return data.Count; } }
        public void add(RoomData that)
        {
            data.Add(that);
        }
        public RoomData get(int index)
        {
            return data[index];
        }
    }
    public class Room : Grid3D<CopyableGameObject>, Copyable<Room>
    {
        public GameObject parent = null;

        public Room()
        {
        }
        public Room(Vector3 position, Vector2Int count, float cell_size)
            : base(position, count, cell_size)
        {
            parent = new GameObject("Room");
            parent.transform.position = position;
        }
        public void set(int x, int y, GameObject that)
        {
            that.transform.parent = parent.transform;
            that.transform.position = grid_to_world(new Vector2Int(x, y), false);
            base.set(x, y, new CopyableGameObject(that));
        }

        public new GameObject get(int x, int y)
        {
            return base.get(x, y).game_object;
        }

        private Room(Room other)
            : base(other)
        {
            parent = GameObject.Instantiate(other.parent);
        }
        public Room create_copy()
        {
            return new Room(this);
        }
    }

    private class World : Grid3D<Room>
    {
        GameObject parent = new GameObject("World");

        public World()
            : base()
        {

        }
        public World(Vector2 position, Vector2Int count, float cell_size)
            : base(position, count, cell_size)
        {
            parent.transform.position = position;
        }

        public void set(int x, int y, RoomData that)
        {
            if (that == null)
                return;
            //that.parent.transform.parent = parent.transform;
            Room room = new Room(grid_to_world(new Vector2Int(x, y), false), that.Count, that.CellSize);
            room.parent.transform.parent = parent.transform;

            for (int ix = 0; ix < that.Count.x; ix++)
            {
                for (int iy = 0; iy < that.Count.y; iy++)
                {
                    var data = that.get(ix, iy);
                    var temp = GameObject.Instantiate(data.Prototype);
                    room.set(data.Index.x, data.Index.y, temp);
                }
            }
            base.set(x, y, room);
        }
    }
    private class WorldData : Grid3D<RoomData>
    {
        public WorldData()
            : base()
        {

        }
        public WorldData(Vector2 position, Vector2Int count, float cell_size)
            : base(position, count, cell_size)
        {
        }

        public void set(int x, int y, RoomData that)
        {
            var copy = that != null ? that.create_copy() : null;
            base.set(x, y, copy);
        }

        private void cut_door_at(int x, int y, Vector2Int direction, GameObject door)
        {
            var data = get(x, y);

            // data.get(data.Count.x / 2, data.Count.y - 1).Prototype = door_prototype

            int tx = direction.x > 0 ? data.Count.x - 1 : direction.x < 0 ? 0 : data.Count.x / 2;
            int ty = direction.y > 0 ? data.Count.y - 1 : direction.y < 0 ? 0 : data.Count.y / 2;
            data.get(tx, ty).Prototype = door;

            tx = tx > 0 && tx < data.Count.x - 1 ? tx - 1 : tx;
            ty = ty > 0 && ty < data.Count.y - 1 ? ty - 1 : ty;
            data.get(tx, ty).Prototype = door;
        }

        public void cut_out_doors(GameObject door_prototype)
        {
            for (int x = 0; x < Count.x; x++)
            {
                for (int y = 0; y < Count.y; y++)
                {
                    if (get(x, y) == null)
                        continue;

                    var data = get(x, y);
                    if (x > 0)              if(get(x - 1, y) != null) cut_door_at(x, y, new Vector2Int(-1, 0), door_prototype);
                    if (y > 0)              if(get(x, y - 1) != null) cut_door_at(x, y, new Vector2Int(0, -1), door_prototype);
                    if (x < Count.x - 1)    if(get(x + 1, y) != null) cut_door_at(x, y, new Vector2Int(1, 0), door_prototype);
                    if (y < Count.y - 1)    if(get(x, y + 1) != null) cut_door_at(x, y, new Vector2Int(0, 1), door_prototype);

                }
            }
        }
    }

    void Start()
    {
        content = json_to_LDtk("Assets/Resources/Voxel.ldtk");
        rooms = create_room_pool(content, voxel_model_prototypes);

        var world_data = create_world_data(rooms, voxel_model_prototypes);
        world_data.cut_out_doors(voxel_model_prototypes[0]);
        world = create_world(world_data);
    }

    void Update()
    {

    }

    private static WorldData create_world_data(Pool rooms, GameObject[] prototypes)
    {
        // Initialise the whole world
        WorldData world_data = new WorldData(Vector3.zero, new Vector2Int(5, 5), 16.0f);
        int index = 0;
        for (int x = 0; x < world_data.Count.x; x++)
        {
            for (int y = 0; y < world_data.Count.y; y++)
            {
                world_data.set(x, y, rooms.get(index));
                index++;
                if (index >= rooms.Length)
                    index = 0;
            }
        }

        // change level stuff stuff
        world_data.set(1, 1, null);
        world_data.set(3, 3, null);
        world_data.set(1, 3, null);
        world_data.set(3, 1, null);
        return world_data;
    }
    private static World create_world(WorldData data)
    {
        // For now a fixed size of 2x2
        // Add procedural room generation here
        /* One bool matrix, deciding how to layout the room
         * Randomise "True" fields in the bool matrix
        */
        var world = new World(data.Position, data.Count, data.CellSize);
        for (int x = 0; x < world.Count.x; x++)
        {
            for (int y = 0; y < world.Count.y; y++)
            {
                world.set(x, y, data.get(x, y));
            }
        }
        return world;
    }
    private static LDtk json_to_LDtk(string path)
    {
        var json = file_to_string(path);
        return QuickType.LDtk.FromJson(json);
    }
    private static string file_to_string(string path)
    {
        StreamReader reader = new StreamReader(path);
        var data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    private static Pool create_room_pool(LDtk content, GameObject[] prototypes)
    {
        Pool rooms = new Pool();
        foreach (var level in content.Levels)
        {
            rooms.add(create_room(level, prototypes));
        }
        return rooms;
    }
    private static RoomData create_room(Level level, GameObject[] prototypes)
    {
        var size = level.LayerInstances[0].GridSize;
        var room = new RoomData(Vector3.zero, new Vector2Int((int)size, (int)size), 1.0f);
        for (int x = 0; x < room.Count.x; x++)
        {
            for (int y = 0; y < room.Count.y; y++)
            {
                var proto_index = level.LayerInstances[0].IntGrid[y * size + x].V;
                var prototype = prototypes[proto_index];
                room.set(x, y, new GameObjectData(new Vector2Int(x, y), prototype));
            }
        }
        return room;
    }
}
