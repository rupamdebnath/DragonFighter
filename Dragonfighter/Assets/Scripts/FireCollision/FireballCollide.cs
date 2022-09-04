using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballCollide : MonoBehaviour
{
    private void OnCollisionEnter(Collision target)
    {
        Debug.Log(target.gameObject.tag);
        if (target.gameObject.tag == "Enemy")
        {
            target.gameObject.GetComponent<EnemyController>().SetHealth(20);
        }
        else if (target.gameObject.tag == "Player")
            target.gameObject.GetComponent<PlayerController>().ReduceHealth(20);
    }
}
