using System;

public class StateMachine<TState> where TState : Enum {
    public TState State { get; private set; }
    public event Action<TState> OnStateChanged;
    public void SetState(TState newState) { if (!Equals(State, newState)) { State = newState; OnStateChanged?.Invoke(State); } }
}
