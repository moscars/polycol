﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollisionObj : MonoBehaviour
{
    public Vector3 gravityForce = new Vector3(0, -9.81f, 0);

    public Vector3 force;
    public Vector3 velocity;
    float mass = 2000;
    Vector3 acceleration;
    public bool isColliding;
    public Vector3 position;

    void Start(){
        //initilize cube
        isColliding = false;
        force = Vector3.zero;
        acceleration = Vector3.zero;
        addForce(gravityForce);
        velocity = Vector3.zero;
    }

    public void moveUp(){
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
    }

    public float getMass(){
        return mass;
    }

    public void resetNetForce(){
        force = Vector3.zero;
    }

    public Vector3 getVelocity(){
        return velocity;
    }

    public void setVelocity(Vector3 vel){
        velocity = vel;
    }

    public void setAcceleration(Vector3 acc){
        acceleration = acc;
    }

    public void setPosition(Vector3 pos){
        transform.position = pos;
    }

    public Vector3 getPosition(){
        return transform.position;
    }

    public Vector3 getAcceleration(){
        return acceleration;
    }

    public void applyGravity(){
        force += gravityForce;
    }

    public void zeroAllMovement(){
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
    }

    public void dontFallThroughFloor(){
        if(transform.position.y < 0.5f){
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            setVelocity(new Vector3(velocity.x, 0, velocity.z));
        }
    }

    public void addForce(Vector3 force){
        this.force += force;
    }

    public void updateAcceleration(){
        acceleration = force / mass;
    }

    public void updateVelocity(){
        velocity += acceleration;
    }

    public void zeroVelocity(){
        velocity = Vector3.zero;
    }

    public void zeroAcceleration(){
        acceleration = Vector3.zero;
    }

    public void updatePos(){
        transform.position = transform.position + velocity;
    }

    public void colliding(){
        changeColor();
    }

    public void changeColor(){
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void changeColorGJK(){
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void setCollidingToTrue(){
        isColliding = true;
    }

    public void setCollidingToFalse(){
        isColliding = false;
    }

    public List<Vector3> getVertices(){
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] v = mesh.vertices;
        List<Vector3> vertices = new List<Vector3>();
        foreach (Vector3 vec in v){
            Vector3 worldPoint = transform.TransformPoint(vec);
            if(!vertices.Contains(worldPoint)){
                vertices.Add(worldPoint);
            }
        }
        return vertices;
    }

    public Vector3 getPos(){
        return transform.position;
    }
    
}
