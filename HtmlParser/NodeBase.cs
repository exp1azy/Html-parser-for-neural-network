using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class NodeBase
    {
        public Guid Id { get; set; }
        public NodeBase? Parent { get; set; }
        public HtmlNode HtmlSrcNode { get; set; }
    }
}
