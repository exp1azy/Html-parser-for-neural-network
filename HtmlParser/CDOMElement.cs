using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class CDOMElement
    {
        public CDOMElement? Parent { get; set; }
        public BlockType Type { get; set; }
        public string Name { get; set; }
        public HtmlNode HtmlSrcNode { get; set; }
        public string Text { get; set; }
        public float[] Features { get; set; }
    }
}