import json
from io import StringIO
import numpy as np
import netArrays

from tf_agents.environments.examples.tic_tac_toe_environment import TicTacToeEnvironment

env= TicTacToeEnvironment()

assembly_path1 = r"C:\Users\alexh\source\repos\DesktopGym\src\MouseSimulatorGui\bin\Debug"
import sys
sys.path.append(assembly_path1)


import clr
clr.AddReference("MouseSimulatorGui")
clr.AddReference("MouseSimulator")
clr.AddReference("GymSharp")
from MouseSimulatorGui import Form1
from MouseSimulatorGui import Program
from MouseSimulatorGui import DesktopEnvironment
from GymSharp import TicTacToeSharpEnvironment
from GymSharp import PythonTest


#np.random.seed(0)
test_rng = np.random.RandomState(0)
test = PythonTest(test_rng);
call_result = test.TestRng(10000)


from numpy.random import RandomState
rs = RandomState(12345)


testStates = np.zeros((2, 3), np.int32)
print(testStates)
states_list = list(zip(*np.where(testStates == 0)))
states_list_np = np.array(states_list)
print(states_list_np.shape)

tic_tac_toe = TicTacToeSharpEnvironment(rs, .5)


clrStates = netArrays.asNetArray(testStates)
npClr = np.array(clrStates)
print(npClr.shape)
debug_states = tic_tac_toe._legal_actions_debug(clrStates)



legal_actions = tic_tac_toe._legal_actions(clrStates)
py_actions = netArrays.asNumpyArray(legal_actions)

native = rs.randint(len([1,2,3,4,5]))


rng_result = tic_tac_toe.randint(20)
print(rng_result)


frm = Form1()
frm.Show()
action_spec_json = frm.ActionSpec()
io = StringIO(action_spec_json)
action_spec_d = json.load(io)
print(action_spec_d)

spec_min = action_spec_d['min']
spec_max = action_spec_d['max']
spec_dtype = action_spec_d['dtype']
spec_name = action_spec_d['name']
spec_shape = action_spec_d['shape']
spec_shape = np.zeros((spec_shape[0], spec_shape[1]))
sharp_env = DesktopEnvironment()

environment = IShaprEnvironment(sharp_env)
environment.Wrap()
utils.validate_py_environment(environment, episodes=5)

num_iterations = 20000
for i in range(num_iterations):
    input = np.random.rand(1, 8)
    result = input.ravel()
    output = frm.RenderDynamic(result)
    print(str(i) + " " + output)

replay = raw_input("Play again? ")

