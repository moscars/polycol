using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public Vector3 gravityForce = new Vector3(0, -9.82f, 0);
    public Vector3 force;
    Vector3 velocity;
    float mass = 2000;
    Vector3 acceleration;
    //Material m_Material;
    Renderer cubeRenderer;

    void Start(){
        //Prepare for being able to change color
        cubeRenderer = this.GetComponent<Renderer>();
        
        //initilize cube
        force = Vector3.zero;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        addForce(gravityForce);
    }

    void FixedUpdate(){
        if(transform.position.y <= 0.51f){
            stopAtButtom();
            changeColor();
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
        cubeRenderer.material.SetColor("_Color", Color.blue);
    }
    
}
