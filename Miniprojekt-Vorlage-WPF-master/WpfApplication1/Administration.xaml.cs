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


        public Administration()
        {
            InitializeComponent();

            var service = new LibraryAdminService("http://mge1.dev.ifs.hsr.ch/");

            

            showGadgets(service, allGadgets);

            var customers = service.GetAllCustomers();
            var reservations = service.GetAllReservations();
            var loans = service.GetAllLoans();



            List<AllOfCustomer> allCustomer = new List<AllOfCustomer>();
            foreach (Customer c in customers)
            {
                List<Loan> cLoans = getLoans(c, loans);
                allCustomer.Add(new AllOfCustomer(c.Studentnumber, c.Name, getReservations(c, reservations), cLoans, getToBackInformations(cLoans)));

            }
            GadgetsByUser.ItemsSource = allCustomer;
            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(GadgetsByUser.ItemsSource);
            view2.Filter = UserFilter1;



            List<ReserveByUser> allReservations = new List<ReserveByUser>();

            foreach (Reservation r in reservations)
            {
                allReservations.Add(new ReserveByUser(r.Customer.Name, r.Gadget.Name, r.WaitingPosition, r.IsReady));

            }
            ReservationsByUsers.ItemsSource = allReservations;
            CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(ReservationsByUsers.ItemsSource);
            view3.Filter = UserFilter2;


            List<LoansByUser> allLoans = new List<LoansByUser>();
            foreach (Loan l in loans)
            {
                if (l.Gadget != null)
                {
                    allLoans.Add(new LoansByUser(l.Gadget.Name, l.Customer.Name, l.ReturnDate, l.IsOverdue, getResByGadget(l, reservations)));
                }
            }
            LeansByGadget.ItemsSource = allLoans;
            CollectionView view4 = (CollectionView)CollectionViewSource.GetDefaultView(LeansByGadget.ItemsSource);
            view4.Filter = UserFilter3;
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
            var service = new LibraryAdminService("http://mge1.dev.ifs.hsr.ch/");

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

        public class AllOfCustomer
        {

            public string KundenNr { get; set; }
            public string Name { get; set; }
            public string Reservations { get; set; }
            public string Loans { get; set; }
            public bool ToBack { get; set; }

            public AllOfCustomer(string knr, string name, List<Reservation> res, List<Loan> loan, bool toB)
            {
                foreach (Reservation r in res)
                {
                    if (r.Gadget != null)
                    {
                        Reservations += r.Gadget.Name + ", ";
                    }
                }

                foreach (Loan l in loan)
                {
                    if (l.Gadget != null)
                    {
                        Loans += l.Gadget.Name + ", ";
                    }
                }

                KundenNr = knr;
                Name = name;
                ToBack = toB;
            }

        }

        public class ReserveByUser
        {
            public string resCustomer { get; set; }
            public string NameRes { get; set; }
            public string WaitSizeRes { get; set; }
            public bool LeanRes { get; set; }

            public ReserveByUser(string customer, string name, int resSize, bool isRes)
            {
                resCustomer = customer;
                NameRes = name;
                WaitSizeRes = resSize.ToString();
                LeanRes = isRes;
            }
        }

        public class LoansByUser
        {
            public string NameLean { get; set; }
            public string LeanLean { get; set; }
            public string BackTillLean { get; set; }
            public bool ToBackLean { get; set; }
            public bool ReservedLean { get; set; }

            public LoansByUser(string name, string leanerName, DateTime? backTill, bool toBack, bool isRes)
            {
                NameLean = name;
                LeanLean = leanerName;
                BackTillLean = backTill.ToString();
                ToBackLean = toBack;
                ReservedLean = isRes;
            }

        }

        public bool getResByGadget(Loan l, List<Reservation> AllRes)
        {
            foreach (Reservation r in AllRes)
            {
                if (r.Gadget != null && r.Gadget.Equals(l))
                {
                    return true;
                }
            }
            return false;
        }

        public List<Reservation> getReservations(Customer c, List<Reservation> AllRes)
        {
            List<Reservation> res = new List<Reservation>();
            foreach (Reservation r in AllRes)
            {
                if (r.Customer != null && r.Customer.Equals(c))
                {
                    res.Add(r);
                }
            }

            return res;
        }


        public List<Loan> getLoans(Customer c, List<Loan> AllLoans)
        {

            List<Loan> loans = new List<Loan>();
            foreach (Loan l in AllLoans)
            {
                if (l.Customer != null && l.Customer.Equals(c))
                {
                    loans.Add(l);
                }
            }

            return loans;
        }

        public bool getToBackInformations(List<Loan> aList)
        {
            foreach (Loan l in aList)
            {
                if (l.IsOverdue)
                {
                    return true;
                }
            }
            return false;
        }

        private void search_ENTRY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GadgetsByUser.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(GadgetsByUser.ItemsSource).Refresh();
            }

            if (LeansByGadget.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(LeansByGadget.ItemsSource).Refresh();
            }
            if (ReservationsByUsers.ItemsSource != null)
            {
                CollectionViewSource.GetDefaultView(ReservationsByUsers.ItemsSource).Refresh();
            }

        }



        private bool UserFilter1(object item)
        {
            if (String.IsNullOrEmpty(search_ENTRY.Text))
                return true;
            else
                return ((item as AllOfCustomer).Name.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private bool UserFilter2(object item)
        {
            if (String.IsNullOrEmpty(search_ENTRY.Text))
                return true;
            else
                return ((item as ReserveByUser).resCustomer.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private bool UserFilter3(object item)
        {
            if (String.IsNullOrEmpty(search_ENTRY.Text))
                return true;
            else
                return ((item as LoansByUser).LeanLean.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }


        private void GadgetsByUser_SelectionChanged(object sender, RoutedEventArgs e)
        {
            sort(sender, e, GadgetsByUser);
        }
        private void LoanssByUser_SelectionChanged(object sender, RoutedEventArgs e)
        {
            sort(sender, e, LeansByGadget);
        }

        private void ReservationsByUser_SelectionChanged(object sender, RoutedEventArgs e)
        {
            sort(sender, e, ReservationsByUsers);
        }


        private void sort(object sender, RoutedEventArgs e, ListView lv)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lv.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lv.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

    }
}

