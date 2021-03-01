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
    float x;

    int counter = 0;

    void Start()
    {
        colliders = new List<CollisionObj>();
        vertexPrefabList = new List<GameObject>();
        gjkAlgo = new GJK();
        x = 0;  
    }

    // Update is called once per frame
    void Update()
    {
        toDraw = new List<Vector3>();
        destoryAllOldVertexPrefabs();
        if(counter % 25 == 0){
            /*CreateObjCloser(x);
            x += 1.7f;
            counter = 1;*/
            CreateNewObj();
        }
        counter++;
        runGJK();
        drawVertices();
    }

    void runGJK(){
        foreach(CollisionObj collider in colliders){
            foreach(CollisionObj collider2 in colliders){
                if(collider != collider2){
                    gjkAlgo.runGJK(collider, collider2);
                }
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

    void CreateObjCloser(float x){
        float y = 20;
        float z = 0;
        Vector3 pos = new Vector3(x, y, z);
        GameObject obj = Instantiate(colObject, pos, Quaternion.identity);
        CollisionObj colObj = obj.GetComponent<CollisionObj>();
        colliders.Add(colObj);
    }

    void CreateNewObj(){
        float extent = 4f;
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
