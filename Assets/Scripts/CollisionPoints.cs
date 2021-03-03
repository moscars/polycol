using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPoints
{
    Vector3 normal;
    float penetrationDepth;
    bool hasCollision; 
    public CollisionPoints(Vector3 normal, float penetrationDepth, bool hasCollision){
        this.normal = normal;
        this.penetrationDepth = penetrationDepth;
        this.hasCollision = hasCollision;
    }
}
