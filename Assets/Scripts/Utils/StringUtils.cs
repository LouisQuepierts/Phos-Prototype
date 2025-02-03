using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Phos.Utils {
    public static class StringUtils {
        public static string EnhancedReplace(string text, Dictionary<string, string> mapping) {
            return Regex.Replace(text, @"#([A-Z_]+)#", match => {
                string key = match.Groups[1].Value;
                return mapping.GetValueOrDefault(key, match.Value);
            });
        }
    }
}