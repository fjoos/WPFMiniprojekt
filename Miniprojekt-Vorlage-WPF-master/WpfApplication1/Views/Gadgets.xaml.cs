using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using static Gadgeothek.Administration;

namespace Gadgeothek.Views
{
    /// <summary>
    /// Interaction logic for Gadgets.xaml
    /// </summary>
    public partial class Gadgets : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        List<Gadget> toRemoveGadgets = new List<Gadget>();

        public Gadgets()
        {
            InitializeComponent();
            var service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            showGadgets(service, allGadgets);
        }

        private void allGadgets_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                allGadgets.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            allGadgets.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));

        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("U Sure?", "My App", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    deleteGadgets();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Oh well, too bad!", "My App");
                    break;
            }
        }

        private void deleteGadgets()
        {
            var service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            foreach (Gadget d in toRemoveGadgets)
            {
                service.DeleteGadget(d);

            }
            showGadgets(service, allGadgets);
        }


        private void showGadgets(LibraryAdminService la, ListView lv)
        {
            var gadgets = la.GetAllGadgets();
            List<Gadget> AllG = new List<Gadget>();
            foreach (Gadget gadget in gadgets)
            {
                AllG.Add(new Gadget() { Name = gadget.Name, Price = gadget.Price, Condition = gadget.Condition, InventoryNumber = gadget.InventoryNumber, Manufacturer = gadget.Manufacturer });
            }

            lv.ItemsSource = AllG;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(allGadgets.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Condition", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("InventoryNumber", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Manufacturer", ListSortDirection.Ascending));

        }

        private void addNewGadget(object sender, RoutedEventArgs e)
        {

            var service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            Gadget gadgetToAdd = new Gadget
            {
                Name = newGadgetName.Text,
                Price = Convert.ToDouble(newGadgetPrice.Text),
                Condition = ch.hsr.wpf.gadgeothek.domain.Condition.New,
                InventoryNumber = newGadgetInventory.Text,
                Manufacturer = newGadgetManufacturer.Text
            };
            service.AddGadget(gadgetToAdd);
            showGadgets(service, allGadgets);
            /*  
          foreach(String d in Enum.GetValues(typeof(ch.hsr.wpf.gadgeothek.domain.Condition))){
               if (d.Equals(newGadgetCondition.Text))
               {

               }
           }*/


            /*
             * 
             *  public string InventoryNumber { get; set; }
        public Condition Condition { get; set; }
        public double Price { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
             * 
             * 
             * */
        }
    }
}
