using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using Gadgeothek.ViewModel;
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

        private ViewModelAusleihe gadgetModel;

        public Ausleihe()
        {
            InitializeComponent();
            gadgetModel = new ViewModelAusleihe();
            DataContext = gadgetModel;
            
        }
        

    }

}




