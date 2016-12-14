using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Gadgeothek.ViewModel;
using System.Configuration;

namespace Gadgeothek.Views
{

    public partial class Gadgets : UserControl
    {
        ViewModelGadgets gadgetModel;
        public Gadgets()
        {
            InitializeComponent();
            gadgetModel = new ViewModelGadgets();
            DataContext = gadgetModel;
        }


        private void deleteGadget(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("U Sure?", "Kontrollfrage", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    while (gadgetView.SelectedItems.Count > 0)
                    {
                        gadgetModel.toDeleteGadget((Gadget)gadgetView.SelectedItem);
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

        private void editGadget(object sender, RoutedEventArgs e)
        {
            
            string content = (sender as Button).Content.ToString();
            Window editWindow;
            if (content.Equals("Edit") && gadgetView.SelectedItems.Count == 1)
            {
                editWindow = new ChangeGadget(gadgetModel, (Gadget)gadgetView.SelectedItem, "Save");
                editWindow.ShowDialog();
            }
            else if(content.Equals("Add new Gadget"))
            {
                editWindow = new ChangeGadget(gadgetModel, new Gadget(), "Add");
                editWindow.ShowDialog();
            }

            
        }
    }
}
