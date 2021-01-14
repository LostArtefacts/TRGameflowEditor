using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TR23LevelManager : AbstractTRLevelManager
    {
        private readonly TR23Script _script;
        private List<TR23Level> _levels;
        internal TR23LevelManager(TR23Script script)
        {
            _script = script;
            Levels = _script.Levels;
        }

        internal override List<AbstractTRLevel> Levels
        {
            get => _levels.Cast<AbstractTRLevel>().ToList();
            set
            {
                _levels = value.Cast<TR23Level>().ToList();
            }
        }

        internal override int LevelCount => _levels.Count;

        internal override void Save()
        {
            throw new NotImplementedException();
        }
    }
}