using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI
{
    public static class MessageBuffer
    {
        [ThreadStatic]
        private static List<string>? _lines;

        public static void Begin()
        {
            _lines = new List<string>();
        }

        public static void Add(string line)
        {
            _lines?.Add(line);
        }

        public static void Commit()
        {
            if (_lines != null && _lines.Count > 0)
            {
                RenderDispatcher.Raise(new RenderActionMessageEvent(_lines));
            }
            _lines = null;
        }

        public static void Cancel()
        {
            _lines = null;
        }

        public static bool IsCollecting => _lines != null;
    }

}
