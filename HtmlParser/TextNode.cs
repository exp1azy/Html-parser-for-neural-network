using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class TextNode : NodeBase
    {
        public string Text { get; set; }
        public readonly float[] Features = new float[128];
    }
}
