using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GJK
{
    public GJK(){}

    public CollisionPoints runGJK(CollisionObj firstCollider, CollisionObj secondCollider){
      (bool gjkRes, CollisionPoints points) g = gjk(firstCollider, secondCollider);
      if(g.gjkRes){
        firstCollider.changeColorGJK();
        secondCollider.changeColorGJK();
      }
      return g.points;
    }

    public (bool, CollisionPoints) gjk(CollisionObj firstCollider, CollisionObj secondCollider){
      Vector3 dir = new Vector3(1, 1, 1);
      Vector3 firstPoint = findMaxPointInConvexHull(dir, firstCollider, secondCollider);
      List<Vector3> SimplexList = new List<Vector3>();
      SimplexList.Add(firstPoint);
      dir = -firstPoint;
      SimplexList.Insert(0, dir); 
      Vector3 retFalse = new Vector3(0, 0, 0);
      SimplexList.Insert(0, retFalse); 
      // First element in SimplexList is boolean flag
      // Second element in SimplexList is directionVector
      // The other elements in SimplexList are the Simplex points

      while(true){
        Vector3 direction = SimplexList[1];
        Vector3 newPoint = findMaxPointInConvexHull(direction, firstCollider, secondCollider);
        float newDotDir = Vector3.Dot(newPoint, direction);
        if(newDotDir <= 0){
          CollisionPoints p = new CollisionPoints(Vector3.zero, -1, false);
          return (false, p);
        }

        SimplexList.Insert(2, newPoint);
        SimplexList = NewSimplex(SimplexList);
        if(SimplexList[0] == new Vector3(1, 1, 1)){ // Check the boolean flag
          CollisionPoints points = EPA(firstCollider, secondCollider, SimplexList);
          return (true, points);
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

      return returnFunc(SimplexList, SimplexList[1], false);
    }


    public List<Vector3> Line(List<Vector3> SimplexList){
      Vector3 pointA = SimplexList[2];
      Vector3 pointB = SimplexList[3];
      Vector3 direction = SimplexList[1];

      Vector3 AB = pointB - pointA;
      Vector3 AO = -pointA;

      if(SameDir(AB, AO)){
        direction = Vector3.Cross(Vector3.Cross(AB, AO), AB);

        return returnFunc(SimplexList, direction, false);
      } else{
        SimplexList.RemoveAt(3);
        direction = AO;

        return returnFunc(SimplexList, direction, false);
      }
    }

      public List<Vector3> Triangle(List<Vector3> SimplexList){
        Vector3 pointA = SimplexList[2];
        Vector3 pointB = SimplexList[3];
        Vector3 pointC = SimplexList[4];
        Vector3 direction = SimplexList[1];

        Vector3 AB = pointB - pointA;
        Vector3 AC = pointC - pointA;
        Vector3 AO = -pointA;

        Vector3 ABC = Vector3.Cross(AB, AC);

        if(SameDir(Vector3.Cross(ABC, AC), AO)){
          
          if(SameDir(AC, AO)){
            SimplexList.RemoveAt(3);
            direction = Vector3.Cross(Vector3.Cross(AC, AO), AC);
            return returnFunc(SimplexList, direction, false);
          
          } else{
            SimplexList.RemoveAt(4);
            return Line(SimplexList);
          
          }

        } else{
          
          if(SameDir(Vector3.Cross(AB, ABC), AO)){
            SimplexList.RemoveAt(4);
            return Line(SimplexList);
          
          } else{
            
            if(SameDir(ABC, AO)){
              direction = ABC;
              return returnFunc(SimplexList, direction, false);
            
            } else{
              direction = -ABC;
              SimplexList[3] = pointC;
              SimplexList[4] = pointB;

              return returnFunc(SimplexList, direction, false);
            }
          }
        }
    }

    public List<Vector3> Tetrahedron(List<Vector3> SimplexList){
      Vector3 pointA = SimplexList[2];
      Vector3 pointB = SimplexList[3];
      Vector3 pointC = SimplexList[4];
      Vector3 pointD = SimplexList[5];
      Vector3 direction = SimplexList[1];

      Vector3 AB = pointB - pointA;
      Vector3 AC = pointC - pointA;
      Vector3 AD = pointD - pointA;
      Vector3 AO = -pointA;

      Vector3 ABC = Vector3.Cross(AB, AC);
      Vector3 ACD = Vector3.Cross(AC, AD);
      Vector3 ADB = Vector3.Cross(AD, AB);

      if(SameDir(ABC, AO)){
        SimplexList.RemoveAt(5); // Remove D
        return Triangle(SimplexList);
      }

      if(SameDir(ACD, AO)){
        SimplexList[3] = pointC;
        SimplexList[4] = pointD;
        SimplexList.RemoveAt(5);
        return Triangle(SimplexList);
      }

      if(SameDir(ADB, AO)){
        SimplexList[3] = pointD;
        SimplexList[4] = pointB;
        SimplexList.RemoveAt(5);
        return Triangle(SimplexList);
      }
      return returnFunc(SimplexList, direction, true);
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


    CollisionPoints EPA(CollisionObj firstCollider, CollisionObj secondCollider, List<Vector3> SimplexList){
      SimplexList.RemoveAt(0); // Remove boolean flag
      SimplexList.RemoveAt(0); // Remove dir
      //Debug.Log(SimplexList.Count);
      List<Vector3> polytope = SimplexList;
      List<int> faces = new List<int>{
        0, 1, 2,
        0, 3, 1,
        0, 2, 3,
        1, 3, 2
      };

      (List<Vector4> normals, int minFace) temp = GetFaceNormals(polytope, faces);
      List<Vector4> normals = temp.normals;
      int minFace = temp.minFace;

      Vector3 minNormal = Vector3.zero;
      float minDistance = float.PositiveInfinity;

      while(minDistance == float.PositiveInfinity){
        float x = normals[minFace].x;
        float y = normals[minFace].y;
        float z = normals[minFace].z;

        minNormal = new Vector3(x, y, z);
        minDistance = normals[minFace].w;

        Vector3 furtherPoint = findMaxPointInConvexHull(minNormal, firstCollider, secondCollider);
        float sDistance = Vector3.Dot(minNormal, furtherPoint);

        if(Math.Abs(sDistance - minDistance) > 0.001f){
          minDistance = float.PositiveInfinity;
          List<(int, int)> uniqueEdges = new List<(int, int)>();

          for(int i = 0; i < normals.Count; i++){
            if(SameDir(normals[i], furtherPoint)){
              int f = i * 3;

              uniqueEdges = AddIfUniqueEdge(uniqueEdges, faces, f, f + 1);
              uniqueEdges = AddIfUniqueEdge(uniqueEdges, faces, f + 1, f + 2);
              uniqueEdges = AddIfUniqueEdge(uniqueEdges, faces, f + 2, f);

              faces[f + 2] = faces[faces.Count - 1];
              faces.RemoveAt(faces.Count - 1);
              faces[f + 1] = faces[faces.Count - 1];
              faces.RemoveAt(faces.Count - 1);
              faces[f] = faces[faces.Count - 1];
              faces.RemoveAt(faces.Count - 1);

              normals[i] = normals[normals.Count - 1];
              normals.RemoveAt(normals.Count - 1);

              i--;
            }
          }
          
          List<int> newFaces = new List<int>();
          foreach ((int first, int second) edge in uniqueEdges){
            newFaces.Add(edge.first);
            //Debug.Log("firstedge:" + edge.first);
            newFaces.Add(edge.second);
            //Debug.Log("secondedge:" + edge.second);
            newFaces.Add(polytope.Count);
            //Debug.Log("polytope" + polytope.Count);
          }
          //Debug.Log("NewFaces:" + newFaces.Count);
          polytope.Add(furtherPoint);
          (List<Vector4> newNormals, int newMinFace) temp2 = GetFaceNormals(polytope, newFaces);
          List<Vector4> newNormals = temp2.newNormals;
          int newMinFace = temp2.newMinFace;

          float oldMinDistance = float.PositiveInfinity;

          for(int i = 0; i < normals.Count; i++){
            if(normals[i].w < oldMinDistance){
              oldMinDistance = normals[i].w;
              minFace = i;
            }
          }

          //Debug.Log("Length: " + newNormals.Count);
          //Debug.Log("MinT: " + newMinFace);

          if(newNormals[newMinFace].w < oldMinDistance){
            minFace = newMinFace + normals.Count;
          }

          foreach (int face in newFaces){
            faces.Add(face);
          }
          foreach (Vector4 normal in newNormals){
            normals.Add(normal);
          } 
        }  
      }

    CollisionPoints points = new CollisionPoints(minNormal, minDistance + 0.001f, true);
    return points;
    }


    List<(int, int)> AddIfUniqueEdge(List<(int, int)> edges, List<int> faces, int a, int b){
    
      bool found = false;
      (int, int) reversed = (faces[b], faces[a]);
      int index = 0;

      for(int i = 0; i < edges.Count; i++){
        (int, int) elem = edges[i];
        if(elem == reversed){
          found = true;
          index = i;
        }
      }

      if(found){
        edges.RemoveAt(index); 
      } else{
        edges.Add((faces[a], faces[b]));
      }

      return edges;
    }



    (List<Vector4>, int) GetFaceNormals(List<Vector3> polytope, List<int> faces){
      List<Vector4> normals = new List<Vector4>();
      int minTriangle = 0;
      float minDistance = float.PositiveInfinity;

      for(int i = 0; i < faces.Count; i += 3){
        Vector3 A = polytope[faces[i]];
        Vector3 B = polytope[faces[i + 1]];
        Vector3 C = polytope[faces[i + 2]];

        Vector3 AB = B - A;
        Vector3 AC = C - A;

        Vector3 normal = Vector3.Cross(AB, AC);
        normal.Normalize();
        float distance = Vector3.Dot(normal, A);

        if (distance < 0){
          normal *= -1;
          distance *= -1;
        }

        Vector4 newNormal = new Vector4(normal.x, normal.y, normal.z, distance);
        normals.Add(newNormal);

        if(distance < minDistance){
          minTriangle = i / 3;
          minDistance = distance;
        }
      }
      //Debug.Log("faces: " + faces.Count);
      //Debug.Log("Length in func: " + normals.Count);
      //Debug.Log("MinT in Func: " + minTriangle);
      return (normals, minTriangle);
    }

}



