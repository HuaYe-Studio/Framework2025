using LogicLayer;
using RenderLayer;
using UnityEngine;

public class PlayerLogic : LogicActor
{
    public int PlayerID { get;private set; }

    public PlayerLogic(int id, RenderObject renderObject)
    {
        PlayerID = id;
        RenderObject = renderObject;
        ObjectType = LogicObjectType.Player;
    }
}
