using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeData : Copyable<PrototypeData>
{
    public PrototypeData()
    {

    }
    public PrototypeData(Vector2Int index, FlyweightList prototype)
    {
        this.Index = index;
        this.prototypes = prototype;
    }

    public Vector2Int Index { get; private set; }
    private FlyweightList prototypes;
    public FlyweightList Prototype { get { return prototypes; } set { prototypes = value; } }

    public PrototypeData create_copy()
    {
        return new PrototypeData(Index, Prototype);
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
