using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject player;
    private Finite.State.Machine FSM;

    public class Default : Finite.State
    {
        public Default(GameObject context) 
            : base(context)
        {
        }

        public override void enter(Finite collection)
        {
            
        }

        public override void exit(Finite collection)
        {
            
        }

        public override Finite.Mode update(Finite collection)
        {
            return Finite.Mode.Run;
        }
    }

    void Start()
    {
        FSM = new Finite.State.Machine(new Default(gameObject));
    }

    void Update()
    {
        FSM.update();
    }
}
