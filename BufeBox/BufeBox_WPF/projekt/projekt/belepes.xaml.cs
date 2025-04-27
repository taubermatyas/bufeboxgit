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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;

namespace projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        public MainWindow()
        {
            InitializeComponent();
        }
        public void teszt()
        {
            if (felhasz.Text != "" && jelszo1.Password != "")
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        private void felhasz_TextChanged(Object sender, TextChangedEventArgs e)

        {
            teszt();
            if (felhasz.Text != "")
            {

                placehold.Visibility = Visibility.Hidden;

            }
            else
            {

                placehold.Visibility = Visibility.Visible;

            }

        }

        private void jelszo1_PasswordChanged_1(object sender, RoutedEventArgs e)
        {
            {
                teszt();
                if (jelszo1.Password.Length != 0)
                {

                    placehold2.Visibility = Visibility.Hidden;

                }
                else
                {

                    placehold2.Visibility = Visibility.Visible;

                }
            }
        }


        private void button_Click_1(object sender, RoutedEventArgs e)
        {

            string sql = $"select felnev, jelszo,nev from dolgozo where felnev='{felhasz.Text}' and jelszo=sha2('{jelszo1.Password}',256)";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            bool volt = false;
            string nev = "";
            while (msdr.Read())
            {
                volt = true;
                nev = Convert.ToString(msdr[2]);
            }
            msdr.Close();
            con.Close();
            if (volt == true)
            {
                raktaros uj = new raktaros(felhasz.Text, nev);
                uj.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Hibás bejelentkezési adatok");
                felhasz.Text = "";
                jelszo1.Password = "";
            }
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {


            string sql = $"select felnev, jelszo,nev from dolgozo where felnev='{felhasz.Text}' and jelszo=sha2('{jelszo1.Password}',256)";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            bool volt = false;
            string nev = "";
            while (msdr.Read())
            {
                volt = true;
                nev = Convert.ToString(msdr[2]);
            }
            msdr.Close();
            con.Close();
            if (volt == true)
            {
                keszlet uj = new keszlet(felhasz.Text, nev);
                uj.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Hibás bejelentkezési adatok");
                felhasz.Text = "";
                jelszo1.Password = "";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Click_1(sender, e);
            }
        }

    }
}


