using System;
using System.Linq;
using HtmlAgilityPack;

namespace Common
{
    public static class HtmlFormatter
    {
        public static bool IsValidHtml(string html, out string explanation)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var errors = htmlDoc.ParseErrors.ToList();
            explanation = !errors.Any()? string.Empty : string.Join($"{Environment.NewLine}", errors.Select(x => x.Reason));

            return !errors.Any();
        }
    }
}
