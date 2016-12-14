using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using Gadgeothek.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Gadgeothek.ViewModel
{

    public class ViewModelGadgets : ViewModelAdmin
    {
        private ObservableCollection<Gadget> allGadgets { get; set; }
        LibraryAdminService service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
        private Gadget selectedGadget;
        private String option;

        public ViewModelGadgets()
        {
            allGadgets = new ObservableCollection<Gadget>(service.GetAllGadgets());
          
        }

        public ViewModelGadgets(Gadget selectedItem, string buttonName)
        {
            selectedGadget = selectedItem;
            Option = buttonName;
        }

        public ObservableCollection<Gadget> AllGadgets
        {
            get { return allGadgets; }
        }

        public Gadget SelectedGadget {
            get { return selectedGadget; }
            set { selectedGadget = value; }
        }

        public String Option {
            get { return option; }
            set { option = value; }
        }

        public void toDeleteGadget(Gadget gadget)
        {
            allGadgets.Remove(gadget);
            //service.DeleteGadget(gadget);
        }

        public void addGadget(Gadget gadget)
        {
            gadget = SelectedGadget;
            MessageBoxResult result = MessageBox.Show(gadget.Name, "Kontrollfrage", MessageBoxButton.YesNo);
          
            allGadgets.Add(gadget);
           //service.AddGadget(gadget);
            
        }

        public void saveGadget(Gadget modifiedGadget)
        {
            Gadget test = SelectedGadget;
            allGadgets.Insert(allGadgets.IndexOf(modifiedGadget), test);
            //service.UpdateGadget(test);
        }
    }
}
