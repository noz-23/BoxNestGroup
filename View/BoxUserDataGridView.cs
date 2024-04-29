using Box.V2.Models;
using BoxNestGroup.Manager;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.View
{
    internal class BoxUserDataGridView 
    {
        public bool Selected { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string UserMailAddress { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ListNowGroup{ get; set; } =string.Empty;
        public string ListModGroup { get; set; } = string.Empty;
        public string ListNestGroup { get; set; } = string.Empty;

        public string UserSpaceUsed { get; set; } = string.Empty;
        public string UserExternalCollaborate { get; set; } = string.Empty;

        public const string BOX_UNLIMITED = "unlimited";
        public const string APP_UNLIMITED = "無制限";

        public const string BOX_ENABLED = "enabled";
        public const string BOX_DISABLED = "disabled";

        public const string APP_ENABLED = "制　限";
        public const string APP_DISABLED = "しない";

        public BoxUserDataGridView()
        {
        }
        public BoxUserDataGridView(BoxUser user_)
        {
            UserName = user_.Name;
            UserMailAddress = user_.Login;
            UserId = user_.Id;
            var list = SettingManager.Instance.ListGroupNameInUser(UserId);
            ListNowGroup = string.Join("\n", list);

            UserExternalCollaborate = (user_.IsExternalCollabRestricted ==true) ? APP_ENABLED : APP_DISABLED;
            UserSpaceUsed = (user_.SpaceUsed == -1) ? APP_UNLIMITED : user_.SpaceUsed.ToString();

        }

        public BoxUserDataGridView(string name_, string mail_, IList<string> listGroup_, string strage_, string colabo_)
        {
            Console.WriteLine("■BoxUserDataGridView name_[{0}] mail_[{1}] listGroup_[{2}] strage_[{3}] colabo_[{4}]", name_, mail_,string.Join(",", listGroup_), strage_,colabo_);
            UserName = name_;
            UserMailAddress = mail_;

            UserSpaceUsed = (strage_.Contains(BOX_UNLIMITED) ==true) ? APP_UNLIMITED : strage_.Replace("\r",string.Empty).Replace("\n", string.Empty);
            UserExternalCollaborate = (colabo_.Contains( "disabled") ==true) ? "許　可" : "不許可";

            ListNowGroup =string.Join("\n",listGroup_);



        }
    }
}
