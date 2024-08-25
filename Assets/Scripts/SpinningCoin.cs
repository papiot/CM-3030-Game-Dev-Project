using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCoin : MonoBehaviour
{
    // fields for adding x, y, z values for rotation in Unity
    //[SerializeField] float xRotate;
    [SerializeField] float yRotate;
    //[SerializeField] float zRotate;

    // Update is called once per frame
    void Update()
    {
        // rotate the object by the x , y , z values entered in Unity
        transform.Rotate(0,
                        yRotate,
                        0);

    }
}