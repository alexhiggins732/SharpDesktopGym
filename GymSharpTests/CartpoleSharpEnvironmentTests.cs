using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace GymSharp.Tests
{
    [TestClass()]
    public class CartpoleSharpEnvironmentTests
    {
        private dynamic np;
        CartpoleSharpEnvironment env;

        public CartpoleSharpEnvironmentTests()
        {
            GymSharp.PythonInitiliazer.InitializePython();
        }

        [TestInitialize]
        public void Setup()
        {
            np = null;
            using (Py.GIL())
            {
                np = Py.Import("numpy");
            }
            np.random.seed(0);
            env = new CartpoleSharpEnvironment(null);
            var ts = env.reset();
            Assert.IsTrue(ts.observation.Length == 4);
            Assert.IsTrue(ts.observation.All(x => x >= -.05 && x <= .5));
        }

        [TestMethod()]
        public void ConsoleColorTest()
        {
            var now = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(String.Format("[{0}] Training started", now));
            Console.ResetColor();
        }

        [TestMethod()]
        public void TestRender()
        {
            var ts = env.reset();
            env.DoRender = true;
            ts = env._step(1);
            var result = env.render();
            Assert.IsNotNull(result);
            while (!ts.is_Last())
            {
                ts = env._step(1);
                result = env.render();
                Assert.IsNotNull(result);
            }
            env.DoRender = false;
            result = env.render();
            Assert.IsNull(result);
        }


        [TestMethod()]
        public void resetTest()
        {
            var ts = env.reset();
            Assert.IsTrue(ts.observation.Length == 4);
            Assert.IsTrue(ts.observation.All(x => x >= -.05 && x <= .5));
            Assert.IsTrue(ts.is_First());
        }

        [TestMethod()]
        public void _stepTest()
        {
            var ts = env.reset();
            Assert.IsTrue(ts.observation.Length == 4);
            Assert.IsTrue(ts.observation.All(x => x >= -.05 && x <= .5));
            Assert.IsTrue(ts.is_First());
            ts = env._step(0);
            Assert.IsTrue(ts.observation.Length == 4);

            var bounds = (float[,])env.observation_spec;
            for (var i = 0; i < 4; i++)
            {
                var obs = ts.observation[i];
                var min = bounds[0, i];
                var max = bounds[0, i];
                Assert.IsTrue(obs >= min && obs <= max);
            }
            Assert.IsTrue(ts.is_Mid());
        }

        [TestMethod()]
        public void RandomPlayTest()
        {
            var ts = env.reset();
            Assert.IsTrue(ts.observation.Length == 4);
            Assert.IsTrue(ts.observation.All(x => x >= -.05 && x <= .5));
            Assert.IsTrue(ts.is_First());
            ts = env._step(0);


            var bounds = (float[,])env.observation_spec;
            Action checkBounds = () =>
            {
                Assert.IsTrue(ts.observation.Length == 4);
                for (var i = 0; i < 4; i++)
                {
                    var obs = ts.observation[i];
                    var min = bounds[0, i];
                    var max = bounds[1, i];
                    if (!ts.is_Last())
                        Assert.IsTrue(obs >= min && obs <= max);
                }
            };
            checkBounds();

            while (!ts.is_Last())
            {
                ts = env._step(0);
                checkBounds();
                Assert.IsTrue(env.StepCount < 100);
            }
        }
    }
}