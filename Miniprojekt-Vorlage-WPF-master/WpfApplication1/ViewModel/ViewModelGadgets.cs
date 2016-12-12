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

namespace Gadgeothek.ViewModel
{

    public class ViewModelGadgets : ViewModelAdmin
    {
        public ObservableCollection<Gadget> allGadgets { get; set; }
        LibraryAdminService service = new LibraryAdminService("http://mge5.dev.ifs.hsr.ch/");


        public ViewModelGadgets()
        {
             allGadgets = new ObservableCollection<Gadget>(service.GetAllGadgets());

        }


        public ObservableCollection<Gadget> AllGadgets
        {
            get { return allGadgets; }
        }


        public void toDeleteGadget(Gadget gadget)
        {

            //service.DeleteGadget(gadget);
            allGadgets.Remove(gadget);
        }
        

    }
}
