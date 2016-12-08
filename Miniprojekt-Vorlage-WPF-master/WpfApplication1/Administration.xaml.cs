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
using ch.hsr.wpf.gadgeothek.service;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Gadgeothek
{
    /// <summary>
    /// Interaction logic for Gadgeothek.xaml
    /// </summary>
    public partial class Administration : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        List<Gadget> toRemoveGadgets = new List<Gadget>();

        public ObservableCollection<Gadget> MyGadgets { get; set; }

        public Administration()
        {
            InitializeComponent();

            var service = new LibraryAdminService("http://mge5.dev.ifs.hsr.ch/");

            MyGadgets = new ObservableCollection<Gadget>(service.GetAllGadgets());
            DataContext = this;

            /*
             * Ausleihe
             */

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
    }
}

