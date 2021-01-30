using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] Animator anim;
    public override void attack()
    {
        Debug.Log("ATTACK");
        anim.Play("Slash");
    }

    public override void on_collision_hit(Collider other)
    {
      
    }

    public override void on_trigger_hit(Collision other)
    {

    }
}
