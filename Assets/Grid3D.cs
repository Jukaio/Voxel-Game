using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Copyable<T>
{
    public abstract T create_copy();
}

public class Grid3D<T> where T : Copyable<T>, new()
{
    public float CellSize { get; set; }
    public Vector2 Position { get; set; }
    private Cell<T>[,] data;
    private Vector2Int count;
    public Vector2Int Count 
    { 
        get 
        { 
            return count;
        }
        private set
        {
            count = new Vector2Int(data.GetLength(0), data.GetLength(1));
        }
    }

    private class Cell<Content> where Content : Copyable<Content>, new()
    {
        private Content data;
        
        public Cell()
            : base()
        {
            
        }
        public Cell(Cell<Content> other)
            : base()
        {
            data = other.data.create_copy();
        }

        public Content get()
        {
            return data;
        }
        public void set(Content to)
        {
            data = to;
        }
    }
    public Grid3D()
        : base()
    {

    }
    public Grid3D(Grid3D<T> other)
        : this()
    {
        this.CellSize = other.CellSize;
        this.CellSize = other.CellSize;
        data = new Cell<T>[other.count.x, other.count.y];
        Count = other.Count;
        init_empty_cell();

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                data[x, y] = new Cell<T>(other.data[x, y]);
            }
        }
    }
    public Grid3D(Vector3 position, Vector2Int count, float cell_size)
        : this()
    {
        this.CellSize = cell_size;
        this.Position = new Vector2(position.x, position.z);
        data = new Cell<T>[count.x, count.y];
        Count = count;
        init_empty_cell();
        
    }
    public void init_empty_cell()
    {
        for(int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                data[x, y] = new Cell<T>();
            }
        }
    }
    public void set(int x, int y, T to)
    {
        data[x, y].set(to);
    }
    public T get(int x, int y)
    {
        return data[x, y].get();
    }
    public void draw_rect(Vector3 pos, float size, Color color)
    {
        Vector3 half = (size / 2.0f) * Vector2.one;
        Vector3 top_left = new Vector3(-half.x, 0.0f, half.y) + pos;
        Vector3 top_right = new Vector3(half.x, 0.0f, half.y) + pos;
        Vector3 bottom_left = new Vector3(-half.x, 0.0f, -half.y) + pos;
        Vector3 bottom_right = new Vector3(half.x, 0.0f, -half.y) + pos;

        Debug.DrawLine(top_left, top_right, color);
        Debug.DrawLine(top_right, bottom_right, color);
        Debug.DrawLine(bottom_right, bottom_left, color);
        Debug.DrawLine(bottom_left, top_left, color);
    }
    public Vector2Int world_to_grid(Vector3 at)
    {
        Vector2 index = new Vector2(at.x, at.z);
        index -= Position;
        index /= CellSize;

        return new Vector2Int((int)index.x, (int)index.y);
    }
    public Vector3 grid_to_world(Vector2Int at, bool at_center)
    {
        Vector2 pos = at;
        pos *= CellSize;
        pos += Position;
        if (at_center) pos += (Vector2.one * (CellSize * 0.5f));

        return new Vector3(pos.x, 0.0f, pos.y);
    }
    public void debug_draw()
    {
        for (int x = 0; x < count.x; x++)
        {
            for (int y = 0; y < count.y; y++)
            {
                draw_rect(grid_to_world(new Vector2Int(x, y), true),
                          CellSize,
                          Color.red);
            }
        }

        //draw_rect(origin_position, new Vector2(0.5f, 0.5f), Color.yellow);
        //draw_rect(center, new Vector2(0.5f, 0.5f), Color.green);
    }
}

