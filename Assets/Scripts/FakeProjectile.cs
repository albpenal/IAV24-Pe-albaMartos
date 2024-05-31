using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeProjectile : MonoBehaviour
{
    private float chrono = 0;

    // Update is called once per frame
    void Update()
    {
        chrono += Time.deltaTime;
        if( chrono > 1.5f ) Destroy(gameObject);
    }
}
