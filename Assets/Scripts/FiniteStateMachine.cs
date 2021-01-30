using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Finite               -> Collection
    Finite.State         -> Polymorphic State
    Finite.State.Machine -> Execution of States 
*/

public class Finite
{
    private List<IState> states = new List<IState>();
    private IState reset = null;
    private IState current = null;
    private IState next = null;
    public bool IsValid { get { return current != null; } }
    public IState Current { get { return current; } }

    public enum Mode
    {
        Run,
        Next,
        Reset
    }
    public interface IState
    {
        public abstract void enter(Finite collection);
        public abstract Mode update(Finite collection);
        public abstract void exit(Finite collection);
    }
    public abstract class State : IState
    {
        private GameObject context;
        public GameObject Context { get { return context; } }
        public State(GameObject context)
        {
            this.context = context;
        }

        public abstract void enter(Finite collection);
        public abstract Mode update(Finite collection);
        public abstract void exit(Finite collection);

        public class Machine
        {
            Finite collection;

            public Machine(IState state)
            {
                collection = new Finite(state);
            }

            public void update()
            {
                if (!collection.IsValid)
                    return;

                switch (collection.Current.update(collection))
                {
                    case Mode.Next:
                        change();
                        break;
                    case Mode.Reset:
                        collection.next = null;
                        change();
                        break;
                }
            }

            private void change()
            {
                if (collection.next != null)
                {
                    collection.current.exit(collection);
                    collection.current = collection.next;
                    collection.current.enter(collection);
                    return;
                }

                collection.current.exit(collection);
                collection.current = collection.reset;
                collection.current.enter(collection);
            }
        }
    }

    public Finite(IState state)
    {
        current = state;
        reset = state;
        states.Add(state);
    }
    public void set_reset(IState state)
    {
        reset = state;
    }

    public void add(IState state)
    {
        states.Add(state);
    }
    public void remove(IState state)
    {
        states.Remove(state);
    }
    public void set_next(IState to)
    {
        foreach (var state in states)
        {
            if (state.Equals(to))
                next = state;
        }
    }

}
