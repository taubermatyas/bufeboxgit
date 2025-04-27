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
using MySql.Data.MySqlClient;

namespace projekt
{
    /// <summary>
    /// Interaction logic for termekmodositas.xaml
    /// </summary>
    public partial class termekmodositas : Window
    {
        private keszlet parentWindow;
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        int tid;
        public termekmodositas(int id, keszlet parent)
        {
            InitializeComponent();
            parentWindow = parent;
            tid = id;
            string sql = $"Select tnev, ar, mennyiseg, kepurl from termek where tid='{id}'";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            while (msdr.Read())
            {
                TermekNevTextBox.Text = $"{msdr[0]}";
                ArTextBox.Text = $"{msdr[1]}";
                DarabTextBox.Text = $"{msdr[2]}";
                KepTextBox.Text = $"{msdr[3]}";
            }
            con.Close();
        }

        private void Megse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Mentes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.Parse(DarabTextBox.Text);
                try
                {
                    int.Parse(ArTextBox.Text);
                    string sql2 = $"UPDATE `termek` SET `ar`='{int.Parse(ArTextBox.Text)}', `tnev`='{TermekNevTextBox.Text}' , `mennyiseg`='{int.Parse(DarabTextBox.Text)}', `kepurl`='{KepTextBox.Text}' WHERE tid='{tid}'";
                    MySqlConnection con2 = new MySqlConnection(kapcsolat);
                    con2.Open();
                    MySqlCommand msqlc2 = new MySqlCommand(sql2, con2);
                    msqlc2.ExecuteNonQuery();
                    con2.Close();
                    parentWindow?.listviewfel();
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Hibás formátum az ár mezőben");
                    string sql = $"Select ar from termek where tid='{tid}'";
                    MySqlConnection con = new MySqlConnection(kapcsolat);
                    con.Open();
                    MySqlCommand msqlc = new MySqlCommand(sql, con);
                    MySqlDataReader msdr = msqlc.ExecuteReader();
                    while (msdr.Read())
                    {
                        ArTextBox.Text = $"{msdr[0]}";
                    }
                    con.Close();
                }
            }
            catch 
            {
                MessageBox.Show("Hibás formátum a darabszám mezőben");
                string sql = $"Select mennyiseg from termek where tid='{tid}'";
                MySqlConnection con = new MySqlConnection(kapcsolat);
                con.Open();
                MySqlCommand msqlc = new MySqlCommand(sql, con);
                MySqlDataReader msdr = msqlc.ExecuteReader();
                while (msdr.Read())
                {
                    DarabTextBox.Text = $"{msdr[0]}";
                }
                con.Close();
            }
        }

        private void TermekNevTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TermekNevTextBox.Text!="" && ArTextBox.Text!="" && DarabTextBox.Text!="" && KepTextBox.Text!="")
            {
                Mentés.IsEnabled = true;
            }
            else
            {
                Mentés.IsEnabled = false;
            }
        }

        private void ArTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TermekNevTextBox.Text != "" && ArTextBox.Text != "" && DarabTextBox.Text != "" && KepTextBox.Text != "")
            {
                Mentés.IsEnabled = true;
            }
            else
            {
                Mentés.IsEnabled = false;
            }
        }

        private void DarabTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TermekNevTextBox.Text != "" && ArTextBox.Text != "" && DarabTextBox.Text != "" && KepTextBox.Text != "")
            {
                Mentés.IsEnabled = true;
            }
            else
            {
                Mentés.IsEnabled = false;
            }
        }

        private void KepTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TermekNevTextBox.Text != "" && ArTextBox.Text != "" && DarabTextBox.Text != "" && KepTextBox.Text != "")
            {
                Mentés.IsEnabled = true;
            }
            else
            {
                Mentés.IsEnabled = false;
            }
        }
    }
}
