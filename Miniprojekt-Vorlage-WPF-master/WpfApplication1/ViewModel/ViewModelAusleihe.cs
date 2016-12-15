using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Timers;

namespace Gadgeothek.ViewModel
{
    class ViewModelAusleihe : ViewModelAdmin
    {

        private ObservableCollection<AllOfCustomer> allCustomer;
        private ObservableCollection<ReserveByUser> allReservations;
        private ObservableCollection<LoansByUser> allLoans;
        private LibraryAdminService service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

        public ViewModelAusleihe()
        {
            allCustomer = new ObservableCollection<AllOfCustomer>();
            allReservations = new ObservableCollection<ReserveByUser>();
            allLoans = new ObservableCollection<LoansByUser>();

            collectData();



            //autoRefresh();

        }
        
        public ObservableCollection<AllOfCustomer> AllCustomer
        {
            get { return allCustomer; }
            set
            {
                allCustomer = value;
            }
        }
        public ObservableCollection<ReserveByUser> AllReservations
        {
            get { return allReservations; }
            set
            {
                allReservations = new ObservableCollection<ReserveByUser>();
                foreach (ReserveByUser reserveByUser in value)
                {
                    allReservations.Add(reserveByUser);
                }
            }
        }
        public ObservableCollection<LoansByUser> AllLoans
        {
            get { return allLoans; }
            set
            {
                allLoans = new ObservableCollection<LoansByUser>();
                foreach (LoansByUser loanbyuser in value)
                {
                    allLoans.Add(loanbyuser);
                }
            }
        }

        private void collectData()
        {
            var customers = service.GetAllCustomers();
            var reservations = service.GetAllReservations();
            var loans = service.GetAllLoans();

            AllCustomer.Clear();
            foreach (Customer c in customers)
            {
                List<Loan> cLoans = getLoans(c, loans);
                AllCustomer.Add(new AllOfCustomer(c.Studentnumber, c.Name, getReservations(c, reservations), cLoans, getToBackInformations(cLoans)));

            }


            foreach (Reservation r in reservations)
            {
                AllReservations.Add(new ReserveByUser(r.Customer.Name, r.Gadget.Name, r.WaitingPosition, r.IsReady));

            }
       
            foreach (Loan l in loans)
            {
                DateTime? date = l.ReturnDate;
                if (l.ReturnDate == null)
                {
                    date = l.OverDueDate;
                }
                AllLoans.Add(new LoansByUser(l.Gadget.Name, l.Customer.Name, date, l.IsOverdue, getResByGadget(l, reservations)));

            }
        }

        private void autoRefresh()
        {
            System.Timers.Timer statusTime = new System.Timers.Timer();
            statusTime.Interval = 1000;
            statusTime.Elapsed += new System.Timers.ElapsedEventHandler(timeElapsed);
            statusTime.Enabled = true;
        }

        private void timeElapsed(object sender, ElapsedEventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                collectData();
            });
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
    }
}
