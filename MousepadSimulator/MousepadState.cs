namespace MousepadSimulator
{

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

    public class AngularMousepadState : MousepadState
    {
        public float Angle;
        public int Speed;
    }


}
