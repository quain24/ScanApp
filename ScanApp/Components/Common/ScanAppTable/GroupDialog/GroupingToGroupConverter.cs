using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.GroupDialog
{
    public static class GroupingToGroupConverter
    {
        public static List<Group<T>> Convert<T>(List<IGrouping<object, T>> grouping)
        {
            var groupList = new List<Group<T>>();
            foreach (var group in grouping)
            {
                groupList.Add(new Group<T>(group.Key.ToString(), group.ToList()));
            }

            return groupList;
        }
    }
}
