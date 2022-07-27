using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("WaterDrop"))
        {
            collision.transform.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}