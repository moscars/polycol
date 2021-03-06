using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;
    public GameObject vertexPrefab;
    List<CollisionObj> colliders;
    List<GameObject> vertexPrefabList;
    public GJK gjkAlgo;
    List<Vector3> toDraw;
    float x;
    List<CollisionPoints> collisionPoints;

    int counter = 1;

    private int screenshotCount = 0;

    void Start()
    {
        collisionPoints = new List<CollisionPoints>();
        colliders = new List<CollisionObj>();
        vertexPrefabList = new List<GameObject>();
        gjkAlgo = new GJK();
        x = 0;
        //throwTwo(); 
        //Application.targetFrameRate = 5;
    }
    /*
    void Update(){
        screenshotCount++;
        String screenshotFilename = "screenshots/screenshot" + screenshotCount + ".png";
        ScreenCapture.CaptureScreenshot(screenshotFilename);
    }*/

    void FixedUpdate()
    {
        toDraw = new List<Vector3>();
        destoryAllOldVertexPrefabs();

        
        if(counter % 40 == 0){
            //CreateObjCloser(x);
            x = x;
            //counter = 1;
            //CreateNewObj();
            //throwTwo();
        }

        if(counter == 40){
            makeWall();
            throwTwo();
        }
        
        if(counter == 60){
            applyForcess();
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
        dontFallThroughFloorForAllColliders();
        resetNetForceForAllColliders();
        collisionPoints = new List<CollisionPoints>();
        //zeroAllMovementForAllColliders();
    }

    void applyForcess(){
        CollisionObj obj1 = colliders[colliders.Count - 1];
        //CollisionObj obj2 = colliders[1];

        obj1.addForce(new Vector3(-30, 2, 0) * 100);
        //obj2.addForce(new Vector3(-5, 3, 0) * 100);
    }

    void throwTwo(){
        //Vector3 pos1 = new Vector3(-7, 2, 0);
        //GameObject obj1 = Instantiate(colObject, pos1, Quaternion.identity);
        //CollisionObj colObj1 = obj1.GetComponent<CollisionObj>();
        //colliders.Add(colObj1);

        Vector3 pos2 = new Vector3(7, 2, 0);
        GameObject obj2 = Instantiate(colObject, pos2, Quaternion.identity);
        CollisionObj colObj2 = obj2.GetComponent<CollisionObj>();
        colliders.Add(colObj2);
        colObj2.changeColor();
    }

    void makeWall(){
        for(int i = 0; i < 12; i+=2){
            for(float j = -3; j < 3; j+=1.2f){
                Vector3 pos = new Vector3(-4, i, j);
                GameObject obj1 = Instantiate(colObject, pos, Quaternion.identity);
                CollisionObj colObj1 = obj1.GetComponent<CollisionObj>();
                colliders.Add(colObj1);
            }
        }
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
            collider.calculateAcceleration();
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
                //secondCollider.setPosition(temp.newSecondPos);
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
            
            float firstColMass = firstCollider.mass;
            float secondColMass = secondCollider.mass;

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

            float firstColMass = firstCollider.mass;
            float secondColMass = secondCollider.mass;

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

    void handleCollision(CollisionPoints points, CollisionObj firstCollider, CollisionObj secondCollider){
        if(points.hasCollision){
            Vector3 firstVelBefore = firstCollider.getVelocity();
            Vector3 secondVelBefore = secondCollider.getVelocity();

            secondCollider.setPosition(secondCollider.getPosition() + points.normal * points.penetrationDepth);

            firstCollider.setVelocity(Vector3.zero);
            secondCollider.setVelocity(Vector3.zero);

            firstCollider.setAcceleration(Vector3.zero);
            secondCollider.setAcceleration(Vector3.zero);
            
        }
    }

    void drawVertices(){
        foreach (Vector3 pos in toDraw){
            instantiateVertex(pos);
        }
    }

    void destoryAllOldVertexPrefabs(){
        foreach (GameObject vertex in vertexPrefabList){
            Destroy(vertex);
        }
    }

    void CreateObjCloser(float x){
        float y = 20;
        float z = 0;
        Vector3 pos = new Vector3(x, y, z);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }

    void CreateNewObj(){
        float extent = 2f;
        float randomX = UnityEngine.Random.Range(-extent, extent);
        float randomZ = UnityEngine.Random.Range(-extent, extent);
        float randomY = UnityEngine.Random.Range(15f, 25f);
        Vector3 pos = new Vector3(randomX, randomY, randomZ);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }

    public void instantiateVertex(Vector3 pos){
      GameObject obj = Instantiate(vertexPrefab, pos, Quaternion.identity);
      vertexPrefabList.Add(obj);
    }
}
