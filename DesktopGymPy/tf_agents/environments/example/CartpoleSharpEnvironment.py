
import tensorflow as tf
from tf_agents.environments import py_environment
from tf_agents.specs import BoundedArraySpec
from tf_agents.trajectories.time_step import StepType
from tf_agents.trajectories.time_step import TimeStep
import numpy as np

import os,sys,inspect
currentdir = os.path.dirname(os.path.abspath(inspect.getfile(inspect.currentframe())))
parentdir = os.path.dirname(currentdir)
sys.path.insert(0,parentdir) 
parentdir = os.path.dirname(parentdir)
sys.path.insert(0,parentdir)
parentdir = os.path.dirname(parentdir)
sys.path.insert(0,parentdir) 

import netArrays
import sys
import clr

from System import IO
clr.AddReference("System")
clr.AddReference("System.IO")

gym_sharp_path = IO.Path.GetFullPath("../GymSharp/bin/Debug")
sys.path.append(gym_sharp_path)

clr.AddReference("GymSharp")
clr.AddReference("System.Runtime.CompilerServices.Unsafe")
from GymSharp import CartpoleSharpEnvironment
from GymSharp import TupleHelper

import gym
import gym.spaces


import gym_wrapper

"""A CSharp environment for Cartpole."""
class CartpoleEnvironment(py_environment.PyEnvironment):
  """A wrapper for the C# cartpole environment.
  """


  def __init__(self, rng: np.random.RandomState=None, discount=1.0):
     super(CartpoleEnvironment, self).__init__()
     sharp_env = CartpoleSharpEnvironment(rng)

     #set the action space
     clr_action_spec = sharp_env.action_spec
     py_action_spec = netArrays.asNumpyArray(clr_action_spec)
     self.py_action_spec = BoundedArraySpec.from_array(py_action_spec)
     action_discrete = gym.spaces.Discrete(2)
     self.action_discrete_spec = gym_wrapper.spec_from_gym_space(action_discrete)

     #set the observation space
     clr_observation_spec_low = sharp_env.bounds_low
     clr_observation_spec_high = sharp_env.bounds_high
     py_observation_spec_low = netArrays.asNumpyArray(clr_observation_spec_low)
     py_observation_spec_high = netArrays.asNumpyArray(clr_observation_spec_high)
     self.py_observation_spec = BoundedArraySpec(tf.TensorShape((4,)), np.float32, minimum=py_observation_spec_low, maximum=py_observation_spec_high)
     
     observation_box = gym.spaces.Box(py_observation_spec_low, py_observation_spec_high)
     self.observation_box_spec = gym_wrapper.spec_from_gym_space(observation_box)

     self.sharp_env = sharp_env

  def enable_render(self):
      self.sharp_env.DoRender = True;
  def disable_render(self):
      self.sharp_env.DoRender = False;

  def render_screen(self):
      self.sharp_env.render();

  def action_spec(self):
    return self.action_discrete_spec #self.py_action_spec #self.sharp_env.action_spec #((2,), np.int32, minimum=0, maximum=2)

  def observation_spec(self):
    return self.observation_box_spec #self.py_observation_spec #self.sharp_env.observation_spec

  def _reset(self):
    clr_time_step = self.sharp_env.reset()
    return self.py_time_step(clr_time_step)

  def legal_actions(self):
      clr_actions = self.sharp_env.legal_actions()
      py_actions = netArrays.asNetArray(clr_actions)
      return clr_actions

  def _legal_actions(self, states: np.ndarray):
    #return list(zip(*np.where(states == 0)))
    clr_states = netArrays.asNetArray(states)
    clr_actions = self.sharp_env._legal_actions(clr_states)
    py_actions = netArrays.asNumpyTupleArray(clr_actions)
    return py_actions


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

    self.sharp_env._set_state(clr_state, str_step, reward, discount)


  def _step(self, action: np.ndarray):
    clr_action = action.item()
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


