namespace GymSharp
{
    public class TimeStep
    {
        public string step_type;
        public float reward;
        public float discount;
        public int[,] observation;

        public TimeStep(string step_type, float reward, float discount, int[,] state)
        {
            this.step_type = step_type;

            this.reward = reward;
            this.discount = discount;
            observation = state;
        }

        public bool is_Last() => step_type == StepType.LAST;
        public bool is_First() => step_type == StepType.FIRST;
        public bool is_Mid() => step_type == StepType.MID;

        public static implicit operator TimeStep((string step_type, float reward, float discount, int[,] _state) v)
        {
            return new TimeStep(v.step_type, v.reward, v.discount, v._state);
        }
    }
}
