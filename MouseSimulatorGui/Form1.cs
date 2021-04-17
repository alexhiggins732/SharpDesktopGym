using MousepadSimulator;
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
    public partial class Form1 : Form
    {
        MousepadSimulator.MousepadState MousepadState => processor.state;
        MousepadSimulator.OutputProcessor processor = new MousepadSimulator.OutputProcessor();
        public Form1()
        {
            InitializeComponent();

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
            processOutput(normalized);
        }

        private void processOutput(NormalizedMousepadOutput normalizedOutput)
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
                case "Up":
                    ouput.MoveUp = true;
                    break;
                case "Down":
                    ouput.MoveDown = true;
                    break;
                case "Left":
                    ouput.MoveLeft = true;
                    break;
                case "Right":
                    ouput.MoveRight = true;
                    break;
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
            processOutput(ouput);


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
