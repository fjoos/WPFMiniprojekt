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
    /// Interaction logic for Ausleihe.xaml
    /// </summary>
    public partial class Ausleihe : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        public Ausleihe()
        {
            var service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            InitializeComponent();

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
