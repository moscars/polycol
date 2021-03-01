using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK
{
    public GJK(){}

    public void runGJK(CollisionObj firstCollider, CollisionObj secondCollider){
      if(gjk(firstCollider, secondCollider)){
        //firstCollider.changeColorGJK();
        //secondCollider.changeColorGJK();
      }
    }

    public bool gjk(CollisionObj firstCollider, CollisionObj secondCollider){
      Vector3 dir = new Vector3(1, 1, 1);
      Vector3 firstPoint = findMaxPointInConvexHull(dir, firstCollider, secondCollider);

      List<Vector3> SimplexList = new List<Vector3>();
      SimplexList.Add(firstPoint);

      dir = -firstPoint;
      SimplexList.Insert(dir); 
      Vector3 retFalse = new Vector3(0, 0, 0);
      SimplexList.Insert(retFalse); 
      // First element in SimplexList is boolean flag
      // Second element in SimplexList is directionVector
      // The other elements in SimplexList is the actual Simplex

      while(1){
        Vector3 direction = SimplexList[1];
        Vector3 newPoint = findMaxPointInConvexHull(direction, firstCollider, secondCollider);
        Vector3 newDotDir = Vector3.Dot(newPoint, direction);
        
        if(newDotDir < 0){
          return false;
        }

        SimplexList.Add(newPoint);
        SimplexList = NewSimplex(SimplexList);
        if(SimplexList[0] == new Vector3(1, 1, 1)){ // Check the boolean flag
          return true;
        }
      }

    } 


    public List<Vector3> NewSimplex(List<Vector3> SimplexList){
      switch (SimplexList.Count)
      {
          case 4: return Line(SimplexList); // 1 flag, 1 dir, 2 points
          case 5: return Triangle(SimplexList);
          case 6: return Tetrahedron(SimplexList);
      }
    }


    public List<Vector3> Line(List<Vector3> SimplexList){
      Vector3 pointA = SimplexList[2];
      Vector3 pointB = SimplexList[3];
      Vector3 direction = SimplexList[1];

      Vector3 AB = pointB - pointA;
      Vector3 AO = -pointA;

      if(SameDir(AB, AO)){
        direction = Vector3.CrossProduct(AB, Vector3.CrossProduct(AO, AB));
        
        return returnFunc(SimplexList, direction, false);
      } else{
        SimplexList.RemoveAt(3);
        direction = AO;

        return returnFunc(SimplexList, direction, false);
      }
    }

    public List<Vector3> returnFunc(List<Vector3> SimplexList, Vector3 direction, bool returnValue){
      Vector3 retTrue = new Vector3(1, 1, 1);
      Vector3 retFalse = Vector3.zero;

      if(returnValue){
        SimplexList[0] = retTrue;
      }

      SimplexList[1] = direction;

      return SimplexList;
    }

    public bool SameDir(Vector3 dir, Vector3 vec){
      if(Vector3.Dot(dir, vec) > 0){
        return true;
      }
      return false;
    }

    public Vector3 findMaxPointInConvexHull(Vector3 dir, CollisionObj firstCollider, CollisionObj secondCollider){
      List<Vector3> firstColliderPoints = firstCollider.getVertices();
      List<Vector3> secondColliderPoints = secondCollider.getVertices();

      Vector3 maxFirst = findMaxPointInDirection(dir, firstColliderPoints);
      Vector3 maxSecond = findMaxPointInDirection(-dir, secondColliderPoints);

      Vector3 res = maxFirst - maxSecond;
      return res;
    }

    public Vector3 findMaxPointInDirection(Vector3 direction, List<Vector3> points){
      float maxdot = float.NegativeInfinity;
      Vector3 result = Vector3.zero;
      foreach (Vector3 point in points){
        float dotRes = Vector3.Dot(point, direction);
        if(dotRes > maxdot){
          maxdot = dotRes;
          result = point;
        }
      }
      return result;
    }

}
