using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandle : MonoBehaviour
{
    private static int CURRENT { get { return 0; } }
    private static int PREVIOUS { get { return 1; } }
    private bool[] state = { false, false };

    public virtual void on_release() { }
    public virtual void on_press() { }
    public virtual void on_hold() { }


    void Update()
    {
        state[PREVIOUS] = state[CURRENT];
        state[CURRENT] = Input.GetButton("Attack");
    }
    void FixedUpdate()
    {
        if(state[CURRENT] && !state[PREVIOUS])
        {
            on_press();
        }

        if (!state[CURRENT] && state[PREVIOUS])
        {
            on_release();
        }

        if (state[CURRENT] && state[PREVIOUS])
        {
            on_hold();
        }

    }
}
