using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCarControl : MonoBehaviour
{
    public GameObject goCar0;
    public GameObject goCar1;
    public GameObject goCar2;
    public GameObject goCar3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            goCar0.transform.Translate(Vector3.right * 50 * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            goCar1.transform.Translate(Vector3.right * 50 * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            goCar2.transform.Translate(Vector3.right * 50 * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            goCar3.transform.Translate(Vector3.right * 50 * Time.deltaTime);
        }

    }
}
