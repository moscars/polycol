using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;
    List<CollisionObj> colliders;
    public GJK gjkRunner;
    List<CollisionPoint> collisionPoints;

    long counter = 1;
    bool oneNeedsVel = false;

    const float areaExtent = 15f;

    void Start()
    {
        collisionPoints = new List<CollisionPoint>();
        colliders = new List<CollisionObj>();
        gjkRunner = new GJK();
    }

    void FixedUpdate()
    {
        if(colliders.Count < 90){

            if(counter % 50 == 0){
                CreateNewObj();
            }

            if(oneNeedsVel){
                applyFoo();
                oneNeedsVel = false;
            }

            if(counter % 40 == 0){
                createOnInRandomPos();
                oneNeedsVel = true;
            }
        }
        
        counter++;
        
        runGJK();
        solveCollisions();

        applyGravityToAllColliders();
        updateAccelerationForAllColliders();
        updateVelocityForAllColliders();
        updatePosForAllColliders();

        resetNetForceForAllColliders();
        dontMoveOutsideArea();
        dontFallThroughFloorForAllColliders();
        
        collisionPoints = new List<CollisionPoint>();
    }

    void dontMoveOutsideArea(){
        foreach(CollisionObj collider in colliders){
            collider.dontMoveOutsideArea(areaExtent);
        }
    }

    public void createOnInRandomPos(){
        float extent = 7f;
        float randomX = UnityEngine.Random.Range(-extent, extent);
        float randomZ = UnityEngine.Random.Range(-extent, extent);
        float randomY = UnityEngine.Random.Range(3f, 6f);
        Vector3 pos = new Vector3(randomX, randomY, randomZ);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }


    void applyFoo(){
        float extent = 14f;
        float randomX = UnityEngine.Random.Range(-extent, extent);
        float randomZ = UnityEngine.Random.Range(-extent, extent);
        colliders[colliders.Count - 1].addForce(new Vector3(randomX, 0, randomZ) * 100);
    }

    void resetNetForceForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.resetNetForce();
        }
    }

    void applyGravityToAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.applyGravity();
        }
    }

    void updateAccelerationForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.updateAcceleration();
        }
    }

    void updateVelocityForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.updateVelocity();
        }
    }

    void updatePosForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.updatePos();
        }
    }

    void dontFallThroughFloorForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.dontFallThroughFloor();
        }
    }

    void zeroAllMovementForAllColliders(){
        foreach(CollisionObj collider in colliders){
            collider.zeroAllMovement();
        }
    }

    void runGJK(){
        for(int i = 0; i < colliders.Count; i++){
            for(int j = i + 1; j < colliders.Count; j++){
                CollisionPoint p = gjkRunner.GJKReturn(colliders[i], colliders[j]);
                if(p.hasCollision){
                    collisionPoints.Add(p);
                }
            }
        }
    }


    void solveCollisions(){
        List<(Vector3, Vector3, Vector3, bool)> newVals = new List<(Vector3, Vector3, Vector3, bool)>();
        const float bounceFactor = 0.5f;

        foreach (CollisionPoint collisionPoint in collisionPoints){
            bool secondToMove = true;
            CollisionObj firstCollider = collisionPoint.getFirstCollider();
            CollisionObj secondCollider = collisionPoint.getSecondCollider();

            Vector3 newSecondPos = secondCollider.getPosition() + collisionPoint.normal * collisionPoint.penetrationDepth;

            if(newSecondPos.y <= 0.5f){
                newSecondPos = firstCollider.getPosition() - collisionPoint.normal * collisionPoint.penetrationDepth;
                secondToMove = false;
            }

            Vector3 newFirstVel = secondCollider.getVelocity() * bounceFactor;
            Vector3 newSecondVel = firstCollider.getVelocity() * bounceFactor;

            newVals.Add((newSecondPos, newFirstVel, newSecondVel, secondToMove));
        }

        int i = 0;
        foreach(CollisionPoint collisionPoint in collisionPoints){
            CollisionObj firstCollider = collisionPoint.getFirstCollider();
            CollisionObj secondCollider = collisionPoint.getSecondCollider();

            (Vector3 newSecondPos, Vector3 newFirstVel, Vector3 newSecondVel, bool secondToMove) temp = newVals[i];
            Vector3 newFirstVel = temp.newFirstVel;
            Vector3 newSecondVel = temp.newSecondVel;
            
            if(temp.secondToMove){
                secondCollider.setPosition(temp.newSecondPos);
            } else{
                firstCollider.setPosition(temp.newSecondPos);
            }
            
            if(firstCollider.getPosition().y <= 0.5f && Vector3.Dot(newFirstVel, new Vector3(0, -1, 0)) > 0){
                newFirstVel = new Vector3(newFirstVel.x, 0, newFirstVel.z);
            } 
            if(secondCollider.getPosition().y <= 0.5f && Vector3.Dot(newSecondVel, new Vector3(0, -1, 0)) > 0){
                newSecondVel = new Vector3(newSecondVel.x, 0, newSecondVel.z);
            }

            firstCollider.setVelocity(newFirstVel);
            secondCollider.setVelocity(newSecondVel);
            firstCollider.setAcceleration(Vector3.zero);
            secondCollider.setAcceleration(Vector3.zero);
            i++;
        }
    }

    void CreateNewObj(){
        float extent = 1.8f;
        float randomX = UnityEngine.Random.Range(-extent, extent);
        float randomZ = UnityEngine.Random.Range(-extent, extent);
        float randomY = UnityEngine.Random.Range(15f, 25f);
        Vector3 pos = new Vector3(randomX, randomY, randomZ);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }
}
