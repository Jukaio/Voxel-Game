using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandle : WeaponHandle
{
    [SerializeField] Sword sword;

    public override void on_hold()
    {
        
    }
    public override void on_press()
    {
        sword.attack();
    }
    public override void on_release()
    {
        
    }
}
