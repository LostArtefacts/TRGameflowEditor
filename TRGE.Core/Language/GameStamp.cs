using System.Collections.Generic;

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
    }
}