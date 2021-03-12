using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollisionObj : MonoBehaviour
{
    private Vector3 gravityForce = new Vector3(0, -9.81f, 0);
    private Vector3 force;
    private Vector3 velocity;
    private float mass = 2000;
    private Vector3 acceleration;
    private const float width = 0.5f;

    

    void Start(){
        force = Vector3.zero;
        acceleration = Vector3.zero;
        addForce(gravityForce);
        velocity = Vector3.zero;
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

    public void dontMoveOutsideArea(float areaExtent){
        Vector3 pos = getPosition();
        if(pos.x + width >= areaExtent || pos.x - width <= -areaExtent){
            setVelocity(new Vector3(-velocity.x, velocity.y, velocity.z));
        }
        if(pos.z + width >= areaExtent || pos.z - width <= -areaExtent){
            setVelocity(new Vector3(velocity.x, velocity.y, -velocity.z));
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
    
}
