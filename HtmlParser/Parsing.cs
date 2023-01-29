using HtmlAgilityPack;

namespace HtmlParser
{
    class Parsing
    {
        public static void Main()
        {
            string filePath = "C:\\Users\\anton\\source\\repos\\HtmlParser\\HtmlParser\\docs\\index.html";
            var doc = new HtmlDocument();

            var collector = new TextNodeCollector();
            collector.Collect(filePath);

            foreach (var textNode in collector.TextNodes)
            {
                TextNodeCollector.CalculateFeatures(textNode);              
            }                     
        }
    }
}