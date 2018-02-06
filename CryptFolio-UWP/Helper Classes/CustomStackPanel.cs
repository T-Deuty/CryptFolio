using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptFolio
{
    class CustomStackPanel : StackPanel
    {
        public double UserValue { get; set; }
        public Button RemoveButton { get; set; }
    }
}
