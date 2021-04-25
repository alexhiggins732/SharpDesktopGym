using MousepadSimulator;
using MousepadSimulator.Directional;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseSimulatorGui
{
    public partial class DirectionalMousePadSimulator : Form
    {
        MousepadState MousepadState => processor.state;
        OutputProcessor processor = new OutputProcessor();
        public DirectionalMousePadSimulator()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            BindMousePadControls();
            InitScreen();
        }

        private void InitScreen()
        {

            MousepadState.Bounds = new MousepadSimulator.Rectangle
            {
                Height = pbScreen.Height,
                Width = pbScreen.Width,
                X = 0,
                Y = 0
            };

            var bmp = new Bitmap(MousepadState.Bounds.Width, MousepadState.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            pbScreen.Image = bmp;
        }

        private void BindMousePadControls()
        {
            var buttons = new[]
            {
              btnTouch, btnDown, btnLeft, btnRight, btnUp, btnLeftClick, btnRightClick, btnMiddleClick
            };
            foreach (var button in buttons)
            {
                button.Click += MousepadButton_Click;
            }
        }

        public string ActionSpec()
        {
            var actionSpec = new BoundedArraySpec<float>("action_space", new[] { 8, 1 }, 0, 1);
            var json = JsonConvert.SerializeObject(actionSpec);
            return json;
        }

        public string RenderDynamic(float[] rawOutputs)
        {
            var output = new UnnormalizedMousepadOutput(rawOutputs);
            var normalized = MouseOutputNormalizer.NormalizeOutput(output);
            //processOutput(normalized);
            var result = $"Recieved: {string.Join(",", rawOutputs)}";
            return result;
        }
        public void Render(float[] rawOutputs)
        {
            Console.WriteLine($"Recieved: {string.Join(",", rawOutputs)}");
            System.Diagnostics.Debug.WriteLine($"Recieved: {string.Join(",", rawOutputs)}");
            var output = new UnnormalizedMousepadOutput(rawOutputs);
            var normalized = MouseOutputNormalizer.NormalizeOutput(output);
            ProcessOutput(normalized);
        }

        public void ProcessOutput(NormalizedMousepadOutput normalizedOutput)
        {
            processor.Process(normalizedOutput);
            UpdateState();
            Application.DoEvents();
        }

        private void MousepadButton_Click(object sender, EventArgs e)
        {
            var ctl = sender as Button;
            var ouput = new NormalizedMousepadOutput();
            switch (ctl.Text)
            {
                case "Touch":
                    ouput.ToggleTouch = true;
                    break;
                //case "Up":
                //    ouput.MoveUp = true;
                //    break;
                //case "Down":
                //    ouput.MoveDown = true;
                //    break;
                //case "Left":
                //    ouput.MoveLeft = true;
                //    break;
                //case "Right":
                //    ouput.MoveRight = true;
                //    break;
                case "L":
                    ouput.ToggleLeft = true;
                    break;
                case "M":
                    ouput.ToggleMiddle = true;
                    break;
                case "R":
                    ouput.ToggleRight = true;
                    break;
                default:
                    throw new NotImplementedException($"{ctl.Name} is not a mouse button");

            }
            ProcessOutput(ouput);


        }

        private void UpdateState()
        {
            cbLeftPressed.Checked = MousepadState.LeftKey == ButtonPressState.Pressed;
            cbRightPressed.Checked = MousepadState.RightKey == ButtonPressState.Pressed;
            cbMiddlePressed.Checked = MousepadState.MiddleKey == ButtonPressState.Pressed;
            cbTouchPressed.Checked = MousepadState.TouckKey == ButtonPressState.Pressed;
            lblX.Text = $"X: {MousepadState.PosX}";
            lblY.Text = $"Y: {MousepadState.PosY}";
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            var bmp = new Bitmap(MousepadState.Bounds.Width, MousepadState.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                var pen = Pens.Black;
                g.DrawRectangle(pen, MousepadState.PosX, MousepadState.PosY, 4, 4);
            }
            pbScreen.Image = bmp;
        }




        IAgent agent;
        private void btnToggleAgent_Click(object sender, EventArgs e)
        {
            if (btnToggleAgent.Text == "Start")
            {
                if (this.rbRandomMouseMouse.Checked)
                {
                    agent = new RandomMouseAgent(this.MousepadState, this.ProcessOutput, () => this.processor);
                }
                if (agent != null)
                {
                    btnToggleAgent.Text = "Stop";
                    agent.Start();
                }

            }
            else
            {
                btnToggleAgent.Text = "Start";
                agent.Stop();
                agent = null;
            }
        }
        internal class RandomMouseAgent : IAgent
        {


            enum MouseMoveAction
            {
                MoveUp = 0,
                MoveRight = 1,
                MoveDown = 2,
                MoveLeft = 3
            }
            public void Start()
            {
                Task.Run(() => DoWork());
            }

            bool run = false;
            bool stopped = false;
            private Action<NormalizedMousepadOutput> processOutput;

            public RandomMouseAgent(MousepadState mousepadState,
                                        Action<NormalizedMousepadOutput> processOutput,
                                        Func<OutputProcessor> processorResolver)
            {

                this.processOutput = processOutput;
                var processor = processorResolver();

                mousepadState.PosX = mousepadState.Bounds.Width / 2;
                mousepadState.PosY = mousepadState.Bounds.Height / 2;
                processor.SetState(mousepadState);
            }

            private void DoWork()
            {
                run = true;

                var output = new NormalizedMousepadOutput();
                output.ToggleTouch = true;
                processOutput(output);
                output.ToggleTouch = false;

                var rng = new Random();
                while (run)
                {
                    var speed = rng.Next(0, 8);
                    var angle = rng.Next(0, 360);
                    MouseMoveAction action = (MouseMoveAction)rng.Next(0, 4);
                    //Console.WriteLine($"[{DateTime.Now}] {action}");
                    switch (action)
                    {
                        case MouseMoveAction.MoveUp:
                            output.MoveUp = true;
                            break;
                        case MouseMoveAction.MoveRight:
                            output.MoveRight = true;
                            break;
                        case MouseMoveAction.MoveDown:
                            output.MoveDown = true;
                            break;
                        case MouseMoveAction.MoveLeft:
                            output.MoveLeft = true;
                            break;
                    }
                    processOutput(output);
                    switch (action)
                    {
                        case MouseMoveAction.MoveUp:
                            output.MoveUp = false;
                            break;
                        case MouseMoveAction.MoveRight:
                            output.MoveRight = false;
                            break;
                        case MouseMoveAction.MoveDown:
                            output.MoveDown = false;
                            break;
                        case MouseMoveAction.MoveLeft:
                            output.MoveLeft = false;
                            break;
                    }

                    System.Threading.Thread.Sleep(20);
                    Application.DoEvents();
                }
                stopped = true;
            }

            public void Stop()
            {
                run = false;
                while (!stopped)
                {
                    System.Threading.Thread.Sleep(1);
                    Application.DoEvents();
                }
            }
        }
    }

   

    internal interface IAgent
    {
        void Start();
        void Stop();
    }

    public class Spec<T>
    {
        public string type => this.GetType().Name;
        public string dtype => typeof(T).Name;
        public string name;
        public Spec(string name)
        {
            this.name = name;
        }

    }
    public class ArraySpec<T> : Spec<T>
    {

        public int[] shape;

        public ArraySpec(string name, int[] shape)
            : base(name)
        {
            this.shape = shape;
        }
    }
    public class BoundedArraySpec<T> : ArraySpec<T>
    {
        public T min;
        public T max;

        public BoundedArraySpec(string name, int[] shape, T min, T max)
            : base(name, shape)
        {
            this.min = min;
            this.max = max;
        }
    }
}
