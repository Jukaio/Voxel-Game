using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using QuickType;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] voxel_model_prototypes;
    [SerializeField] float cell_size = 1.0f;
    private Pool rooms = new Pool();
    private LDtk content;
    public World world { get; private set; }

    public class Pool
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
    public class RoomData : Grid3D<GameObjectData>, Copyable<RoomData>
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
    public class World : Grid3D<Room>
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
    public class WorldData : Grid3D<RoomData>
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
        rooms = create_room_pool(content, voxel_model_prototypes, cell_size);

        var world_data = create_world_data(rooms, voxel_model_prototypes, cell_size);
        world_data.cut_out_doors(voxel_model_prototypes[0]);
        world = create_world(world_data);
    }

    void LateUpdate()
    {
        world.debug_draw();   
    }

    /*
     
    def carve_passages_from(cx, cy, grid)
  directions = [N, S, E, W].sort_by{rand}

  directions.each do |direction|
    nx, ny = cx + DX[direction], cy + DY[direction]

            if ny.between ? (0, grid.length-1) && nx.between?(0, grid[ny].length-1) && grid[ny][nx] == 0
              grid[cy][cx] |= direction
              grid[ny][nx] |= OPPOSITE[direction]
              carve_passages_from(nx, ny, grid)
            end
          end
        end

     * */


    private static WorldData create_world_data(Pool rooms, GameObject[] prototypes, float cell_size)
    {
        // Initialise the whole world
        WorldData world_data = new WorldData(Vector3.zero, new Vector2Int(7, 7), 16.0f * cell_size);
        /*
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
        */

        Vector2Int current = new Vector2Int(3, 3);
        Vector2Int prev;
        List<Vector2Int> open_direction = new List<Vector2Int>();
        world_data.set(current.x, current.y, rooms.get(0));
        prev = current;

        var layout = RecursiveBacktracking.build_path(new Vector2Int(3, 3), world_data.Count, 3);
        for (int x = 0; x < layout.GetLength(0); x++)
        {
            for (int y = 0; y < layout.GetLength(1); y++)
            {
                if(layout[x,y])
                {
                    world_data.set(x, y, rooms.get(Random.Range(0, rooms.Length)));
                }
            }
        }
        /*
        for (int i = 0; i < 12; i++)
        {
            open_direction.Clear();
            if (current.x > 0)                      open_direction.Add(new Vector2Int(-1,  0));
            if (current.y > 0)                      open_direction.Add(new Vector2Int( 0, -1));
            if (current.x < world_data.Count.x - 1) open_direction.Add(new Vector2Int( 1,  0));
            if (current.y < world_data.Count.y - 1) open_direction.Add(new Vector2Int( 0,  1));

            for (int j = 0; j < open_direction.Count; j++)
            {
                if((current += open_direction[j]) == prev)
                {
                    open_direction.RemoveAt(j);
                    break;
                }
            }

            current += open_direction[Random.Range(0, open_direction.Count)];


            world_data.set(current.x, current.y, rooms.get(0));
            prev = current;
        }
        */
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
        return LDtk.FromJson(json);
    }
    private static string file_to_string(string path)
    {
        StreamReader reader = new StreamReader(path);
        var data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    private static Pool create_room_pool(LDtk content, GameObject[] prototypes, float cell_size)
    {
        Pool rooms = new Pool();
        foreach (var level in content.Levels)
        {
            rooms.add(create_room(level, prototypes, cell_size));
        }
        return rooms;
    }
    private static RoomData create_room(Level level, GameObject[] prototypes, float cell_size)
    {
        var size = level.LayerInstances[0].GridSize;
        var room = new RoomData(Vector3.zero, new Vector2Int((int)size, (int)size), cell_size);
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
