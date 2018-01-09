using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class ExtensionMethods
{
    public static string NoHTMLString(this string HTML)
    {
        return Regex.Replace(HTML, @"<[^>]+>|&nbsp;", "").Trim();
    }
}

