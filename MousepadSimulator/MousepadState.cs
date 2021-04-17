using System;

namespace MousepadSimulator
{

    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
    public enum ButtonPressState
    {
        Depressed = 0,
        Pressed = 1
    }

    public class MousepadState
    {
        public ButtonPressState LeftKey;
        public ButtonPressState MiddleKey;
        public ButtonPressState RightKey;
        public ButtonPressState TouckKey;
        public int PosX;
        public int PosY;
        public Rectangle Bounds;
    }

    public class OutputProcessor
    {
        public MousepadState state;
        public OutputProcessor()
        {
            state = new MousepadState()
            {
                Bounds = new Rectangle()
            };
        }
        public void Process(NormalizedMousepadOutput output)
        {
            if (output.ToggleLeft)
                state.LeftKey = (ButtonPressState)(~(int)state.LeftKey & 1);
            if (output.ToggleRight)
                state.RightKey = (ButtonPressState)(~(int)state.RightKey & 1);
            if (output.ToggleMiddle)
                state.MiddleKey = (ButtonPressState)(~(int)state.MiddleKey & 1);
            if (output.ToggleTouch)
                state.TouckKey = (ButtonPressState)(~(int)state.TouckKey & 1);

            if (state.TouckKey == ButtonPressState.Pressed)
            {
                if (output.MoveDown)
                    state.PosY = Math.Min(state.PosY + 1, state.Bounds.Height - 1);
                if (output.MoveUp)
                    state.PosY = Math.Max(state.PosY - 1, state.Bounds.Y);
                if (output.MoveLeft)
                    state.PosX = Math.Max(state.PosX - 1, state.Bounds.X);
                if (output.MoveRight)
                    state.PosX = Math.Min(state.PosX + 1, state.Bounds.Width - 1);
            }
        }
    }


    public class NormalizedMousepadOutput
    {
        public bool ToggleLeft;
        public bool ToggleMiddle;
        public bool ToggleRight;
        public bool ToggleTouch;
        public bool MoveUp;
        public bool MoveDown;
        public bool MoveLeft;
        public bool MoveRight;
    }

    public class MouseOutputNormalizer
    {
        public static NormalizedMousepadOutput NormalizeOutput(UnnormalizedMousepadOutput input)
        {
            var result = new NormalizedMousepadOutput
            {
                ToggleLeft = input.ToggleLeft > .5,
                ToggleMiddle = input.ToggleMiddle > .5,
                ToggleRight = input.ToggleRight > .5,
                ToggleTouch = input.ToggleTouch > .5,
                MoveUp = input.MoveUp > .5,
                MoveDown = input.MoveDown > .5,
                MoveLeft = input.MoveLeft > .5,
                MoveRight = input.MoveRight > .5,
            };
            return result;
        }
    }

    public class UnnormalizedMousepadOutput
    {
        private float[] RawOutputs;
        public float ToggleLeft => RawOutputs[0];
        public float ToggleMiddle => RawOutputs[1];
        public float ToggleRight => RawOutputs[2];
        public float ToggleTouch => RawOutputs[3];
        public float MoveUp => RawOutputs[4];
        public float MoveDown => RawOutputs[5];
        public float MoveLeft => RawOutputs[6];
        public float MoveRight => RawOutputs[7];
   
        public UnnormalizedMousepadOutput(float[] outputs): this()
        {
            if (outputs.Length != 8)
            {
                throw new Exception("Ouputs should be of lenght 8");
            }
            Array.Copy(outputs, RawOutputs, outputs.Length);
        }

        public UnnormalizedMousepadOutput()
        {
            this.RawOutputs = new float[8];
        }
    }
    //FC: out 8
    //{

    //}
}
