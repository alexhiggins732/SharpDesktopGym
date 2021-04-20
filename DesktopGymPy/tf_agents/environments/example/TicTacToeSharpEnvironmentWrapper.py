
import tensorflow as tf
from tf_agents.environments import py_environment
from tf_agents.specs import BoundedArraySpec
from tf_agents.trajectories.time_step import StepType
from tf_agents.trajectories.time_step import TimeStep
import numpy as np
import netArrays
import sys
import clr

from System import IO
clr.AddReference("System")
clr.AddReference("System.IO")

gym_sharp_path = IO.Path.GetFullPath("../GymSharp/bin/Debug")
sys.path.append(gym_sharp_path)

clr.AddReference("GymSharp")
from GymSharp import TicTacToeSharpEnvironment
from GymSharp import TupleHelper

"""A CSharp environment for Tic-Tac-Toe game."""
class TicTacToeEnvironment(py_environment.PyEnvironment):
  """A state-settable environment for Tic-Tac-Toe game.
  """
  REWARD_WIN = np.asarray(1., dtype=np.float32)
  REWARD_LOSS = np.asarray(-1., dtype=np.float32)
  REWARD_DRAW_OR_NOT_FINAL = np.asarray(0., dtype=np.float32)
  # A very small number such that it does not affect the value calculation.
  REWARD_ILLEGAL_MOVE = np.asarray(-.15, dtype=np.float32)

  REWARD_WIN.setflags(write=False)
  REWARD_LOSS.setflags(write=False)
  REWARD_DRAW_OR_NOT_FINAL.setflags(write=False)

  def __init__(self, rng: np.random.RandomState=None, discount=1.0):
     super(TicTacToeEnvironment, self).__init__()
     sharp_env = TicTacToeSharpEnvironment(rng, discount)
     sharp_env.action_spec = BoundedArraySpec(tf.TensorShape((2,)), np.int32, minimum=0, maximum=2)
     sharp_env.observation_spec = BoundedArraySpec(tf.TensorShape((3, 3)), np.int32, minimum=0, maximum=2)
     py_state = np.zeros((3, 3), np.int32)
     clr_state = netArrays.asNetArray(py_state)
     sharp_env._states = clr_state

     self.sharp_env = sharp_env

  def render_screen(self):
      print(self.current_time_step())

  def action_spec(self):
    return self.sharp_env.action_spec #((2,), np.int32, minimum=0, maximum=2)

  def observation_spec(self):
    return self.sharp_env.observation_spec

  def _reset(self):
    clr_time_step = self.sharp_env.reset()
    return self.py_time_step(clr_time_step)

  def legal_actions(self):
      clr_actions = self.sharp_env.legal_actions()
      py_actions = netArrays.asNumpyTupleArray(clr_actions)
      return py_actions

  def _legal_actions(self, states: np.ndarray):
    #return list(zip(*np.where(states == 0)))
    clr_states = netArrays.asNetArray(states)
    clr_actions = self.sharp_env._legal_actions(clr_states)
    py_actions = netArrays.asNumpyTupleArray(clr_actions)
    return py_actions

  def _opponent_play(self, states: np.ndarray):
      clr_states = netArrays.asNetArray(states)
      clr_action = self.sharp_env._opponent_play(clr_states)
      py_tuple = netArrays.asPyTuple(clr_action)
      return py_tuple

  def get_state(self) -> TimeStep:
    # Returning an unmodifiable copy of the state.
    clr_current_time_step = self.sharp_env._current_time_step
    return copy.deepcopy(clr_current_time_step)

  def set_state(self, time_step: TimeStep):
    #self.sharp_env._current_time_step = time_step
    #self.sharp_env._states = time_step.observation

    step_type = time_step.step_type
    str_step = ''
    if step_type == 0:
        str_step = 'FIRST'
    elif step_type == 1:
        str_step = 'MID'
    elif step_type == 2:
        str_step = 'LAST'

    reward = float(time_step.reward)
    discount = float(time_step.discount)
    obs = time_step.observation
    clr_state = netArrays.asNetArray(obs)

    self.sharp_env._set_state(clr_state, str_step, reward, discount )


  def _step(self, action: np.ndarray):
    clr_action = netArrays.asNetArray(action)
    clr_time_step = self.sharp_env._step(clr_action)
    return self.py_time_step(clr_time_step)

  def current_time_step(self):
    clr_time_step = self.sharp_env.current_time_step()
    return self.py_time_step(clr_time_step)


  def py_time_step(self, clr_time_step):
    (step_type, reward, discount, clr_state) = (clr_time_step.step_type, clr_time_step.reward, clr_time_step.discount, clr_time_step.observation)
    py_state = netArrays.asNumpyArray(clr_state)

    py_step_type = None
    if step_type == "MID":
        py_step_type = StepType.MID
    elif step_type == "FIRST":
        py_step_type = StepType.FIRST
    elif step_type == "LAST":
        py_step_type = StepType.LAST
    return TimeStep(py_step_type, np.asarray(reward, dtype=np.float32),
                    np.asarray(discount, dtype=np.float32), py_state)

  def randint(self, bound):
      return self.sharp_env.randint(bound)

  def _check_states(self, states: np.ndarray):
    clr_states = netArrays.asNetArray(states)
    #result_dedug = self.sharp_env._check_states_debug(clr_states)
    result = self.sharp_env._check_states(clr_states)
    return (result.Item1, np.asarray(result.Item2, dtype=float))
