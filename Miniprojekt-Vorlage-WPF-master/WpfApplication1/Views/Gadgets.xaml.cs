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

namespace Gadgeothek.Views
{
    /// <summary>
    /// Interaction logic for Gadgets.xaml
    /// </summary>
    public partial class Gadgets : UserControl
    {
        private List<Gadget> toRemoveGadgets = new List<Gadget>();
        LibraryAdminService service = new LibraryAdminService("http://mge5.dev.ifs.hsr.ch/");
        ViewModelGadgets gadgetModel = new ViewModelGadgets();

        public Gadgets()
        {
            InitializeComponent();
            DataContext = new ViewModelGadgets();
            
        }


        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
          

        }

        private void addNewGadget(object sender, RoutedEventArgs e)
        {

        }

        private void deleteGadget(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("U Sure?", "Kontrollfrage", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    foreach (ListViewItem eachItem in gadgetView.SelectedItems)
                    {
                        Gadget gadget = (Gadget)eachItem.Content;
                        gadgetModel.toDeleteGadget(gadget);
                    }

                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

        public void allGadgets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Gadget item in e.RemovedItems)
            {
                toRemoveGadgets.Remove(item);
            }

            foreach (Gadget item in e.AddedItems)
            {
                toRemoveGadgets.Add(item);
            }
        }
    }
}
