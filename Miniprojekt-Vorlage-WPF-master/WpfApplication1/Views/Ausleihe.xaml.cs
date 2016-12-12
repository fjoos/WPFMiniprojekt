using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using static Gadgeothek.Administration;

namespace Gadgeothek.Views
{

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



            ObservableCollection<AllOfCustomer> allCustomer = new ObservableCollection<AllOfCustomer>();
            foreach (Customer c in customers)
            {
                List<Loan> cLoans = getLoans(c, loans);
                allCustomer.Add(new AllOfCustomer(c.Studentnumber, c.Name, getReservations(c, reservations), cLoans, getToBackInformations(cLoans)));

            }
            GadgetsByUser.ItemsSource = allCustomer;
            CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(GadgetsByUser.ItemsSource);
            view1.Filter = UserFilter1;



            ObservableCollection<ReserveByUser> allReservations = new ObservableCollection<ReserveByUser>();
            foreach (Reservation r in reservations)
            {
                allReservations.Add(new ReserveByUser(r.Customer.Name, r.Gadget.Name, r.WaitingPosition, r.IsReady));

            }
            ReservationsByUsers.ItemsSource = allReservations;
            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(ReservationsByUsers.ItemsSource);
            view2.Filter = UserFilter2;


            ObservableCollection<LoansByUser> allLoans = new ObservableCollection<LoansByUser>();
            foreach (Loan l in loans)
            {
                    DateTime? date = l.ReturnDate;
                    if (l.ReturnDate == null)
                    {
                        date = l.OverDueDate;
                    }
                    allLoans.Add(new LoansByUser(l.Gadget.Name, l.Customer.Name, date, l.IsOverdue, getResByGadget(l, reservations)));
                
            }
            LeansByGadget.ItemsSource = allLoans;
            CollectionView view3 = (CollectionView)CollectionViewSource.GetDefaultView(LeansByGadget.ItemsSource);
            view3.Filter = UserFilter3;
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
                return ((item as global::AllOfCustomer).Name.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private bool UserFilter2(object item)
        {
            if (String.IsNullOrEmpty(search_ENTRY.Text))
                return true;
            else
                return ((item as global::ReserveByUser).resCustomer.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private bool UserFilter3(object item)
        {
            if (String.IsNullOrEmpty(search_ENTRY.Text))
                return true;
            else
                return ((item as global::LoansByUser).LeanLean.IndexOf(search_ENTRY.Text, StringComparison.OrdinalIgnoreCase) >= 0);
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




