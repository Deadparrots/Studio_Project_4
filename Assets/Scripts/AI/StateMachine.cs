using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateStuff
{
    public class StateMachine<T>
    {
        public State<T> currentState { get; set; }
        public State<T> nextState { get; set; }
        public T owner;
        private Dictionary<string, State<T>> state_dictionary;
        public StateMachine(T _owner)
        {
            owner = _owner;
            currentState = null;
            state_dictionary = new Dictionary<string, State<T>>();
        }

        public void SetNextState(string nextStateID)
        {
            if(state_dictionary.ContainsKey(nextStateID))
            {
                foreach (var item in state_dictionary)
                {
                    if (item.Key == nextStateID)
                        nextState = item.Value;
                }
            }
        }

        public void AddState(State<T> _newState)
        {
            if (_newState == null)
                return;
            if (state_dictionary.ContainsKey(_newState.GetStateID()))
                return;

            if (currentState == null)
                currentState = nextState = _newState;
            state_dictionary.Add(_newState.GetStateID(), _newState);
        }

        public string GetCurrentState()
        {
            if (currentState != null)
                return currentState.GetStateID();
            return "No Current State!";
        }

        public void Update(double dt)
        {
            if(nextState != currentState)
            {

                currentState.Exit();
                currentState = nextState;
                currentState.Enter();
            }

            if (currentState != null)
                currentState.Update(dt);
        }
    }

    public abstract class State<T>
    {
        string m_stateID;

        protected State(string _stateID)
        {
            m_stateID = _stateID;
        }

        public string GetStateID()
        {
            return m_stateID;
        }

        public abstract void Enter();
        public abstract void Update(double dt);
        public abstract void Exit();

    }
}

