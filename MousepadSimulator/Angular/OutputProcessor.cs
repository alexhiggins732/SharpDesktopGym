using System;

namespace MousepadSimulator.Angular
{
    public class OutputProcessor
    {
        public AngularMousepadState state;
        public OutputProcessor()
        {
            state = new AngularMousepadState()
            {
                Bounds = new Rectangle()
            };
        }

        PointF LastPos;
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

            state.Speed = output.Speed;
            state.Angle = output.Angle;
            if (state.TouckKey == ButtonPressState.Pressed)
            {

                int length = 1 << output.Speed;
                var current_x = state.PosX;
                var current_y = state.PosY;

                LastPos = new PointF(current_x, current_y);
                var dest = move_to(current_x, current_y, output.Angle, length);
                dest = state.Bounds.Clamp(dest);

                state.PosX = (int)dest.X;
                state.PosY = (int)dest.Y;
            }
        }

        public void SetState(AngularMousepadState state)
        {
            this.state = state;
        }

        PointF move_to(float x, float y, float degree, int length)
        {
            degree -= 90;
            if (degree < 0) degree += 360;
            var angle = DegToRad(degree);
            var x_out = x + (length * Math.Cos(angle));
            var y_out = y + (length * Math.Sin(angle));
            return new PointF(x: (float)x_out, y: (float)y_out);
        }
        float DegToRad(float degree)
        {
            var rads = degree * Math.PI / 180;
            return (float)rads;
        }

        float Angle180Degrees(float cx, float cy, float ex, float ey)
        {
            var dy = ey - cy;
            var dx = ex - cx;
            var theta = Math.Atan2(dy, dx); // range (-PI, PI]
            theta *= PI_DIV_180; // rads to degs, range (-180, 180]
                                    //if (theta < 0) theta = 360 + theta; // range [0, 360)
            return (float)theta;
        }
        float Angle360Degrees(float cx, float cy, float ex, float ey)
        {
            var theta = Angle180Degrees(cx, cy, ex, ey); // range (-180, 180]
            if (theta < 0) theta = 360 + theta; // range [0, 360)
            return theta;
        }

        const double TWO_X_PI = 2 * Math.PI;
        const double HALF_PI = Math.PI / 2;
        const double PI_DIV_180 = 180 / Math.PI;
        float GeoAngle(float cx, float cy, float ex, float ey)
        {
            var dy = ey - cy;
            var dx = ex - cx;
            var theta = Math.Atan2(dy, dx);
            if (theta < 0) theta += TWO_X_PI; //angle is now in radians

            theta -= (HALF_PI); //shift by 90deg
                                    //restore value in range 0-2pi instead of -pi/2-3pi/2
            if (theta < 0) theta += TWO_X_PI;
            if (theta < 0) theta += TWO_X_PI;
            theta = Math.Abs(TWO_X_PI - theta); //invert rotation
            theta = theta * PI_DIV_180; //convert to deg
            return (float)theta;
        }

    }


    public struct PointF
    {
        public float X;
        public float Y;
        public PointF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class UnnormalizedMousepadOutput
    {
        private float[] RawOutputs;
        public float ToggleLeft => RawOutputs[0];
        public float ToggleMiddle => RawOutputs[1];
        public float ToggleRight => RawOutputs[2];
        public float ToggleTouch => RawOutputs[3];
        public float Angle => RawOutputs[4];
        public float Speed => RawOutputs[5];

        public UnnormalizedMousepadOutput(float[] outputs) : this()
        {
            if (outputs.Length != 8)
            {
                throw new Exception("Ouputs should be of lenght 8");
            }
            Array.Copy(outputs, RawOutputs, outputs.Length);
        }

        public UnnormalizedMousepadOutput()
        {
            this.RawOutputs = new float[5];
        }
    }
    public class NormalizedMousepadOutput
    {
        public float Angle;
        public int Speed;
        public bool ToggleLeft;
        public bool ToggleMiddle;
        public bool ToggleRight;
        public bool ToggleTouch;
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
                Angle = input.Angle,
                Speed = (int)input.Speed
            };
            return result;
        }
    }
}
