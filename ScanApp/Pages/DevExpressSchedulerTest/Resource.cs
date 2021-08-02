using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.DevExpressSchedulerTest
{
    public class Resource
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string TextCss { get; set; }
        public string BackgroundCss { get; set; }
        public override bool Equals(object obj) {
            Resource resource = obj as Resource;
            return resource != null && resource.Id == Id;
        }
        public override int GetHashCode() {
            return Id;
        }
    }
}
