using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK
{
    public GJK(){}

    public void runGJK(CollisionObj obj1, CollisionObj obj2){
      if(gjk(obj1, obj2)){
        obj1.changeColorGJK();
        obj2.changeColorGJK();
      }
    }

    public bool gjk(CollisionObj obj1, CollisionObj obj2){
      return true;
    }
 
}
