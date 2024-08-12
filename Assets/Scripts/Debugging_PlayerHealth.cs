using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging_PlayerHealth : MonoBehaviour
{
    public int health = 20;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SetPlayerHealth(health); // Initialize the GameManager with the player's starting health

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
            GameManager.Instance.SetPlayerHealth(health); // Notify GameManager of the new health value
            Destroy(collision.gameObject);
            Debug.Log("Player Health:" + health);
        }
        
    }
}
