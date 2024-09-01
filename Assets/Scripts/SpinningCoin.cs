using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCoin : MonoBehaviour
{
    // fields for adding x, y, z values for rotation in Unity
    //[SerializeField] float xRotate;
    [SerializeField] float yRotate;
    //[SerializeField] float zRotate;
    private PauseMenu gameState;
    private void Start()
    {
        gameState = GameObject.Find("PersistentManager").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameState.isPaused)
        {
            // rotate the object by the x , y , z values entered in Unity
            transform.Rotate(0, yRotate, 0);
        }

    }
}