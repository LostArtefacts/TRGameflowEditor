using System.Collections.Generic;
using TRGE.Coord;

namespace TRGE.View.Model
{
    public class RecentFolderList : List<RecentFolder>
    {
        public RecentFolderList(IRecentFolderOpener folderOpener)
        {
            foreach (string folder in TRCoord.Instance.History)
            {
                Add(new RecentFolder(folderOpener)
                {
                    Index = Count + 1,
                    FolderPath = folder
                });
            }
        }

        public bool IsEmpty => Count == 0;
    }
}