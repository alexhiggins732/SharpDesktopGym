namespace GymSharp
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// SciSharp uses strongly typed Step(NDArray observation, float reward, bool done, Dict information)
    ///     Additional meta data goes in the Dict
    /// OpenAI Gym uses:
    ///     observation (object): agent's observation of the current environment
    ///     reward(float) : amount of reward returned after previous action
    ///     done(bool): whether the episode has ended, in which case further step() calls will return undefined results
    ///     info(dict): contains auxiliary diagnostic information (helpful for debugging, and sometimes learning)
    /// Tensoflow env uses:
    ///     ts.transition(self._get_observation(), 0) which returns a TimeStep:
    ///         observation: types.NestedTensorOrArray, (this takes a step_type in the constructor.
    ///         reward: types.NestedTensorOrArray,
    ///         discount: types.Float = 1.0,
    ///         outer_dims: Optional[types.Shape] = None) -> TimeStep:
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class TimeStep<T>
    {
        public string step_type;
        public float reward;
        public float discount;
        public T observation;

        public TimeStep(string step_type, float reward, float discount, T state)
        {
            this.step_type = step_type;

            this.reward = reward;
            this.discount = discount;
            observation = state;
        }

        public bool is_Last() => step_type == StepType.LAST;
        public bool is_First() => step_type == StepType.FIRST;
        public bool is_Mid() => step_type == StepType.MID;

        public static implicit operator TimeStep<T>((string step_type, float reward, float discount, T _state) v)
        {
            return new TimeStep<T>(v.step_type, v.reward, v.discount, v._state);
        }
    }
}