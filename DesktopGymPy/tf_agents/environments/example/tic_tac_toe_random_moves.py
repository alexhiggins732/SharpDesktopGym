import time
import numpy as np
import tensorflow as tf
from TicTacToeSharpEnvironmentWrapper import TicTacToeEnvironment

np.random.seed(0)
rng = np.random.RandomState()
env = TicTacToeEnvironment(rng)

time_step = env.reset()
rewards = []
steps = []
number_of_episodes = 10000
start = time.time()
total_start = time.time()
for i in range(number_of_episodes):

  reward_t = 0
  steps_t = 0
  env.reset()
  episode_steps=0
  episode_reward= 0.
  while True:
    legal_actions= env.legal_actions()
    idx = np.random.randint(len(legal_actions)) #tf.random.uniform([1], 0, len(legal_actions), dtype=tf.int32)
    action = legal_actions[idx];
    action_array = np.array(action, dtype=np.int32)
    next_time_step = env.step(action_array)
    episode_steps += 1
    episode_reward += next_time_step.reward
    if env.current_time_step().is_last():
      break

  rewards.append(episode_reward)
  steps.append(episode_steps)
  

  if (i % 1000 ==0):
    mean_no_of_steps = np.mean(steps)
    mean_reward = np.mean(rewards)
    end = time.time()
    elapsed = end - start
    print ("{2}: Average {0} Reward in {1} steps. Time: {3} ".format(mean_reward, mean_no_of_steps, i, elapsed))
    start= time.time();

total_end = time.time()
total_elapsed= total_end-total_start

mean_no_of_steps = np.mean(steps)
mean_reward = np.mean(rewards)
print ("Average {0} Reward in {1} steps. Time: {2} ".format(mean_reward, mean_no_of_steps, total_elapsed))
