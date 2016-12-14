using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ch.hsr.wpf.gadgeothek.domain;
using Gadgeothek.ViewModel;

namespace Gadgeothek.Views
{
    public partial class ChangeGadget : Window
    {
        private Gadget modifiedGadget;
        private ViewModelGadgets gadgetModel;
        public ChangeGadget(Gadget selectedItem, String buttonName)
        {
            this.modifiedGadget = selectedItem;
            InitializeComponent();
            gadgetModel  = new ViewModelGadgets(modifiedGadget, buttonName);
            DataContext = gadgetModel;
            
        }

        private void modifyGadget(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            if (content.Equals("Add"))
            {
                gadgetModel.addGadget(modifiedGadget);
            }
            else if (content.Equals("Save"))
            {
                gadgetModel.saveGadget(modifiedGadget);
            }
        }
    }
}
