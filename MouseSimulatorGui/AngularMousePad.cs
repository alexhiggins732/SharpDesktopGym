using MousepadSimulator;
using MousepadSimulator.Angular;
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
using Rectangle = MousepadSimulator.Rectangle;
namespace MouseSimulatorGui
{
    public partial class AngularMousePad : Form
    {
        AngularMousepadState MousepadState => processor.state;
        OutputProcessor processor = new OutputProcessor();
        public AngularMousePad()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            BindMousePadControls();
            InitScreen();
            this.FormClosing += AngularMousePad_FormClosing;
        }

        private void AngularMousePad_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (agent != null)
            {
                agent.Stop();
                agent = null;
            }
        }

        private void InitScreen()
        {

            MousepadState.Bounds = new Rectangle(
                x: 0,
                y: 0,
                width: pbScreen.Width,
                height: pbScreen.Height
            );

            var bmp = new Bitmap(MousepadState.Bounds.Width, MousepadState.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            pbScreen.Image = bmp;
        }

        private void BindMousePadControls()
        {
            var buttons = new Control[]
            {
              btnTouch, btnLeftClick, btnRightClick, btnMiddleClick
            };
            foreach (var button in buttons)
            {
                button.Click += MousepadControl_Changed;
            }
            ddAngle.DataSource = Enumerable.Range(0, 360).ToList();
            ddSpeed.DataSource = Enumerable.Range(0, 8).ToList();
            ddAngle.SelectedIndexChanged += MousepadControl_Changed;
            ddSpeed.SelectedIndexChanged += MousepadControl_Changed;
            trSpeed.ValueChanged += MousepadControl_Changed;
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

        private void MousepadControl_Changed(object sender, EventArgs e)
        {
            var ctl = sender as Button;
            var output = new NormalizedMousepadOutput();
            output.Angle = this.MousepadState.Angle;
            output.Speed = this.MousepadState.Speed;
            if (ctl != null)
            {
                switch (ctl.Text)
                {
                    case "Touch":
                        output.ToggleTouch = true;
                        break;
                    case "L":
                        output.ToggleLeft = true;
                        break;
                    case "M":
                        output.ToggleMiddle = true;
                        break;
                    case "R":
                        output.ToggleRight = true;
                        break;
                    default:
                        throw new NotImplementedException($"{ctl.Name} is not a mouse button");

                }

            }
            else if (sender is ComboBox dd)
            {
                switch (dd.Name)
                {
                    case nameof(ddAngle):
                        output.Angle = (float)(int)ddAngle.SelectedValue;
                        break;

                }

            }
            else if (sender is TrackBar tr)
            {
                output.Speed = tr.Value;
            }

            ProcessOutput(output);


        }

        private void UpdateState()
        {
            cbLeftPressed.Checked = MousepadState.LeftKey == ButtonPressState.Pressed;
            cbRightPressed.Checked = MousepadState.RightKey == ButtonPressState.Pressed;
            cbMiddlePressed.Checked = MousepadState.MiddleKey == ButtonPressState.Pressed;
            cbTouchPressed.Checked = MousepadState.TouckKey == ButtonPressState.Pressed;
           
            lblAngle.Text = $"Angle: {MousepadState.Angle}";
            lblSpeed.Text = $"Speed: {MousepadState.Speed}";
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
            private AngularMousepadState state;
            public RandomMouseAgent(AngularMousepadState mousepadState,
                                        Action<NormalizedMousepadOutput> processOutput,
                                        Func<OutputProcessor> processorResolver)
            {

                this.processOutput = processOutput;
                var processor = processorResolver();
                this.state = mousepadState;

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
                    bool incSpeed = rng.NextDouble() > .5;

                    var speed = state.Speed;
                    if (incSpeed)
                    {
                        speed += 1;
                        if (speed > 4)
                            speed = 4;
                    }
                    else
                    {
                        speed -= 1;
                        if (speed < 0)
                            speed = 0;
                    }

                    var angle = state.Angle;
                    var incAngle = rng.NextDouble() > .5;
                    if (incAngle)
                    {
                        angle += 1;
                        if (angle > 359)
                            angle = 0;
                    }
                    else
                    {
                        angle -= 1;
                        if (angle < 0)
                            angle = 0;
                    }
                    //var speed = rng.Next(0, 8);
                    angle = rng.Next(0, 360);
                    MouseMoveAction action = (MouseMoveAction)rng.Next(0, 4);
                    //Console.WriteLine($"[{DateTime.Now}] {action}");
                    output.Angle = angle;
                    output.Speed = speed;
                    processOutput(output);

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
}
