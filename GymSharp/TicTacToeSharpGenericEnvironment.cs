using Newtonsoft.Json;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSharp
{
    public class TicTacToeSharpGenericEnvironment :
        SharpGenericEnvironment<int[,], int[]>
    {

        static TicTacToeSharpGenericEnvironment()
        {
            PythonInitiliazer.InitializePython();
        }

        public readonly float[] REWARD_WIN = { 1 };
        public readonly float[] REWARD_LOSS = { -1 };
        public readonly float[] REWARD_DRAW_OR_NOT_FINAL = { 0 };
        public readonly float[] REWARD_ILLEGAL_MOVE = { -.15f };
        readonly float[] penalties = new[] { 0f, -1f, -.8f, -.6f, -.4f, -.2f };
        private dynamic _rng;
        public bool UseDynamicReward = false;

        public TicTacToeSharpGenericEnvironment(dynamic rng = null, float discount = 1.0f)
        {
            _rng = rng;
            this.discount = discount;
        }

        public void _set_state(int[,] _state, string step_type, float reward, float discount)
        {
            set_time_step((step_type, reward, discount, _state));
        }

        public override TimeStep<int[,]> current_time_step()
        {
            return base._current_time_step;
        }

        public TimeStep<int[,]> reset()
        {
            return base.reset(() => new int[3, 3]);
        }

        protected override TimeStep<int[,]> _set_time_step(string step_type, float reward, float discount, int[,] _state)
        {
            return base.set_time_step(new TimeStep<int[,]>(step_type, reward, discount, _state));
        }

        public override TimeStep<int[,]> _step(int[] action)
        {
            steps++;
            int[,] states = _state;
            if (_current_time_step.is_Last() == true)
            {
                return reset();
            }
            if (states[action[0], action[1]] != 0)
            {
                var illegal_reward = UseDynamicReward ? penalties[steps] : REWARD_ILLEGAL_MOVE[0];
                return set_time_step((StepType.LAST, illegal_reward, discount, _state));
            }

            states[action[0], action[1]] = 1;

            (bool is_final, float reward) = _check_states(states);

            if (is_final)
            {
                return set_time_step((StepType.LAST, reward, discount, _state));
            }


            (int o_x, int o_y) = _opponent_play(states);
            states[o_x, o_y] = 2;

            (is_final, reward) = _check_states(states);

            var step_type = StepType.MID;
            if (states.Cast<int>().All(x => x == 0))
            {
                step_type = StepType.FIRST;
            }
            else if (is_final)
            {
                step_type = StepType.LAST;
            }
            return set_time_step((step_type, reward, discount, states));
        }

        public string _check_states_debug(int[,] state)
        {
            var output = _check_states(state);
            var input_json = JsonConvert.SerializeObject(state);
            var output_json = JsonConvert.SerializeObject(output);
            var result = $"Input: {input_json}{Environment.NewLine}Ouput:{output_json}";
            return result;
        }

        public Tuple<bool, float> _check_states(int[,] state)
        {
            int[,] states = state;
            var flattened = states.Cast<int>().ToArray();
            bool player1Won = check_state(flattened, 1);
            bool player2Won = check_state(flattened, 2);
            bool draw = flattened.All(x => x != 0);
            bool done = player1Won || player2Won || draw;

            if (player1Won)
            {
                return (true, REWARD_WIN[0]).ToTuple();
            }
            if (player2Won)
            {
                return (true, REWARD_LOSS[0]).ToTuple();
            }
            if (!done)
            {
                return (false, REWARD_DRAW_OR_NOT_FINAL[0]).ToTuple();
            }
            return (true, REWARD_DRAW_OR_NOT_FINAL[0]).ToTuple();
        }

        private bool check_state(int[] state, int player)
        {
            bool player_won = false;
            if (state[0] == player && state[1] == player && state[2] == player)
            {
                player_won = true;
            }
            else if (state[3] == player && state[4] == player && state[5] == player)
            {
                player_won = true;
            }
            else if (state[6] == player && state[7] == player && state[8] == player)
            {
                player_won = true;
            }
            if (state[0] == player && state[3] == player && state[6] == player)
            {
                player_won = true;
            }
            else if (state[1] == player && state[4] == player && state[7] == player)
            {
                player_won = true;
            }
            else if (state[2] == player && state[5] == player && state[8] == player)
            {
                player_won = true;
            }
            else if (state[0] == player && state[4] == player && state[8] == player)
            {
                player_won = true;
            }
            else if (state[2] == player && state[4] == player && state[6] == player)
            {
                player_won = true;
            }
            return player_won;
        }

        public string _legal_actions_debug(int[,] state)
        {
            var result = _legal_actions(state);
            int[,] states = state;
            var json = JsonConvert.SerializeObject(result);
            return $"Rank: {states.Rank}: Len: {states.GetLength(1)} - {json}";

        }

        public int[,] legal_actions() => _legal_actions(_state);

        public int[,] _legal_actions(int[,] state)
        {
            int[,] states = state;
            var rank = states.Rank;
            var len1 = states.Length;

            var numRows = states.GetLength(0);
            var numColumns = states.GetLength(1);

            List<int[]> candidates = new List<int[]>();
            for (var row = 0; row < numRows; row++)
            {
                for (var column = 0; column < numColumns; column++)
                {

                    if (states[row, column] == 0) candidates.Add(new[] { row, column });
                }
            }
            if (candidates.Count > 0)
            {
                var resultDims = candidates.Count;
                var resultLen = candidates.First().Length;
                var result = new int[resultDims, resultLen];
                for (var i = 0; i < resultDims; i++)
                {
                    var row = candidates[i];
                    for (var j = 0; j < row.Length; j++)
                        result[i, j] = row[j];
                }

                var result2 = candidates.To2DFast();
                return result2;
            }
            return null;

        }

        public (int, int) _opponent_play(int[,] state)
        {
            int[,] actions = _legal_actions(state);
            if (actions == null)
                throw new Exception("There is no empty space for opponent to play at.");

            int i = _rng == null ? 0 : randint(actions.GetLength(0));
            var result = (actions[i, 0], actions[i, 1]);
            return result;

        }

        public int randint(int low)
        {
            using (Py.GIL())
            {
                var result = (int)_rng.randint(low);
                return result;
            }

        }
        public int randint(int low, int high)
        {

            using (Py.GIL())
            {
                var result = (int)_rng.randint(low, high);
                return result;
            }

        }
    }
}
