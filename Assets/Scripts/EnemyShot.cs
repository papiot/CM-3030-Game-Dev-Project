using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    private float numHits = 0;
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
        if(collision.gameObject.tag == "bullet")
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
            numHits++;

            if(numHits > 2)
            {
                Destroy(gameObject);
            }
        }
    }
}
