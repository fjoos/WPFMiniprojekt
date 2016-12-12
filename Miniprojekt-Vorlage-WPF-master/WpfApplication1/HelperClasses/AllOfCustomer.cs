using ch.hsr.wpf.gadgeothek.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
