using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.Views
{
    public class BaseView : INotifyPropertyChanged
    {
        /// <summary>
        /// 状態更新
        /// </summary>
        public enum Status
        {
            [Description("　　")]
            NONE,
            [Description("新規")]
            NEW,
            [Description("変更")]
            MOD,
        }

        /// <summary>
        /// 状態の文字列
        /// </summary>
        /// <param name="StatudData_"></param>
        /// <returns></returns>
        //https://www.sejuku.net/blog/42539
        public string StatusString(Status Status_)
        {
            var gm = Status_.GetType().GetMember(Status_.ToString());
            var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute)attributes[0]).Description;
            return description;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// プロパティの値を設定する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field_"></param>
        /// <param name="value_"></param>
        /// <param name="propertyName_"></param>
        /// <returns></returns>
        protected bool _SetValue<T>(ref T field_, T value_, [CallerMemberName] string propertyName_ ="")
        {
            if (EqualityComparer<T>.Default.Equals(field_, value_))
            {
                return false;
            }
            field_ = value_;
            _NotifyPropertyChanged(propertyName_);
            return true;
        }


    }
}
