using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymSharp
{
    public partial class WinFormViewer : Form, IEnvViewer
    {
        private int _lastSize = 0;
        private readonly ManualResetEventSlim _ready = new ManualResetEventSlim();

        /// <summary>
        ///     A delegate that creates a <see cref="WinFormEnvViewer"/> based on given parameters.
        /// </summary>
        public static IEnvironmentViewerFactoryDelegate Factory => Run;

        /// <summary>
        ///     Starts a <see cref="WinFormEnvViewer"/> in seperate thread.
        /// </summary>
        /// <param name="height">The height of the form</param>
        /// <param name="width">The width of the form</param>
        /// <param name="title">The title of the form, also mentioned in the thread name.</param>
        public static IEnvViewer Run(int width, int height, string title = null)
        {
            WinFormViewer v = null;
            using (var me = new ManualResetEventSlim())
            {
                var thread = new Thread(() => {
                    v = new WinFormViewer(width + 12, height + 12, title);
                    me.Set();
                    v.ShowDialog();
                });
                thread.Start();
                thread.Name = $"Viewer{(string.IsNullOrEmpty(title) ? "" : $"-{title}")}";

                if (!me.Wait(10_000))
                    throw new Exception("Starting viewer timed out.");
            }

            Debug.Assert(v != null, "At this point viewer shouldn't be null.");

            return v;
        }

        public WinFormViewer(int screen_width, int screen_height, string title = null)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            this.Text = title ?? "GymSharp";
            this.Width = screen_width + 12;
            this.Height = screen_height + 12;
            this.Focus();
        }

        public void Render(SixLabors.ImageSharp.Image img)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Render(img)));
                return;
            }

            using (var ms = new MemoryStream(_lastSize))
            {
                img.SaveAsBmp(ms);
                _lastSize = (int)ms.Length;
                Clear();
                frame.Image = new Bitmap(ms);
                Application.DoEvents();
            }
        }


        /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Shown" /> event.</summary>
        /// <param name="e">A <see cref="T:System.EventArgs" /> that contains the event data. </param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _ready.Set();
        }

        public void Clear()
        {
            if (frame.Image != null)
            {
                var img = frame.Image;
                frame.Image = null;
                img.Dispose();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            frame.Image.TryDispose();
            _ready.TryDispose();
        }
    }
}
