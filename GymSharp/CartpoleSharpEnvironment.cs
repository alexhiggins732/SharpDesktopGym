using Python.Runtime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace GymSharp
{
    public class CartpoleSharpEnvironment : SharpGenericEnvironment<float[], int>, IDisposable
    {

        private const float gravity = 9.8f;
        private const float masscart = 1.0f;
        private const float masspole = 0.1f;
        private const float total_mass = masspole + masscart;
        private const float length = 0.5f;
        private const float polemass_length = masspole * length;
        private const float force_mag = 10.0f;
        private const float tau = 0.02f;
        private const string kinematics_integrator = "euler";

        private const float theta_threshold_radians = (float)(12 * 2 * Math.PI / 360); // Angle at which to fail the episode   

        private const float x_threshold = 2.4f;
        private Random rnd;
        dynamic np;

        private int steps_beyond_done = -1;
        public readonly float[] bounds_low;
        public readonly float[] bounds_high;
        public CartpoleSharpEnvironment(dynamic rng = null)
        {
            this._rng = rng;
            this.bounds_low = new[] { -(x_threshold * 2), -float.MaxValue, -(theta_threshold_radians * 2), -float.MaxValue };
            this.bounds_high = new[] { x_threshold * 2, float.MaxValue, theta_threshold_radians * 2, float.MaxValue };
            var jagged = new[] { bounds_low, bounds_high };
            this.observation_spec = jagged.To2DFast();

            this.action_spec = new[] { 0, 1 };
            rnd = new Random();
            using (Py.GIL())
            {
                np = Py.Import("numpy");
                np.random.seed(0);
            }
        }

        public TimeStep<float[]> reset()
        {
            float[] state = null;
            using (Py.GIL())
            {
                var pyState = np.random.uniform(-0.05, 0.05, 4);
                state = (float[])pyState;
            }
            steps_beyond_done = -1;
            return base.reset(() => state);
            
        }

        public override TimeStep<float[]> _step(int action)
        {
            var n = DateTime.Now;
            var n2 = DateTime.Now;
            var ne = n2.Subtract(n);
            var sec = ne.TotalSeconds
            Debug.Assert(Array.IndexOf((int[])action_spec, action) > -1,
                    $"{action} ({action.GetType().Name}) invalid action for {GetType().Name} environment");
            //get the last step data
            var state = _state;
            var x = (double)state[0];
            var x_dot = (double)state[1];
            var theta = (double)state[2];
            var theta_dot = (double)state[3];

            var force = action == 1 ? force_mag : -force_mag;
            var costheta = Math.Cos(theta);
            var sintheta = Math.Sin(theta);
            var temp = (force + polemass_length * theta_dot * theta_dot * sintheta) / total_mass;
            var thetaacc = (gravity * sintheta - costheta * temp) / (length * (4.0 / 3.0 - masspole * costheta * costheta / total_mass));
            var xacc = temp - polemass_length * thetaacc * costheta / total_mass;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (kinematics_integrator == "euler")
            {
                x = x + tau * x_dot;
                x_dot = x_dot + tau * xacc;
                theta = theta + tau * theta_dot;
                theta_dot = theta_dot + tau * thetaacc;
            }
            else
            {
                // semi-implicit euler
                x_dot = x_dot + tau * xacc;
                x = x + tau * x_dot;
                theta_dot = theta_dot + tau * thetaacc;
                theta = theta + tau * theta_dot;
            }

            state = new[] { (float)x, (float)x_dot, (float)theta, (float)theta_dot };
            var done = x < -x_threshold || x > x_threshold || theta < -theta_threshold_radians || theta > theta_threshold_radians;
            float reward;
            steps++;
            string step_type = steps == 1 ? StepType.FIRST : StepType.MID;
            if (!done)
            {
                reward = 1.0f;
            }
            else if (steps_beyond_done == -1)
            {

                // Pole just fell!

                steps_beyond_done = 0;
                reward = 1.0f;
                step_type = StepType.LAST;
                reset();
            }
            else
            {
                if (steps_beyond_done == 0)
                {
                    Console.WriteLine("You are calling 'step()' even though this environment has already returned done = True. You should always call 'reset()' once you receive 'done = True' -- any further steps are undefined behavior.");
                    //todo logging: logger.warn("You are calling 'step()' even though this environment has already returned done = True. You should always call 'reset()' once you receive 'done = True' -- any further steps are undefined behavior.");
                }

                steps_beyond_done += 1;
                reward = 0.0f;
                step_type = StepType.LAST;
            }

            return base.set_time_step((step_type, reward, 1, state));
        }

        public int[] legal_actions() => (int[])this.action_spec;

        const int screen_width = 600;
        const int screen_height = 400;
        private bool doRender = false;
        public bool DoRender
        {
            get
            {
                return doRender;
            }
            set
            {
                if (doRender != value)
                {
                    doRender = value;
                    if (value)
                    {
                        if (_viewer == null)
                            lock (this)
                            {
                                //to prevent double initalization.
                                if (_viewer == null)
                                {
                                    if (_viewerFactory == null)
                                        _viewerFactory = WinFormViewer.Factory;
                                    _viewer = _viewerFactory(screen_width, screen_height, "cartpole-v1");
                                }
                            }
                    }
                    else
                    {
                        if (_viewer != null)
                        {
                            _viewer.Close();
                            _viewer = null;
                        }
                    }
                }
            }
        }
        private IEnvViewer _viewer;
        private IEnvironmentViewerFactoryDelegate _viewerFactory;
        public Image render(string mode = "human")
        {
            if (!DoRender)
                return null;
            float b, t, r, l;

            const float world_width = x_threshold * 2;
            const float scale = screen_width / world_width;
            const int carty = 300;
            const float polewidth = 10.0f;
            const float poleheight = scale * (2 * length);
            const float cartwidth = 50.0f;
            const float cartheight = 30.0f;


            if (_viewer == null)
                lock (this)
                {
                    //to prevent double initalization.
                    if (_viewer == null)
                    {
                        if (_viewerFactory == null)
                            _viewerFactory = WinFormViewer.Factory;
                        _viewer = _viewerFactory(screen_width, screen_height, "cartpole-v1");
                    }
                }

            //pole
            l = -polewidth / 2;
            r = polewidth / 2;
            t = poleheight - polewidth / 2;
            b = -polewidth / 2;
            var pole = new RectangularPolygon(-polewidth / 2, carty - poleheight, polewidth, poleheight);
            var circle = new EllipsePolygon(0, carty - polewidth / 2, polewidth / 2);

            //cart
            l = -cartwidth / 2;
            r = cartwidth / 2;
            t = cartheight / 2;
            b = -cartheight / 2;
            var axleoffset = cartheight / 4.0;
            var cart = new RectangularPolygon(-cartwidth / 2, carty - cartheight / 2, cartwidth, cartheight);
            var draw = new List<(IPath, Rgba32)>();

            var state = _state;
            if (!(state is null))
            {
                var center_x = (float)(state[0] * scale + screen_width / 2.0f);
                //no y cuz it doesnt change.
                var cbounds = circle.Bounds;
                var pivotPoint = new PointF(cbounds.X + cbounds.Width / 2f, cbounds.Y + cbounds.Height / 2f);

                draw.Add((cart.Translate(center_x, 0), Color.Black));
                draw.Add((pole.Transform(Matrix3x2.CreateRotation((float)state[2], pivotPoint)).Translate(center_x, 0), new Rgba32(204, 153, 102)));
                draw.Add((circle.Translate(center_x, 0), Color.Teal));
            }
            else
            {
                draw.Add((pole, Color.Orange));
                draw.Add((cart, Color.Black));
                draw.Add((circle, Color.Teal));
            }

            var img = new Image<Rgba32>(screen_width, screen_height);

            //line
            img.Mutate(i => i.BackgroundColor(Color.White));
            img.Mutate(i => i.Fill(Color.Black, new RectangularPolygon(new PointF(0, carty), new PointF(screen_width, carty + 1))));
            foreach (var (path, rgba32) in draw)
            {
                img.Mutate(i => i.Fill(rgba32, path));
            }

            _viewer.Render(img);
            return img;
        }

        public void Dispose()
        {
            if (_viewer != null)
            {
                _viewer.Close();
                _viewer?.Dispose();
            }
           
        }
    }
}
