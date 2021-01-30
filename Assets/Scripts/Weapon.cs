using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract void attack();
    public abstract void on_collision_hit(Collider other);
    public abstract void on_trigger_hit(Collision other);


    private void OnTriggerEnter(Collider other)
    {
        on_collision_hit(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        on_trigger_hit(collision);
    }
}
