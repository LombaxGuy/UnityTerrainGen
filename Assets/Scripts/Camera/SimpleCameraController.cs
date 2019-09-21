using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    float speed = 50;

    float borders = 100;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(0, 30, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        float translationX = Input.GetAxis("Vertical") * speed;
        float translationZ = Input.GetAxis("Horizontal") * speed;
        translationX *= Time.deltaTime;
        translationZ *= Time.deltaTime;
        transform.Translate(translationZ, 0, translationX, Space.World);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -borders, borders), transform.position.y, Mathf.Clamp(transform.position.z, -borders, borders));
    }
}
