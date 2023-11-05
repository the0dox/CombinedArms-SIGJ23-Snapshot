using UnityEngine;

// classes that move through state should extend this class
public class StateController<T> : MonoBehaviour where T : StateController<T>
{
    // the current state object this class is set to
    protected State<T> _currentState;

    // called every frame
    protected virtual void Update()
    {
        _currentState.OnUpdate();
    }

    // called every physics update frame
    protected virtual void FixedUpdate()
    {
        _currentState.OnFixedUpdate();
    }

    // exits the current state and enters a newState
    public void SetState(State<T> newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter((T)this);
    }

    // returns true if new state is the same as active state
    public bool IsStateActive(State<T> newState)
    {
        return newState == _currentState;
    }
}

// this class should not actually be 
public class StateController : MonoBehaviour
{

}


