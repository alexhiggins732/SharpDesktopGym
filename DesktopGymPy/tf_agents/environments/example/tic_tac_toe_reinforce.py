# Derived from Tensforflow Reinforce Agenet Tutorial https://www.tensorflow.org/agents/tutorials/6_reinforce_tutorial


import tensorflow as tf
from tf_agents.networks import q_network
import time
import numpy as np

# change to false to use the native python environment
use_csharp_env = True
if use_csharp_env == True:
    from TicTacToeSharpEnvironmentWrapper import TicTacToeEnvironment
else:
    from tic_tac_toe_environment import TicTacToeEnvironment

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
import tf_agents.environments.wrappers as wrappers

import imageio
import IPython
import matplotlib
import matplotlib.pyplot as plt
import numpy as np
import PIL.Image

np.random.seed(0)
rng = np.random.RandomState()
env = TicTacToeEnvironment(rng)
eval_env = TicTacToeEnvironment(rng)

train_env = tf_py_environment.TFPyEnvironment(env)
eval_env = tf_py_environment.TFPyEnvironment(eval_env)

num_iterations = 100000 # @param {type:"integer"}
collect_episodes_per_iteration = 5 # @param {type:"integer"}
replay_buffer_capacity = 2000 # @param {type:"integer"}
fc_layer_params = (100,)

learning_rate = 1e-3 # @param {type:"number"}
log_interval = 5 # @param {type:"integer"}
num_eval_episodes = 100 # @param {type:"integer"}
eval_interval = 50 # @param {type:"integer"}


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

#@test {"skip": true}
def compute_avg_return(environment, policy, num_episodes=10):

  total_return = 0.0
  for _ in range(num_episodes):

    time_step = environment.reset()
    episode_return = 0.0

    while not time_step.is_last():
      action_step = policy.action(time_step)
      time_step = environment.step(action_step.action)
      episode_return += time_step.reward
    total_return += episode_return

  avg_return = total_return / num_episodes
  return avg_return.numpy()[0]


# Please also see the metrics module for standard implementations of different
# metrics.
replay_buffer = tf_uniform_replay_buffer.TFUniformReplayBuffer(data_spec=tf_agent.collect_data_spec,
    batch_size=train_env.batch_size,
    max_length=replay_buffer_capacity)

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

def show_episode(environment, policy, num_episodes):
  episode_counter = 0
  environment.reset()
  episode_actions = []
  last_time_step = None
  while episode_counter < num_episodes:
    
    time_step = environment.current_time_step()
    action_step = policy.action(time_step)
    next_time_step = environment.step(action_step.action)
    traj = trajectory.from_transition(time_step, action_step, next_time_step)

    if traj.is_boundary():
      obs = last_time_step.observation
      print("Episode {0} Reward {1}".format(episode_counter,last_time_step.reward))
      print(obs)
      episode_counter += 1
    last_time_step = environment.pyenv.current_time_step()


# This loop is so common in RL, that we provide standard implementations of
# these.  For more details see the drivers module.

#@test {"skip": true}


# (Optional) Optimize by wrapping some of the code in a graph using TF
# function.
tf_agent.train = common.function(tf_agent.train)

# Reset the train step
tf_agent.train_step_counter.assign(0)

# Evaluate the agent's policy once before training.
avg_return = compute_avg_return(eval_env, tf_agent.policy, num_eval_episodes)
returns = [avg_return]

for i in range(num_iterations):

  # Collect a few episodes using collect_policy and save to the replay buffer.
  collect_episode(train_env, tf_agent.collect_policy, collect_episodes_per_iteration)

  # Use data from the buffer and update the agent's network.
  experience = replay_buffer.gather_all()
  train_loss = tf_agent.train(experience)
  replay_buffer.clear()

  step = tf_agent.train_step_counter.numpy()

  if step % log_interval == 0:
    print('step = {0}: loss = {1}'.format(step, train_loss.loss))

  if step % eval_interval == 0:
    avg_return = compute_avg_return(eval_env, tf_agent.policy, num_eval_episodes)
    print('step = {0}: Average Return = {1}'.format(step, avg_return))
    show_episode(eval_env, tf_agent.policy, 5)
    returns.append(avg_return)

#@test {"skip": true}
steps = range(0, num_iterations + 1, eval_interval)
plt.plot(steps, returns)
plt.ylabel('Average Return')
plt.xlabel('Step')
plt.ylim(top=250)