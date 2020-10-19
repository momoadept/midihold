using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiHolder.Models
{
    public class Feed
    {
        private Action<IList<string>> feeder;
        private IList<string> current = new List<string>();

        public Feed(Action<IList<string>> feeder)
        {
            this.feeder = feeder;
        }

        public void Set(IList<string> lines)
        {
            feeder(current = lines);
        }

        public void Say(string line)
        {
            current.Add(line);
            Set(current);
        }
    }
}
