# SharpDesktopGym

[SharpDesktopGym](https://github.com/alexhiggins732/SharpDesktopGym) uses [Python.NET](https://pythonnet.github.io/) to build C# environments for interaction [TensorFlow Agents](https://www.tensorflow.org/agents) written in Python.

This repository contains working samples of OpenAI Gym style Tensorflow Agent environments written using C# that can be trained by TensorFlow Agents written in Python.

## Getting started

Project Structure:
- [DesktopGymPy](src/DesktopGymPy/ "DesktopGymPy") - Contains Python code and implementations.
- [GymSharp](src/GymSharp/ "GymSharp") - Contains C# enviroments the Tensorflow Python Deep Learning Reinforcement agents will interact with.
- [GymSharp](src/GymSharpTests "GymSharpTests") - Contains C# unit tests for the C# environments.


The [Examples](src/DesktopGymPy/tf_agents/environments/example "Examples") folder contains examples environments written in Python and their equivalent Csharp enviornment wrappers, along with the python unit test for the environment.

The [GymSharp](src/GymSharp "GymSharp") is the C# class library that contains the enviornments, which implement the base environment abstract class as explained the [TensorFlow Agents Enviornment tutorial](https://www.tensorflow.org/agents/tutorials/2_environments_tutorial "TensorFlow Agents Enviornment tutorial ").

## Working Example: 
**Tic Tac Toe Enviornment:**
- [tic_tac_toe_environment.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment.py "tic_tac_toe_environment.py") contains the TensorFlow Agents Tic Tac Toe environment.
- [TicTacToeEnvironment.cs](src/GymSharp/TicTacToeSharEnvironment.cs "TicTacToeSharpEnvironment.cs") contains the CSharp implementation of the same environment.
- [TicTacToeSharpEnvironmentWrapper.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "TicTacToeSharpEnvironmentWrapper.py") contains the wrapper which for the [TicTacToeEnvironment.cs](src/GymSharp/TicTacToeSharEnvironment.cs "TicTacToeSharpEnvironment.cs") C# Tic Tac Toe environment.

Both the Python environment and the Python wrapper for the C# environment can be used as the target of Python unit tests and a training environment for TensorFlow agents.

### Implementing a custom environment: 

Using the base `SharpGenericEnvironment<TState, TAction>` (along with a Python Wrapper for your class) you can easily setup Python unit tests and train any of the TensorFlow agents to learn your custom environment.

As an example the [TicTacToeSharpGenericEnvironment](src/GymSharp/TicTacToeSharpGenericEnvironment.cs "TicTacToeSharpGenericEnvironment.cs") is provided. This generic class builds upon the [TicTacToeEnvironment](src/GymSharp/TicTacToeSharEnvironment.cs "TicTacToeSharpEnvironment.cs") provided as a concrete example by using the base generic class to show how a custom environment can be setup implementing the base environment class for both unit tests and as target of TensorFlow agent training.

### Unit Tests: 
There are unit tests written in both Python and C#. The python unit tests are in the same folder as the environment module. The CSharp unit tests are in the [GymSharp](src/GymSharpTests "GymSharpTests") class.
- [tic_tac_toe_environment_test.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "tic_tac_toe_environment_test.py") contains the TensorFlow Agents unit tests for the Tic Tac Toe Enviornment.
- [TicTacToeSharpEnvironmentTests.cs](src/GymSharpTests/TicTacToeSharpEnvironmentTests.cs") contains the TensorFlow Agents unit tests for the Tic Tac Toe Enviornment.

### Development:
The recommended strategy is to take a native python environment and create a Python wrapper for it. The wrapper simply uses the Python clr module to call the corresponding methods of the C# library for the environment.

After implementing the enviornment in C#, run the Python unit tests against the Python wrapper to assure you implementation is working correctly. As you go, port the Python unit test to a C# unit test.

For Example see [TicTacToeSharpEnvironmentWrapper.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_environment_test.py "TicTacToeSharpEnvironmentWrapper.py").

Note: The C# unit tests should function by themselves without the need to be called by the Python unit tests. For example, see how Python.Net was used for Numpy random number generation which in the Python unit test is passed to the C# enviornment.

## Training:
Once your environment is passing all the unit tests, it is time to pick a TensorFlow Agent and being training. Training is simply a matter of creating an instance of your environment and passing it to the agent to train against.

For Example see the [tic_tac_toe_reinforce.py](src/DesktopGymPy/tf_agents/environments/example/tic_tac_toe_reinforce.py "tic_tac_toe_reinforce.py") Reinforce agent, modeled from the original [TensorFlow REINFORCE tutorial](https://www.tensorflow.org/agents/tutorials/6_reinforce_tutorial "TensorFlow REINFORCE tutorial"). In theory, this agent should work any custom environment you implement.

## Roadmap:

The end goal of this project is to provide a fundamental building block needed to bring the dawn of General Artificial Intellenge. To that end, the goal is to provide the [full computer desktop experience](https://github.com/alexhiggins732/CsharpMouseKeyboardDesktopLibrary) to AI and Machine learning algorithmns using an abstraction that allows them to interact with the entire internet in the way a human would.

To achive this goal the following milestones have been set:

- Additional environments
- Additional agents
- Virtual mouse driver environment (see feature branches).
- Virtual keyboard training environment (see feature branches).
- Implement a C# windows form application for viewing agent training and replays. ([Desktop recording already coded](https://github.com/alexhiggins732/CsharpMouseKeyboardDesktopLibrary))
- Implement pre-existing [Video recording and replay](https://github.com/alexhiggins732/CsharpMouseKeyboardDesktopLibrary)
- Implement a replacement for the `pythonvirtualdisplay` to allow OpenAi environments to be visualized on Windows.
- Virtual PC desktop training environment (likely using Ubuntu running on Docker as working code for this milestone is already available [docker-ubuntu-vnc-desktop](https://github.com/alexhiggins732/docker-ubuntu-vnc-desktop).)
- C# virtual stack machine environment for agent training (Ground work already coded in [MachineLearningVirtualStackEngine](https://github.com/alexhiggins732/MachineLearningVirtualStackEngine) along with an an updated OpCode Debugger for the machine (see [DILE-MSIL-DEBUGGER](https://github.com/alexhiggins732/DILE-MSIL-DEBUGGER))
- Provide REAL LOGIC based (not just math-based statistical classification) machine learning using genetic programming and fuzzy logic (as presented in [AForge.NET](https://github.com/alexhiggins732/AForge.NET)) that runs on AI opcode ran in a virtual stack machine.





