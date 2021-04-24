# Derived from Tensforflow Reinforce Agenet Tutorial
# https://www.tensorflow.org/agents/tutorials/6_reinforce_tutorial
import clr
import System
from System import IO
from System import String
from System import DateTime
from System import Console
from System import ConsoleColor


default_color = Console.ForegroundColor
now = DateTime.Now
Console.ForegroundColor = ConsoleColor.White

Console.ResetColor()
Console.ForegroundColor = ConsoleColor.Yellow
Console.WriteLine(String.Format("[{0}] Training started", now))
Console.WriteLine(String.Format("[{0}] Loading TensorFlow", now))
Console.ResetColor()
Console.ForegroundColor = ConsoleColor.DarkGray

import tensorflow as tf
from tf_agents.networks import q_network
import time
import signal
import sys

# change to false to use the native python environment
from CartpoleSharpEnvironment import CartpoleEnvironment


#from tf_agents.environments.examples.masked_cartpole import MaskedCartPoleEnv
from tf_agents.agents.reinforce import reinforce_agent
from tf_agents.drivers import dynamic_step_driver
from tf_agents.environments import suite_gym
from tf_agents.environments import tf_py_environment
from tf_agents.eval import metric_utils
from tf_agents.metrics import tf_metrics
from tf_agents.networks import actor_distribution_network
from tf_agents.replay_buffers import tf_uniform_replay_buffer
from tf_agents.trajectories import trajectory
from tf_agents.utils import common
from tf_agents.policies import policy_saver
import tf_agents.environments.wrappers as wrappers
from tf_agents.policies import py_tf_eager_policy

import os
import imageio
import IPython
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import PIL.Image



def signal_handler(sig, frame):
    Console.ForegroundColor = default_color
    sys.exit(0)

signal.signal(signal.SIGINT, signal_handler)


np.random.seed(0)
rng = np.random.RandomState()
env = CartpoleEnvironment(rng)
eval_env = CartpoleEnvironment(rng)

train_env = tf_py_environment.TFPyEnvironment(env)
eval_env = tf_py_environment.TFPyEnvironment(eval_env)
train_env.reset()

train_obs = train_env.observation_spec()

num_iterations = 100000 # @param {type:"integer"}
collect_episodes_per_iteration = 5 # @param {type:"integer"}
replay_buffer_capacity = 2000 # @param {type:"integer"}
fc_layer_params = (100,)

learning_rate = 1e-3 # @param {type:"number"}
log_interval = 5 # @param {type:"integer"}
num_eval_episodes = 10 # @param {type:"integer"}
eval_interval = 50 # @param {type:"integer"}
max_reward = 1000


#creating the directory causes a 'SavedModel file does not exist' exception
#IO.Directory.CreateDirectory(policy_dir)

#this does not work on windows
#tempdir = os.getenv("TEST_TMPDIR", tempfile.gettempdir())
actor_net = actor_distribution_network.ActorDistributionNetwork(train_env.observation_spec(),
    train_env.action_spec(),
    fc_layer_params=tf.TensorShape((100,)))

tf.compat.v1.enable_v2_behavior()

optimizer = tf.compat.v1.train.AdamOptimizer(learning_rate=learning_rate)

train_step_counter = tf.compat.v2.Variable(0)


tf_agent = reinforce_agent.ReinforceAgent(train_env.time_step_spec(),
    train_env.action_spec(),
    actor_network=actor_net,
    optimizer=optimizer,
    normalize_returns=True,
    train_step_counter=train_step_counter)
tf_agent.initialize()


eval_policy = tf_agent.policy
collect_policy = tf_agent.collect_policy

# Please also see the metrics module for standard implementations of different
# metrics.
replay_buffer = tf_uniform_replay_buffer.TFUniformReplayBuffer(data_spec=tf_agent.collect_data_spec,
    batch_size=train_env.batch_size,
    max_length=replay_buffer_capacity)




# (Optional) Optimize by wrapping some of the code in a graph using TF
# function.
tf_agent.train = common.function(tf_agent.train)

# Reset the train step
tf_agent.train_step_counter.assign(0)



#@test {"skip": true}
def compute_avg_return(environment, policy, num_episodes=10):

  total_return = 0.0
  py_env = environment.pyenv.envs[0]
  py_env.enable_render()
  avg_now = DateTime.Now
  color_prev = Console.ForegroundColor
  Console.ForegroundColor = ConsoleColor.DarkGreen
  for i in range(num_episodes):

    time_step = environment.reset()
    episode_return = 0.0

    while not time_step.is_last() and episode_return < max_reward:
      action_step = policy.action(time_step)
      time_step = environment.step(action_step.action)
      episode_return += time_step.reward
      py_env.render_screen()

    avg_next= DateTime.Now
    avg_elapsed = avg_next.Subtract(avg_now)
    secs = avg_elapsed.TotalSeconds;
    fps = episode_return / secs;
    avg_now = avg_next.Now
    print("[{0}] Episode {1} Reward {2} ({3}) (fps: {4})".format(avg_next, i, episode_return, avg_elapsed, fps))
    total_return += episode_return

  Console.ForegroundColor = color_prev
  avg_return = total_return / num_episodes
  py_env.disable_render()
  return avg_return.numpy()[0]




#@test {"skip": true}
def collect_episode(environment, policy, num_episodes):

  episode_counter = 0
  environment.reset()

  while episode_counter < num_episodes:
    time_step = environment.current_time_step()
    action_step = policy.action(time_step)
    next_time_step = environment.step(action_step.action)
    traj = trajectory.from_transition(time_step, action_step, next_time_step)

    # Add trajectory to the replay buffer
    replay_buffer.add_batch(traj)

    if traj.is_boundary():
      episode_counter += 1


# Evaluate the agent's policy once before training.
color_prev = Console.ForegroundColor
Console.ForegroundColor = ConsoleColor.DarkGreen
avg_return = compute_avg_return(eval_env, tf_agent.policy, num_eval_episodes)
Console.ResetColor()
returns = [avg_return]

#train for num_iterations
now = DateTime.Now

for i in range(num_iterations):

  # Collect a few episodes using collect_policy and save to the replay buffer.
  Console.ForegroundColor = ConsoleColor.Cyan
  collect_episode(train_env, tf_agent.collect_policy, collect_episodes_per_iteration)
  Console.ResetColor()

  # Use data from the buffer and update the agent's network.
  Console.ResetColor()
  Console.ForegroundColor = ConsoleColor.DarkGray

  experience = replay_buffer.gather_all()
  train_loss = tf_agent.train(experience)
  replay_buffer.clear()

  Console.ResetColor()

  step = tf_agent.train_step_counter.numpy()
  if step % log_interval == 0:
    now2 = DateTime.Now
    elapsed = now2.Subtract(now)
    secs_per_episode = collect_episodes_per_iteration / elapsed.TotalSeconds
    Console.ForegroundColor = ConsoleColor.Cyan
    print('[{0}] step = {1}: loss = {2} ({3}) (eps: {4})'.format(DateTime.Now, step, train_loss.loss, elapsed,secs_per_episode))
    now = DateTime.Now
    Console.ResetColor()

  if (step< 400 and step % eval_interval == 0) or (step >= 400 and step % log_interval == 0):
    compute_start = DateTime.Now
    color_prev = Console.ForegroundColor
    Console.ForegroundColor = ConsoleColor.DarkGreen
    avg_return = compute_avg_return(eval_env, tf_agent.policy, num_eval_episodes)

    compute_end = DateTime.Now
    compute_elapsed = compute_end.Subtract(compute_start)
    Console.ForegroundColor = color_prev
    print('[{0}] Step = Average Return = {1} {2}'.format(DateTime.Now, avg_return, compute_elapsed))
   

    now = DateTime.Now
    #show_episode(eval_env, tf_agent.policy, 5)
    returns.append(avg_return)
    if avg_return >= max_reward:
        print('Environment has been mastered')
        break

Console.ResetColor()


def play_infinite_loop(environment, policy):

  episode_counter = 1
  environment.reset()
  py_env = environment.pyenv.envs[0]
  py_env.enable_render()
  episode_actions = []
  last_time_step = None
  episode_return = 0.0
  max_episode_reward = 0.0
  episode_start = DateTime.Now
  while episode_counter > 0:
    
    time_step = environment.current_time_step()
    action_step = policy.action(time_step)
    next_time_step = environment.step(action_step.action)
    traj = trajectory.from_transition(time_step, action_step, next_time_step)
    episode_return += next_time_step.reward
    py_env.render_screen()
    
    if traj.is_boundary():
      episode_end = DateTime.Now
      episode_elapsed = episode_start.Subtract(episode_start)
      episode_seconds = episode_elapsed.TotalSeconds
      episode_fps = episode_return /episode_seconds
      obs = last_time_step.observation
      print("Episode {0} Reward {1} - Max Reward {2} ({})".format(episode_counter,episode_return, max_episode_reward,episode_elapsed))
      print("Episode {0} Reward {1} - Max Reward {2} ({})".format(episode_counter,episode_return, max_episode_reward,episode_elapsed))
      print(obs)
      episode_counter += 1
      environment.reset()
      max_episode_reward = episode_return if episode_return > max_episode_reward else max_episode_reward
      episode_return = 0

    last_time_step = environment.pyenv.current_time_step()

  py_env.disable_render()



eager_py_policy = py_tf_eager_policy.SavedModelPyTFEagerPolicy(policy_dir, eval_env.time_step_spec(), eval_env.action_spec())


play_infinite_loop(eval_env, tf_agent.policy)
