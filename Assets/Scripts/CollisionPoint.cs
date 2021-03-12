using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPoint
{
    public Vector3 normal;
    public float penetrationDepth;
    public bool hasCollision; 
    CollisionObj firstCollider;
    CollisionObj secondCollider;

    public CollisionPoint(Vector3 normal, float penetrationDepth, bool hasCollision, CollisionObj firstCollider, CollisionObj secondCollider){
        this.normal = normal;
        this.penetrationDepth = penetrationDepth;
        this.hasCollision = hasCollision;
        this.firstCollider = firstCollider;
        this.secondCollider = secondCollider;
    }

    public CollisionObj getFirstCollider(){
        return firstCollider;
    }

    public CollisionObj getSecondCollider(){
        return secondCollider;
    }
}
