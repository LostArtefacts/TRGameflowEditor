using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TRGE.Core
{
    public class GameStamp
    {
        private readonly Dictionary<TRLanguage, string> _langMap;

        public string DefaultStamp
        {
            get => this[TRLanguage.English];
            set => this[TRLanguage.English] = value;
        }

        public string this[TRLanguage lang]
        { 
            get
            {
                return _langMap.ContainsKey(lang) ? _langMap[lang] : DefaultStamp;
            }
            set
            {
                _langMap[lang] = value;
            }
        }

        public GameStamp()
        {
            _langMap = new Dictionary<TRLanguage, string>();
            this[TRLanguage.English] = string.Empty;
        }

        public string Encode(TRLanguage language)
        {
            StringBuilder sb = new();
            foreach (char c in this[language])
            {
                char d = char.ToUpper(c);
                switch (d)
                {
                    case 'À':
                    case 'È':
                    case 'Ì':
                    case 'Ò':
                    case 'Ù':
                        sb.Append("$").Append(Normalise(c));
                        break;
                    case 'Á':
                    case 'É':
                    case 'Í':
                    case 'Ó':
                    case 'Ú':
                    case 'Ý':
                        sb.Append(")").Append(Normalise(c));
                        break;
                    case 'Â':
                    case 'Ê':
                    case 'Î':
                    case 'Ô':
                    case 'Û':
                        sb.Append("(").Append(Normalise(c));
                        break;
                    case 'Ä':
                    case 'Ë':
                    case 'Ï':
                    case 'Ö':
                    case 'Ü':
                    case 'Ÿ':
                        sb.Append("~").Append(Normalise(c));
                        break;
                    case 'ß':
                        sb.Append('=');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        private string Normalise(char c)
        {
            StringBuilder sb = new();
            string data = c.ToString().Normalize(NormalizationForm.FormD);
            foreach (char d in data)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(d) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(d);
                }
            }
            return sb.ToString();
        }
    }
}