using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK
{
    //Cube[] cubes;

    void Start(){
      //  cubes = new List<Cube>();
    }



    void respondToCollision(Cube collider){
        collider.colliding();
    }
 
}
