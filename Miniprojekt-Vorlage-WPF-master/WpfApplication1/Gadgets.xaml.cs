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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using System.ComponentModel;

namespace Gadgeothek
{

    public partial class Gadgets : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        List<Gadget> toRemoveGadgets = new List<Gadget>();

        public Gadgets()
        {
            InitializeComponent();

            var service = new LibraryAdminService("http://mge1.dev.ifs.hsr.ch/");

            var gadgets = service.GetAllGadgets();
            List<Gadget> AllG = new List<Gadget>();
            foreach (Gadget gadget in gadgets)
            {
                AllG.Add(new Gadget() { Name = gadget.Name, Price = gadget.Price, Condition = gadget.Condition, InventoryNumber = gadget.InventoryNumber, Manufacturer = gadget.Manufacturer });
            }
            allGadgets.ItemsSource = AllG;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(allGadgets.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Condition", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("InventoryNumber", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Manufacturer", ListSortDirection.Ascending));


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
        public class SortAdorner : Adorner
        {
            private static Geometry ascGeometry =
                    Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                    Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                    : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                        (
                                AdornedElement.RenderSize.Width - 15,
                                (AdornedElement.RenderSize.Height - 5) / 2
                        );
                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }
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

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            Window confirmationWindow = new Window();


            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            stackPanel.Children.Add(new Label { Content = "U Sure?" });
            stackPanel.Children.Add(new Button { Name = "Confirm", Content = "Confirm" });
            stackPanel.Children.Add(new Button { Name = "Cancel", Content = "Cancel" });

            confirmationWindow.Content = stackPanel;
            confirmationWindow.Show();
            this.Hide();
        }


    }
}
