using System.ComponentModel;

namespace Catalog.Wpf.ViewModel
{
    public interface IValidatable : INotifyDataErrorInfo
    {
        public bool ValidateModel();
    }
}
