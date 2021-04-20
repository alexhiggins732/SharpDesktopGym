# SharpDesktopGym

SharpDesktopGym uses [Python.NET](https://pythonnet.github.io/) to build C# environments for interaction [TensorFlow Agents](https://www.tensorflow.org/agents) written in Python.

This repository contains working samples of OpenAI Gym environments written using C# that can be trained by TensorFlow Agents written in Python.

## Getting started

Project Structure:
- [DesktopGymPy](src/DesktopGymPy/ "DesktopGymPy") - Contains Python code and implementations.
- [GymSharp](src/GymSharp/ "GymSharp") - Contains C# enviroments the Tensorflow Python Deep Learning Reinforcement agents will interact with.
- [GymSharp](src/GymSharpTests "GymSharpTests") - Contains C# unit tests for the C# environments.


The [Examples](src/DesktopGymPy/tf_agents/environments/example "Examples") folder contains examples environments written in Python and their equivalent Csharp enviornment wrappers, along with the python unit test for the environment.

The [GymSharp](src/GymSharp "GymSharp") is the C# class library that contains the enviornments, which implement the base environment abstract class as explained the [TensorFlow Agents Enviornment tutorial](https://www.tensorflow.org/agents/tutorials/2_environments_tutorial "TensorFlow Agents Enviornment tutorial ").

### Example: 
**Tic Tac Toe Enviornment:**
- [tic_tac_toe_environment.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment.py "tic_tac_toe_environment.py") contains the TensorFlow Agents Tic Tac Toe environment.
- [TicTacToeSharpEnvironmentWrapper.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "TicTacToeSharpEnvironmentWrapper.py") contains the wrapper for the C# Tic Tac Toe environment.


### Unit Tests: 
There are unit tests written in both Python and C#. The python unit tests are in the same folder as the environment module. The CSharp unit tests are in the [GymSharp](src/GymSharpTests "GymSharpTests") class.
- [tic_tac_toe_environment_test.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "tic_tac_toe_environment_test.py") contains the TensorFlow Agents unit tests for the Tic Tac Toe Enviornment.
- [TicTacToeSharpEnvironmentTests.cs](src/GymSharpTests/TicTacToeSharpEnvironmentTests.cs") contains the TensorFlow Agents unit tests for the Tic Tac Toe Enviornment.

### Development:
The recommended strategy is to take a native python environment and create a Python wrapper for it. The wrapper simply uses the Python clr module to call the corresponding methods of the C# library for the environment.

After implementing the enviornment in C#, run the Python unit tests against the Python wrapper to assure you implementation is working correctly. As you go, port the Python unit test to a C# unit test.

For Example see [TicTacToeSharpEnvironmentWrapper.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "TicTacToeSharpEnvironmentWrapper.py").

Note: The C# unit tests should function by themselves without the need to be called by the Python unit tests. For example, see how Python.Net was used for Numpy random number generation which in the Python unit test is passed to the C# enviornment.

### Training:
Once your environment is passing all the unit tests, it is time to pick a TensorFlow Agent and being training. Training is simply a matter of creating an instance of your environment and passing it to the agent to train against.

For Example see the [tic_tac_toe_reinforce.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_reinforce.py "tic_tac_toe_reinforce.py") Reinforce agent, modeled from the original [TensorFlow REINFORCE tutorial](https://www.tensorflow.org/agents/tutorials/6_reinforce_tutorial "TensorFlow REINFORCE tutorial"). In theory, this agent should work any custom environment you implement.

### Roadmap:

- Implement additional environments
- Implement additional agents
- Implement a C# windows form application for veiwing agent training and replays.
- Implement a replacement for the pythonvirtualdisplay to allow OpenAi ports to be visualized on Windows platforms.


