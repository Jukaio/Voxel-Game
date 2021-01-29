using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectData : Copyable<GameObjectData>
{
    public GameObjectData()
    {

    }
    public GameObjectData(Vector2Int index, GameObject prototype)
    {
        this.Index = index;
        this.prototype = prototype;
    }

    public Vector2Int Index { get; private set; }
    private GameObject prototype;
    public GameObject Prototype { get { return prototype; } set { prototype = value; } }

    public GameObjectData create_copy()
    {
        return new GameObjectData(Index, Prototype);
    }
}

public class CopyableGameObject : Copyable<CopyableGameObject>
{
    public GameObject game_object { get; private set; }
    public CopyableGameObject()
    {
        game_object = null;
    }
    public CopyableGameObject(GameObject that)
    {
        game_object = that;
    }

    public CopyableGameObject create_copy()
    {
        GameObject temp = GameObject.Instantiate(game_object);
        return new CopyableGameObject(temp);
    }
}
