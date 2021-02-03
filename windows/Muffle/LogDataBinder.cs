using System.ComponentModel;
using System.Windows.Forms;

namespace Muffle
{
    public class LogDataBinder : IBindLogData
    {
        public BindingSource Source { get; }

        public LogDataBinder()
        {
            Source = new BindingSource();
            Source.ListChanged += SourceOnListChanged;
        }

        private void SourceOnListChanged(object sender, ListChangedEventArgs e)
        {
            if(Source.Count >= 25)
                Source.RemoveAt(0);
        }
    }
}