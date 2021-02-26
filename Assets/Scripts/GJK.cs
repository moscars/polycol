using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK : MonoBehaviour
{
    public List<Vector3> vertices;
    public GJK(){
      vertices = new List<Vector3>();
    }

    public void runGJK(CollisionObj obj1, CollisionObj obj2){
      if(gjk(obj1, obj2)){
        obj1.changeColorGJK();
        obj2.changeColorGJK();
      }
    }

    public bool gjk(CollisionObj obj1, CollisionObj obj2){
      return true;
    } 

    public List<Vector3> getVerticesOfObj(CollisionObj obj){
      List<Vector3> vert1 = obj.getVertices();
      changeColorIfUnder(obj);
      return vert1;
    }

    public void changeColorIfUnder(CollisionObj obj){
      Vector3 pos = obj.getPos();
      if(pos.y < 10){
        obj.changeColorGJK();
      }
    }

    public void emptyVertices(){
      vertices = new List<Vector3>();
    }

    public List<Vector3> getVerticesToLight(){
      return vertices;
    }
}
