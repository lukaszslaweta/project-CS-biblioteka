﻿using System;
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

namespace project_cs_library
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bibliotekaEntities context;
        public MainWindow()
        {
            InitializeComponent();
            context = new bibliotekaEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataDoPicker.SelectedDate = DateTime.Now;
            System.Windows.Data.CollectionViewSource wypozyczenieViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("wypozyczenieViewSource")));
            wypozyczenieViewSource.Source = context.wypozyczenie.ToList();
        }

        private void PerformSearch(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource wypozyczenieViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("wypozyczenieViewSource")));
            String title = titleText.Text;
            String autor = autorText.Text;
            DateTime? dateFrom = dataOdPicker.SelectedDate;
            DateTime? dateTo = dataDoPicker.SelectedDate;

            var query = from wypozyczenie in context.wypozyczenie
                        where 
                        (title.Length > 0 ? wypozyczenie.ksiazka.tytul.Contains(title) : true) &&
                        (autor.Length > 0 ? wypozyczenie.ksiazka.autor.imie.Contains(autor) || wypozyczenie.ksiazka.autor.nazwisko.Contains(autor) : true) &&
                        (dateFrom != null ? wypozyczenie.data_oddania > dateFrom || wypozyczenie.data_wypozyczenia > dateFrom : true) &&
                        (dateTo != null ? wypozyczenie.data_oddania < dateTo || wypozyczenie.data_wypozyczenia < dateTo : wypozyczenie.data_oddania == null)
                        select wypozyczenie;

            wypozyczenieViewSource.Source = query.ToList();
        }

        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch(null, null);
            }
        }

        private void SearchDatePicker(object sender, SelectionChangedEventArgs e)
        {
            PerformSearch(null, null);
        }

        private void AddNewEntry(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
            AddWypozyczenie addWypozyczenie = new AddWypozyczenie(context);
            addWypozyczenie.ShowDialog();
            System.Windows.Data.CollectionViewSource wypozyczenieViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("wypozyczenieViewSource")));
            wypozyczenieViewSource.Source = context.wypozyczenie.ToList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            context.SaveChanges();
        }
    }
}
