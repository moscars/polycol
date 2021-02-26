using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollisionObj : MonoBehaviour
{
    public Vector3 gravityForce = new Vector3(0, -9.82f, 0);
    public Vector3 force;
    Vector3 velocity;
    float mass = 2000;
    Vector3 acceleration;
    public bool isColliding;
    Vector3 position;
    public List<Vector3> vertices;

    void Start(){
        //initilize cube
        isColliding = false;
        force = Vector3.zero;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        addForce(gravityForce);
    }


    void Update(){
        if (isColliding){
            changeColor();
        }
    }

    void FixedUpdate(){
        if(transform.position.y <= 0.51f){
            stopAtButtom();
            isColliding = true;
        } else{
            updateVelocity();
            updatePos();
            calculateAcceleration();
        }
    }

    void stopAtButtom(){
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
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
        GetComponent<Renderer>().material.color = Color.blue;
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
         List<Vector3> vertices = new List<Vector3>(v);
         return vertices;
    }

    public Vector3 getPos(){
        return transform.position;
    }
    
}
