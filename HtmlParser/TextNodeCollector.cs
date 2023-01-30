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
        private List<CDOMElement> _textNodes;
        private List<CDOMElement> _all;
        public List<CDOMElement> TextNodes => _textNodes;

        private static string[] _tagsToSkip = { "checkbox", "head", "hr", "iframe", "img", "input" };

        public void Collect(string filePath)
        {
            _textNodes = new List<CDOMElement>();
            _all = new List<CDOMElement>();
            var doc = new HtmlDocument();
            doc.Load(filePath);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            RecursiveNodeTreeTraversal(new CDOMElement { HtmlSrcNode = body });
        }

        private void RecursiveNodeTreeTraversal(CDOMElement parent)
        {
            foreach (var child in parent.HtmlSrcNode.ChildNodes)
            {
                if (_tagsToSkip.Contains(child.Name))
                    continue;
                
                if (child.Name == "br")
                {
                    var elem = new CDOMElement
                    {
                        Type = BlockType.LineBreak,
                        Name = child.Name,
                        Parent = parent,
                        HtmlSrcNode = child
                    };
                    _textNodes.Add(elem);
                    _all.Add(elem);
                }
                else if (child.Name == "#text")
                {
                    if (!string.IsNullOrWhiteSpace(child.InnerText))
                    {
                        var str = child.InnerText.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
                        var elem = new CDOMElement
                        {
                            Type = str == string.Empty ? BlockType.LineBreak : BlockType.Text,
                            Name = child.Name,
                            Text = child.InnerText,
                            Parent = parent,
                            HtmlSrcNode = child
                        };
                        _textNodes.Add(elem);
                        _all.Add(elem);
                    }
                }                    
                else
                    RecursiveNodeTreeTraversal(new CDOMElement { Parent = parent, HtmlSrcNode = child });                    
            }
        }

        public void CollapseSingleChild()
        {
            foreach (var current in _textNodes)
            {
                while (_all.Count(n => object.ReferenceEquals(current.Parent, n.Parent)) == 1)
                {
                    current.Parent = current.Parent.Parent;
                    current.Name = $"{current.Parent.Name}/{current.Name}";
                    current.Parent.Parent = null;
                }
            }  
        }

        public static void CalculateFeatures(CDOMElement textNode)
        {
            //Regex regex = new Regex(@"\d");
            //MatchCollection matchesForB11AndB12 = regex.Matches(textNode.Text);
            //textNode.Features[10] = CalculateB11(textNode, matchesForB11AndB12);
            //textNode.Features[11] = CalculateB12(textNode, matchesForB11AndB12);

            //textNode.Features[14] = CalculateB15(textNode);
            //textNode.Features[15] = CalculateB16(textNode);
            //textNode.Features[16] = CalculateB17(textNode);
            //textNode.Features[17] = CalculateB18(textNode);
            //textNode.Features[18] = CalculateB19(textNode);
        }

        private static float MatchValue(Regex regex, CDOMElement textNode)
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

        private static float CalculateB11(CDOMElement textNode, MatchCollection matches) //has numeric
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

        private static float CalculateB12(CDOMElement textNode, MatchCollection matches) //numeric ratio
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

        private static float CalculateB15(CDOMElement textNode) //ends with question mark
        {
            if ((textNode.Text[textNode.Text.Length - 1]).Equals('?')) 
            { 
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        private static float CalculateB16(CDOMElement textNode) //contains copyright
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

        private static float CalculateB17(CDOMElement textNode) //contains email
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return MatchValue(regex, textNode);
        }

        private static float CalculateB18(CDOMElement textNode) //contains url
        {
            Regex regex = new Regex(@"^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
            return MatchValue(regex, textNode);
        }

        private static float CalculateB19(CDOMElement textNode) //contains year
        {
            Regex regex = new Regex(@"\d{4}");
            return MatchValue(regex, textNode);
        }
    }
}