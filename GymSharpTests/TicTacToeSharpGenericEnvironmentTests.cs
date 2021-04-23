using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using static GymSharp.ArrayExtensions;
using System.Runtime.CompilerServices;

namespace GymSharp.Tests
{
    [TestClass()]
    public class TicTacToeSharpGenericEnvironmentTests
    {
        private dynamic np;
        TicTacToeSharpGenericEnvironment env;

        public TicTacToeSharpGenericEnvironmentTests()
        {
            GymSharp.PythonInitiliazer.InitializePython();
        }

        [TestInitialize]
        public void Setup()
        {
            using (Py.GIL())
            {
                np = Py.Import("numpy");
            }
            np.random.seed(0);
            env = new TicTacToeSharpGenericEnvironment(null);
            var ts = env.reset();
            (new int[3, 3]).AssertEquals(ts.observation);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void test_check_states()
        {
            var expected = (false, 0.0f);
            var actual = env._check_states(new[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } });
            assert_tuples_equal(expected, actual);

            expected = (true, -1.0f);
            actual = env._check_states(new[,] { { 2, 2, 2 }, { 0, 1, 1 }, { 0, 0, 0 } });
            assert_tuples_equal(expected, actual);

            expected = (false, 0.0f);
            actual = env._check_states(new[,] { { 2, 2, 1 }, { 1, 2, 1 }, { 1, 0, 0 } });
            assert_tuples_equal(expected, actual);

            expected = (true, 0.0f);
            actual = env._check_states(new[,] { { 2, 1, 2 }, { 1, 2, 1 }, { 1, 2, 1 } });
            assert_tuples_equal(expected, actual);
        }


        [TestMethod()]
        public void test_legal_actions()
        {
            var states = new[,] { { 0, 0, 0 }, { 1, 0, 0 }, { 2, 1, 0 } };
            var env_result = env._legal_actions(states);
            var expected_result = new[,] { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 1, 1 }, { 1, 2 }, { 2, 2 } };
            expected_result.AssertEquals(env_result);
        }

        [TestMethod()]
        public void test_randomint()
        {
            np.random.seed(0);
            dynamic rng = np.random.RandomState(0);
            var expected = (int)rng.randint(10000);
            dynamic rng2 = np.random.RandomState(0);
            var env = new TicTacToeSharpGenericEnvironment(rng2);
            var actual = env.randint(10000);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void test_opponent_play_deterministic()
        {
            env = new TicTacToeSharpGenericEnvironment(null);
            //Assert.Fail();
            int[,] state1 = new[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } };
            //self.env._opponent_play([[0, 0, 0], [0, 0, 0], [0, 0, 1]]))
            var result1 = env._opponent_play(state1);
            var expected1 = (0, 0);
            Assert.AreEqual(result1, expected1);

            var state2 = new[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 0 } };
            var result2 = env._opponent_play(state2);
            var expected2 = (2, 2);
            Assert.AreEqual(result2, expected2);

            var state3 = new[,] { { 1, 1, 1 }, { 1, 1, 0 }, { 1, 1, 0 } };
            var result3 = env._opponent_play(state3);
            var expected3 = (1, 2);
            Assert.AreEqual(result3, expected3);

            var state4 = new[,] { { 1, 1, 1 }, { 0, 1, 0 }, { 1, 1, 0 } };
            var result4 = env._opponent_play(state4);
            var expected4 = (1, 0);
            Assert.AreEqual(result4, expected4);
        }

        [TestMethod()]
        public void test_opponent_play_random()
        {
            using (Py.GIL())
            {
                dynamic np = Py.Import("numpy");
                dynamic rng = np.random.RandomState(0);
                var env = new TicTacToeSharpGenericEnvironment(rng);

                int[,] states = new[,] { { 0, 1, 2 }, { 0, 0, 0 }, { 0, 0, 1 } };
                var legal_actions = env._legal_actions(states);
                var actions = legal_actions.ToTupleArray();

                var s = new List<(int, int)>();
                for (var i = 0; i < 100; i++)
                {
                    var action = env._opponent_play(states);
                    s.Add(action);

                }
                Assert.IsTrue(s.All(action => actions.Contains(action)));
            }
        }

        [TestMethod()]
        public void test_step_win()
        {
            env._set_state(new int[,] { { 2, 2, 0 }, { 0, 1, 1 }, { 0, 0, 0 } }, "MID", 0, 1);
            var current_time_step = env.current_time_step();
            Assert.AreEqual(current_time_step.step_type, "MID");

            var ts = env._step(new[] { 1, 0 });
            var expected = new[,] { { 2, 2, 0 }, { 1, 1, 1 }, { 0, 0, 0 } };
            expected.AssertEquals(ts.observation);

            Assert.AreEqual(StepType.LAST, ts.step_type);
            Assert.AreEqual(env.REWARD_WIN[0], ts.reward);


            ts = env._step(new int[] { 2, 0 });
            Assert.AreEqual(StepType.FIRST, ts.step_type);
            Assert.AreEqual(0f, ts.reward);
        }


        [TestMethod()]
        public void test_step_loss()
        {
            env._set_state(new int[,] { { 2, 2, 0 }, { 0, 1, 1 }, { 0, 0, 0 } }, "MID", 0, 1);
            var current_time_step = env.current_time_step();
            Assert.AreEqual(current_time_step.step_type, "MID");

            var ts = env._step(new[] { 2, 0 });
            var expected = new[,] { { 2, 2, 2 }, { 0, 1, 1 }, { 1, 0, 0 } };
            expected.AssertEquals(ts.observation);

            Assert.AreEqual(StepType.LAST, ts.step_type);
            Assert.AreEqual(env.REWARD_LOSS[0], ts.reward);


            ts = env._step(new int[] { 2, 0 });
            Assert.AreEqual(StepType.FIRST, ts.step_type);
            Assert.AreEqual(0f, ts.reward);
        }

        [TestMethod()]
        public void test_rantest_step_illegal_move()
        {
            env._set_state(new int[,] { { 2, 2, 0 }, { 0, 1, 1 }, { 0, 0, 0 } }, "MID", 0, 1);
            var current_time_step = env.current_time_step();
            Assert.AreEqual(current_time_step.step_type, "MID");

            // Taking an illegal move.
            var ts = env._step(new[] { 0, 0 });

            var expected = new int[,] { { 2, 2, 0 }, { 0, 1, 1 }, { 0, 0, 0 } };
            var expected2d = expected.Flatten();
            var ts2d = ts.observation.Flatten();
            var equal = expected2d.SequenceEqual(ts2d);
            Assert.IsTrue(equal);

            Assert.AreEqual(StepType.LAST, ts.step_type);
            Assert.AreEqual(env.REWARD_ILLEGAL_MOVE[0], ts.reward);


            // Reset if an action is taken after final state is reached.
            ts = env._step(new int[] { 2, 0 });
            Assert.AreEqual(StepType.FIRST, ts.step_type);
            Assert.AreEqual(0f, ts.reward);

        }

        void assert_tuples_equal(ITuple expected, ITuple actual)
        {
            var a = expected.ToArray();
            var b = actual.ToArray();
            Assert.IsTrue(a.SequenceEqual(b));
        }
    }
}