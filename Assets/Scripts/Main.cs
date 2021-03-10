using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;
    List<CollisionObj> colliders;
    public GJK gjkAlgo;
    List<CollisionPoints> collisionPoints;

    long counter = 1;
    bool oneNeedsVel = false;
    int numObjs = 0;

    const float areaExtent = 15f;

    void Start()
    {
        collisionPoints = new List<CollisionPoints>();
        colliders = new List<CollisionObj>();
        gjkAlgo = new GJK();
    }

    void FixedUpdate()
    {
        if(colliders.Count < 90){

            if(counter % 50 == 0){
                CreateNewObj();
                numObjs++;
            }

            if(oneNeedsVel){
                applyFoo();
                oneNeedsVel = false;
            }

            if(counter % 40 == 0){
                createOnWithRandomVel();
                numObjs++;
                Debug.Log(numObjs);
                oneNeedsVel = true;
            }
        }
        
        counter++;
        
        runGJK();
        solveCollisions();
        //solveCollisionPos();
        //solveCollisionsVel(); 
        
        applyGravityToAllColliders();
        updateAccelerationForAllColliders();
        updateVelocityForAllColliders();
        updatePosForAllColliders();

        resetNetForceForAllColliders();
        dontMoveOutsideArea();
        dontFallThroughFloorForAllColliders();
        
        collisionPoints = new List<CollisionPoints>();
    }

    void dontMoveOutsideArea(){
        float colliderWidth = 0.5f;

        foreach(CollisionObj collider in colliders){
            Vector3 pos = collider.getPosition();
            if(pos.x + colliderWidth >= areaExtent || pos.x - colliderWidth <= -areaExtent){
                collider.setVelocity(new Vector3(-collider.velocity.x, collider.velocity.y, collider.velocity.z));
            }
            if(pos.z + colliderWidth >= areaExtent || pos.z - colliderWidth <= -areaExtent){
                collider.setVelocity(new Vector3(collider.velocity.x, collider.velocity.y, -collider.velocity.z));
            }
        }
    }

    public void createOnWithRandomVel(){
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
                CollisionPoints p = gjkAlgo.runGJK(colliders[i], colliders[j]);
                if(p.hasCollision){
                    collisionPoints.Add(p);
                }
            }
        }
    }


    void solveCollisions(){
        List<(Vector3, Vector3, Vector3, bool)> newVals = new List<(Vector3, Vector3, Vector3, bool)>();
        const float bounceFactor = 0.5f;

        foreach (CollisionPoints collisionPoint in collisionPoints){
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
        foreach(CollisionPoints collisionPoint in collisionPoints){
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

    void solveCollisionPos(){
        List<(Vector3, Vector3)> deltas = new List<(Vector3, Vector3)>();

        foreach (CollisionPoints collisionPoint in collisionPoints){
            CollisionObj firstCollider = collisionPoint.getFirstCollider();
            CollisionObj secondCollider = collisionPoint.getSecondCollider();
            
            float firstColMass = firstCollider.getMass();
            float secondColMass = secondCollider.getMass();

            const float percent = 0.4f;
			const float slop = 0.01f;

            Vector3 correction = collisionPoint.normal * percent * Math.Max(collisionPoint.penetrationDepth - slop, 0.0f) / (firstColMass + secondColMass);

            Vector3 deltaA = -firstColMass * correction;
            Vector3 deltaB = secondColMass * correction;

            deltas.Add((deltaA, deltaB));
        }

        int i = 0;
        foreach(CollisionPoints collisionPoint in collisionPoints){
            CollisionObj firstCollider = collisionPoint.getFirstCollider();
            CollisionObj secondCollider = collisionPoint.getSecondCollider();

            (Vector3 firstDelta, Vector3 secondDelta) temp = deltas[i];

            firstCollider.setPosition(firstCollider.getPosition() + temp.firstDelta);
            secondCollider.setPosition(secondCollider.getPosition() + temp.secondDelta);
            i++;
        }
    }

    void solveCollisionsVel(){
        foreach (CollisionPoints collisionPoint in collisionPoints){
            CollisionObj firstCollider = collisionPoint.getFirstCollider();
            CollisionObj secondCollider = collisionPoint.getSecondCollider();
            
            Vector3 firstVel = firstCollider.getVelocity();
            Vector3 secondVel = secondCollider.getVelocity();

            Vector3 diffVel = secondVel - firstVel;

            float nSpd = Vector3.Dot(diffVel, collisionPoint.normal);

            float firstColMass = firstCollider.getMass();
            float secondColMass = secondCollider.getMass();

            if(nSpd >= 0){
                continue;
            }

            float bounceFactor = 0.8f * 0.8f; //Bounce of firstCol * Bouce of secondCol

            float j = -(1.0f + bounceFactor) * nSpd / (firstColMass + secondColMass);

            Vector3 impulse = j * collisionPoint.normal;

            firstVel -= impulse * firstColMass;
            secondVel += impulse * secondColMass;

            firstCollider.setVelocity(firstVel);
            secondCollider.setVelocity(secondVel);
        }
    }

    void CreateNewObj(){
        float extent = 1.8f;
        float randomX = UnityEngine.Random.Range(-extent, extent);
        float randomZ = UnityEngine.Random.Range(-extent, extent);
        float randomY = UnityEngine.Random.Range(15f, 25f);
        Vector3 pos = new Vector3(randomX, randomY, randomZ);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);//Quaternion.Euler(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f)));
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }
}
