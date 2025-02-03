using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class BaseView : INotifyPropertyChanged
    {
        /// <summary>
        /// 状態更新
        /// </summary>
        public enum Status
        {
            NONE,
            NEW,
            MOD,
        }

        /// <summary>
        /// 状態の文字列
        /// </summary>
        /// <param name="StatudData_"></param>
        /// <returns></returns>
        public string StatusString(Status StatudData_)
        {
            switch (StatudData_)
            {
                case Status.NEW: return "新規";
                case Status.MOD: return "変更";
                default:
                case Status.NONE:
                    break;
            }
            return "　　";
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
