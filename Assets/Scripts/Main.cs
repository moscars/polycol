using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;
    public GameObject vertexPrefab;
    List<CollisionObj> colliders;
    List<GameObject> vertexPrefabList;
    public GJK gjkAlgo;
    List<Vector3> toDraw;

    int counter = 0;

    void Start()
    {
        colliders = new List<CollisionObj>();
        vertexPrefabList = new List<GameObject>();
        gjkAlgo = new GJK();
    }

    // Update is called once per frame
    void Update()
    {
        toDraw = new List<Vector3>();
        destoryAllOldVertexPrefabs();   
        if(counter % 50 == 0){
            CreateNewObj();
            counter = 1;
        }
        counter++;
        runGJK();
        drawVertices();
    }

    void runGJK(){
        foreach (CollisionObj collider in colliders){
            List<Vector3> list = gjkAlgo.getVerticesOfObj(collider);
            foreach(Vector3 vertex in list){
                toDraw.Add(vertex);
            }
        }
    }

    void drawVertices(){
        foreach (Vector3 pos in toDraw){
            instantiateVertex(pos);
        }
    }

    void destoryAllOldVertexPrefabs(){
        foreach (GameObject vertex in vertexPrefabList){
            Destroy(vertex);
        }
    }

    void CreateNewObj(){
        float extent = 8f;
        float randomX = Random.Range(-extent, extent);
        float randomZ = Random.Range(-extent, extent);
        float randomY = Random.Range(15f, 25f);
        Vector3 pos = new Vector3(randomX, randomY, randomZ);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }

    public void instantiateVertex(Vector3 pos){
      GameObject obj = Instantiate(vertexPrefab, pos, Quaternion.identity);
      vertexPrefabList.Add(obj);
    }
}
