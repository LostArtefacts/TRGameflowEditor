using System;
using System.Collections.Generic;
using System.Linq;

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

        internal void RandomiseUnarmedLevels(RandomGenerator rng, uint levelCount, List<AbstractTRLevel> originalLevels)
        {
            RandomiseLevelsWithOperation
            (
                rng, 
                levelCount, 
                originalLevels, 
                new TROperation(TR23OpDefs.RemoveWeapons, ushort.MaxValue, true)
            );
        }

        internal List<TR23Level> GetUnarmedLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveWeapons, true).Cast<TR23Level>().ToList();
        }

        internal virtual List<MutableTuple<string, string, bool>> GetUnarmedLevelData()
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (TR23Level level in _levels)
            {
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesWeapons));
            }
            return data;
        }

        internal virtual void SetUnarmedLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR23Level level = (TR23Level)GetLevel(item.Item1);
                if (level != null)
                {
                    level.RemovesWeapons = item.Item3;
                }
            }
        }

        internal override void Save()
        {
            throw new NotImplementedException();
        }

        
    }
}