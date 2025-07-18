using System.Collections;
using System.Collections.Generic;
using FixIntPhysics;
using FixMath;
using RenderLayer;
using UnityEngine;


/// <summary>
/// 代表玩家和敌人同时具有的基础属性
/// </summary>
public abstract class LogicObject
{
   private FixIntVector3 _logicPos;
   private FixIntVector3 _logicDir;
   private FixIntVector3 _logicAngle;
   private FixInt _logicMoveSpeed = 3;
   private FixInt _logicAxis = 1;
   private bool _isActive;

   public FixIntVector3 LogicPos { get {return _logicPos ;} set { _logicPos = value; } }
   public FixIntVector3 LogicDir { get {return _logicDir ;} set { _logicDir = value; } }
   public FixIntVector3 LogicAngle { get {return _logicAngle ;} set { _logicAngle = value; } }
   public FixInt LogicMoveSpeed { get {return _logicMoveSpeed ;} set { _logicMoveSpeed = value; } }
   public FixInt LogicAxis { get {return _logicAxis ;} set { _logicAxis = value; } }
   public bool IsActive { get {return _isActive ;} set { _isActive = value; } }
   
   public RenderObject RenderObject { get; protected set; }
   
   public FixIntBoxCollider Collider { get; protected set; }
   
   public LogicObjectState State { get; set; }
   
   public LogicObjectType ObjectType { get; set; }
   
   public LogicObjectActionState ActionState { get; set; }

   public virtual void OnCreate()
   {
       
       
   }

   public virtual void OnLogicFrameUpdate()
   {
       
   }

   public virtual void OnDestroy()
   {
       
   }
   
}

public enum LogicObjectState
{
   Survival,
   Death,
}

public enum LogicObjectType
{
    Player,
    Enemy,
    Effect,
}

public enum LogicObjectActionState
{
    Idle,
    Move,
    SkillReleasing,
    Floating,
    Hitting,
    StockPiling
    
    
}


