using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseSimulatorGui
{
    public class DesktopEnvironment
    {
        public BoundedArraySpec<int> ActionSpec;

        public BoundedArraySpec<int> ObservationSpec { get; }

        public DesktopEnvironment()
        {
            this.ActionSpec = new BoundedArraySpec<int>("action_spec", new[] { 8, 1 }, 0, 1);
            this.ObservationSpec = new BoundedArraySpec<int>("observation_spec", new[] { 256, 256 }, 0, 1);
            _action_spec = Enumerable.Range(0,this.ActionSpec.shape[1])
                    .Select(y=> Enumerable.Range(0, ActionSpec.shape[0]).Select(x => 0).ToArray())
                    .ToArray();
            _observation_spec = Enumerable.Range(0, this.ObservationSpec.shape[1])
                    .Select(y => Enumerable.Range(0, ObservationSpec.shape[0]).Select(x => 0).ToArray())
                    .ToArray();
            _state = Enumerable.Range(0, ActionSpec.shape[1]).Select(x => 0).ToArray();
        }

        public string ActionSpecJson() => JsonConvert.SerializeObject(ActionSpec);
        public string ObservationSpectJson() => JsonConvert.SerializeObject(ObservationSpec);
        public int[][] _action_spec;
        public int[][] _observation_spec;
        public int[] _state;
        public bool _episode_ended;



    }
}
