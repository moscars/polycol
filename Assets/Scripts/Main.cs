using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject collisionPrefab;

    private List <GameObject> objList;

    void Start()
    {
        objList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        CreateNewObj();
        foreach (GameObject obj in objList){

        }
    }

    void CreateNewObj(){
        Vector3 pos = new Vector3(20, 20, 20);
        Instatiate(collisionPrefab, Vector3.zero, Quaternion.identity);
    }
}
