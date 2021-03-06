using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollisionObj : MonoBehaviour
{
    public Vector3 gravityForce = new Vector3(0, -9.81f, 0);

    public Vector3 force;
    public Vector3 velocity;
    public float mass;
    Vector3 acceleration;
    public bool isColliding;
    Vector3 position;

    
    Quaternion orientation;
    Vector3 angularMomentum;

    Quaternion spin;
    Vector3 angularVelocity;

    float inertia;
    float inverseIntertia;

    Vector3 torque;
    float x = 0;


    void Start(){
        //initilize cube
        mass = 2000;
        isColliding = false;
        force = Vector3.zero;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        addForce(gravityForce);
        inertia = 0.16666f * mass;
        inverseIntertia = 1 / inertia;
        torque = Vector3.zero;
        angularMomentum = Vector3.zero;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 30, 0));
    }

    public void reCalculateRotation(){

        //angularVelocity = angularMomentum * inverseIntertia; //Done

        orientation.Normalize();

        //Quaternion q  = (0f, angularVelocity.x, angularVelocity.y, angularVelocity.z); DOne

        //spin = 0.5f * q * orientation; Doneish not 0.5
    }

    public void updateOrientation(){
        spin = Quaternion.Euler(angularVelocity);
        //Debug.Log("spin: " + spin);
        //transform.rotation = new Quaternion(1, 1, 1, 1); //spin;
        //Debug.Log("rot: " + transform.rotation);
        transform.rotation *= spin;
        //transform.rotation;
        //x++;
        //transform.rotation *= Quaternion.Euler(0, 0, 45);
    }

    public void addTorque(Vector3 torque){
        this.torque += torque;
        //angularVelocity += torque;
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + torque);
    }

    public void setAngularVelocity(Vector3 angularVelocity){
        this.angularVelocity = angularVelocity;
    }

    public void zeroTorque(){
        torque = Vector3.zero;
    }

    public void updateAngularVelocity(){
        angularMomentum += torque;
        //Debug.Log("angmom = " + angularMomentum + " and tor = : " + torque);

        angularVelocity = angularMomentum * inverseIntertia;
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
        }
    }

    public void addForce(Vector3 force){
        this.force += force;
    }

    public void calculateAcceleration(){
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
        //Get vertices here
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
