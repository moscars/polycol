using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colObject;

    private List <GameObject> objList;
    int counter = 0;

    void Start()
    {
        CreateNewObj();
    }

    // Update is called once per frame
    void Update()
    {
        if(counter % 25 == 0){
            CreateNewObj();
        }
        counter++;
        if(counter > 10000){
            counter = 0;
        }
    }

    void CreateNewObj(){
        Vector3 pos = new Vector3(0, 20, 0);
        Instantiate(colObject, pos, Quaternion.identity);
    }
}
