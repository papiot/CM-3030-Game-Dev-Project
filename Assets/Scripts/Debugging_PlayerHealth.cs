using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging_PlayerHealth : MonoBehaviour
{
    public int health = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="enemy damage")
        {
            health -=1;
            Destroy(collision.gameObject);
        }
        Debug.Log("Player Health:" + health);
    }
}
