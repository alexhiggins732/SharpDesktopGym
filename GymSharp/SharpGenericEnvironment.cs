using System;
using System.Collections.Generic;

namespace GymSharp
{
    public abstract class SharpGenericEnvironment<TState, TAction>
    {
        static SharpGenericEnvironment()
        {
            PythonInitiliazer.InitializePython();
        }
        protected dynamic _rng;


        public dynamic action_spec;
        public dynamic observation_spec;
        public dynamic _shape;
        public TState _state;
        public TimeStep<TState> _current_time_step;
        protected int steps = 0;
        public int StepCount => steps;
        public float discount;

        public SharpGenericEnvironment()
        {

        }

        public virtual TimeStep<TState> current_time_step()
        {
            return _current_time_step;
        }

        public virtual TimeStep<TState> reset(Func<TState> stateGenerator)
        {
            steps = 0;
            //TODO: get shape of obersevation spec and initial default state using the shape.
            _state = stateGenerator == null ? default(TState) : stateGenerator();
            this._current_time_step = new TimeStep<TState>(StepType.FIRST, 0, this.discount, _state);
            return _current_time_step;

        }
        public abstract TimeStep<TState> _step(TAction action);


        protected virtual TimeStep<TState> _set_time_step(string step_type, float reward, float discount, TState _state)
        {
            return set_time_step(new TimeStep<TState>(step_type, reward, discount, _state));
        }

        public virtual TimeStep<TState> set_time_step(TimeStep<TState> timeStep)
        {
            this._current_time_step = timeStep;
            this._state = timeStep.observation;
            return this._current_time_step;
        }

        public virtual TimeStep<TState> _set_state(TState _state, string step_type, float reward, float discount)
        {
            return set_time_step((step_type, reward, discount, _state));
        }
    }
}