using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;
    List<CollisionObj> colliders;
    public GJK gjkAlgo;

    int counter = 0;

    void Start()
    {
        colliders = new List<CollisionObj>();
        gjkAlgo = new GJK();
    }

    // Update is called once per frame
    void Update()
    {
        if(counter % 10 == 0){
            CreateNewObj();
        }
        counter++;
        if(counter > 10000){
            counter = 0;
        }
        runGJK();
    }

    void runGJK(){
        foreach (CollisionObj collider in colliders){
          
        }
        if(colliders.Count > 3){
            CollisionObj obj1 = colliders[colliders.Count - 1];
            CollisionObj obj2 = colliders[colliders.Count - 2];
            gjkAlgo.runGJK(obj1, obj2);
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
}
