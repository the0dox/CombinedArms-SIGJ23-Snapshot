// template for an individual state of a finite state machine
// each state is matched to a specific state machine type TController such that an enemy state couldn't be assigned to a player controller state machine for example
public abstract class State<TController> where TController : StateController<TController>
{
    // reference to the state machine that controls this is how the state accesses its gameobject and any components attached to it
    protected TController _myContext;

    // called when state is first entered
    public void OnEnter(TController context)
    {
        _myContext = context;
        OnStateEnter();
    }

    // called every frame whilist this is the active state
    public void OnUpdate()
    {
        OnStateUpdate();
    }
    
    // called every fixed update frame whilist this is the active state
    public void OnFixedUpdate()
    {
        OnStateFixedUpdate();
    }

    // called when the state is exited from
    public void OnExit()
    {
        OnStateExit();
    }

    // allows class that derive State to 
    protected abstract void OnStateEnter();

    // called every frame
    protected abstract void OnStateUpdate();

    // called every physics update frame
    protected abstract void OnStateFixedUpdate();

    // called when this state is ended
    protected abstract void OnStateExit();
}