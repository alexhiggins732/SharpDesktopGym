import json
from io import StringIO
import numpy as np


assembly_path1 = r"C:\Users\alexh\source\repos\DesktopGym\src\MouseSimulatorGui\bin\Debug"
import sys
sys.path.append(assembly_path1)


import clr
clr.AddReference("MouseSimulatorGui")
clr.AddReference("MouseSimulator")

from MouseSimulatorGui import Form1
from MouseSimulatorGui import Program
from MouseSimulatorGui import DesktopEnvironment
frm = Form1()
frm.Show()
action_spec_json= frm.ActionSpec()
io = StringIO(action_spec_json)
action_spec_d = json.load(io)
print (action_spec_d)

spec_min= action_spec_d['min']
spec_max = action_spec_d['max']
spec_dtype= action_spec_d['dtype']
spec_name= action_spec_d['name']
spec_shape = action_spec_d['shape']
spec_shape = np.zeros((spec_shape[0], spec_shape[1]))
sharp_env = DesktopEnvironment()

environment = IShaprEnvironment(sharp_env)
environment.Wrap()
utils.validate_py_environment(environment, episodes=5)

num_iterations = 20000
for i in range(num_iterations):
    input= np.random.rand(1, 8)
    result = input.ravel()
    output = frm.RenderDynamic(result)
    print(str(i)+ " " + output)

replay = raw_input("Play again? ")