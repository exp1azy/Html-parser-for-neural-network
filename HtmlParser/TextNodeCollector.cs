using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class TextNodeCollector
    {
        private List<TextNode> _textNodes;
        public List<TextNode> TextNodes => _textNodes;

        public void Collect(string filePath)
        {
            _textNodes = new List<TextNode>();
            var doc = new HtmlDocument();
            doc.Load(filePath);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            Rec(new NodeBase { Id = Guid.NewGuid(), HtmlSrcNode = body });
        }

        private void Rec(NodeBase parent)
        {
            foreach (var child in parent.HtmlSrcNode.ChildNodes)
            {
                if (child.Name == "#text")
                    _textNodes.Add(new TextNode
                    {
                        Id = Guid.NewGuid(),
                        Text = child.InnerText,
                        Parent = parent,
                        HtmlSrcNode = child
                    });
                else
                    Rec(new NodeBase { Id = Guid.NewGuid(), Parent = parent, HtmlSrcNode = child });
            }
        }

        public static void CalculateFeatures(TextNode textNode)
        {
            Regex regex = new Regex(@"\d");
            MatchCollection matchesForB11AndB12 = regex.Matches(textNode.Text);
            textNode.Features[10] = CalculateB11(textNode, matchesForB11AndB12);
            textNode.Features[11] = CalculateB12(textNode, matchesForB11AndB12);

            textNode.Features[15] = CalculateB16(textNode);
            textNode.Features[16] = CalculateB17(textNode);
            textNode.Features[17] = CalculateB18(textNode);
            textNode.Features[18] = CalculateB19(textNode);
        }

        private static float MatchValue(Regex regex, TextNode textNode)
        {
            MatchCollection matches = regex.Matches(textNode.Text);
            if (matches.Count > 0)
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private static float CalculateB11(TextNode textNode, MatchCollection matches) //has numeric
        {
            if (matches.Count > 0)
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private static float CalculateB12(TextNode textNode, MatchCollection matches) //numeric ratio
        {
            if(matches.Count > 0)
            {
                var ratio = (float)(matches.Count / textNode.Text.Length);
                return ratio;
            }
            else
            {
                return 0.0f;
            }
        }

        private static float CalculateB16(TextNode textNode) //contains copyright
        {
            if(textNode.Text.Contains("©"))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private static float CalculateB17(TextNode textNode) //contains email
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return MatchValue(regex, textNode);
        }

        private static float CalculateB18(TextNode textNode) //contains url
        {
            Regex regex = new Regex(@"^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
            return MatchValue(regex, textNode);
        }

        private static float CalculateB19(TextNode textNode) //contains year
        {
            Regex regex = new Regex(@"\d{4}");
            return MatchValue(regex, textNode);
        }
    }
}