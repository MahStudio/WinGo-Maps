using System.Collections.Generic;
using System.Linq;

namespace GoogleMapsUnofficial.Helpers.LanguageConverters
{
    class PersianConvert
    {
        public Dictionary<string, string> Characters = new Dictionary<string, string>()
        {
            { "ب", "b" },
            { "د","d" },
            { "ج", "dʒ" },
            { "ف", "f" },
            { "گ", "ɡ" },
            { "غ", "q" },
            { "ق", "q" },
            { "ه", "h" },
            { "ح", "h" },
            { "ی", "j" },
            { "ک", "k" },
            { "ل", "l" },
            { "م", "m" },
            { "ن", "n" },
            { "پ", "p" },
            { "ر", "r" },
            { "س", "s" },
            { "ص", "s" },
            { "ث", "s" },
            { "ش", "ʃ" },
            { "ت", "t" },
            { "ط", "t" },
            { "چ", "tʃ" },
            { "و", "v" },
            { "خ", "x" },
            { "ز", "z" },
            { "ض", "z" },
            { "ظ", "z" },
            { "ذ", "z" },
            { "ژ", "ʒ" },
            { "ع", "ʔ" },
            { "ء", "ʔ" },
            { "ا", "a" }
        };
        public string Converted(string persiantext)
        {
            var en = Characters.ToList();
            foreach (var item in en)
            {
                persiantext = persiantext.Replace(item.Key, item.Value);
            }
            return persiantext;
        }
    }
}
