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
    public class Termekek
    {
        public int tid { get; set; }
        public string tnev { get; set; }
        public int mennyiseg { get; set; }
        public Termekek(int _tid, string _tnev, int _mennyiseg)
        {
            tid = _tid;
            tnev = _tnev;
            mennyiseg = _mennyiseg;
        }
    }
    public partial class keszlet : Window
    {
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        string felhasznalonev;
        string neve;
        public keszlet(string fn, string n)
        {
            InitializeComponent();
            felhasznalonev = fn;
            neve = n;
            nev.Content = neve;
            listviewfel();
            combofel1();
            combofel2();
        }

        public void combofel1()
        {
            string sql = "Select distinct kiszereles from termek order by kiszereles";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            while (msdr.Read())
            {
                kiszerelesek.Items.Add(msdr[0]);
            }
        }

        public void combofel2()
        {
            string sql = "Select distinct knev from kategoria order by knev";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            while (msdr.Read())
            {
                kategoriak.Items.Add(msdr[0]);
            }
        }

        public void listviewfel()
        {
            listView.Items.Clear();
            string sql = "Select tid, tnev, mennyiseg from termek order by tid";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            while (msdr.Read())
            {
                listView.Items.Add(new Termekek(Convert.ToInt32(msdr[0]), msdr[1].ToString(), Convert.ToInt32(msdr[2])));
            }
            con.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow uj = new MainWindow();
            uj.Show();
            this.Close();
        }

        private void Hozzaad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.Parse(ar.Text);
                try
                {
                    int.Parse(dbszam.Text);
                    int katid = 0;
                    string sql = $"Select kid from kategoria where knev='{kategoriak.SelectedItem}'";
                    MySqlConnection con = new MySqlConnection(kapcsolat);
                    con.Open();
                    MySqlCommand msqlc = new MySqlCommand(sql, con);
                    MySqlDataReader msdr = msqlc.ExecuteReader();
                    while (msdr.Read())
                    {
                        katid = Convert.ToInt32(msdr[0]);
                    }
                    string sql2 = $"INSERT INTO `termek`(`tnev`, `mennyiseg`, `kiszereles`, `ar`, `afa`, `kid`, `kepurl`) VALUES ('{tnev.Text}','{dbszam.Text}','{kiszerelesek.SelectedItem}','{ar.Text}','{27}','{katid}','{tkep.Text}')";
                    MySqlConnection con2 = new MySqlConnection(kapcsolat);
                    con2.Open();
                    MySqlCommand msqlc2 = new MySqlCommand(sql2, con2);
                    msqlc2.ExecuteNonQuery();
                    con2.Close();
                    tnev.Text = "";
                    dbszam.Text = "";
                    kiszerelesek.SelectedIndex = -1;
                    ar.Text = "";
                    kategoriak.SelectedIndex = -1;
                    listviewfel();
                }
                catch 
                {
                    MessageBox.Show("Nem megfelelő a darabszám formátuma");
                }
            }
            catch 
            {
                MessageBox.Show("Nem megfelelő az ár formátuma");
            }
           
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedIndex!=-1)
            {
                Feltotes.IsEnabled = true;
                Modositas.IsEnabled = true;
                Torles.IsEnabled = true;
            }
            else
            {
                Feltotes.IsEnabled = false;
                Modositas.IsEnabled = false;
                Torles.IsEnabled = false;
            }
        }

        private void Feltotes_Click(object sender, RoutedEventArgs e)
        {
            darabhozzaad uj = new darabhozzaad(((Termekek)listView.SelectedItem).tid, this);
            uj.Show();
        }


        private void Torles_Click(object sender, RoutedEventArgs e)
        {
            string sql2 = $"DELETE FROM `termek` WHERE tid='{((Termekek)listView.SelectedItem).tid}'";
            MySqlConnection con2 = new MySqlConnection(kapcsolat);
            con2.Open();
            MySqlCommand msqlc2 = new MySqlCommand(sql2, con2);
            msqlc2.ExecuteNonQuery();
            con2.Close();
            listviewfel();
        }

        private void tnev_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tnev.Text!="" && dbszam.Text!="" && ar.Text!="" && kiszerelesek.SelectedIndex!=-1 && kategoriak.SelectedIndex!=-1 && tkep.Text!="")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
            if (tnev.Text!="")
            {
                placehold1.Visibility = Visibility.Hidden;
            }
            else
            {
                placehold1.Visibility = Visibility.Visible;
            }
        }

        private void dbszam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tnev.Text != "" && dbszam.Text != "" && ar.Text != "" && kiszerelesek.SelectedIndex != -1 && kategoriak.SelectedIndex != -1 && tkep.Text != "")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
            if (dbszam.Text != "")
            {
                placehold2.Visibility = Visibility.Hidden;
            }
            else
            {
                placehold2.Visibility = Visibility.Visible;
            }
        }

        private void ar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tnev.Text != "" && dbszam.Text != "" && ar.Text != "" && kiszerelesek.SelectedIndex != -1 && kategoriak.SelectedIndex != -1 && tkep.Text != "")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
            if (ar.Text != "")
            {
                placehold3.Visibility = Visibility.Hidden;
            }
            else
            {
                placehold3.Visibility = Visibility.Visible;
            }
        }

        private void kiszerelesek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tnev.Text != "" && dbszam.Text != "" && ar.Text != "" && kiszerelesek.SelectedIndex != -1 && kategoriak.SelectedIndex != -1 && tkep.Text != "")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
        }

        private void kategoriak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tnev.Text != "" && dbszam.Text != "" && ar.Text != "" && kiszerelesek.SelectedIndex != -1 && kategoriak.SelectedIndex != -1 && tkep.Text != "")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
        }

        private void Modositas_Click(object sender, RoutedEventArgs e)
        {
            termekmodositas uj = new termekmodositas(((Termekek)listView.SelectedItem).tid, this);
            uj.Show();
        }

        private void tkep_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tnev.Text != "" && dbszam.Text != "" && ar.Text != "" && kiszerelesek.SelectedIndex != -1 && kategoriak.SelectedIndex != -1 && tkep.Text != "")
            {
                Hozzaad.IsEnabled = true;
            }
            else
            {
                Hozzaad.IsEnabled = false;
            }
            if (tkep.Text != "")
            {
                placehold4.Visibility = Visibility.Hidden;
            }
            else
            {
                placehold4.Visibility = Visibility.Visible;
            }
        }

        private void Kereses_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Kereses.Text != "")
            {
                placehold5.Visibility = Visibility.Hidden;
                listView.Items.Clear();
                string sql = $"Select tid, tnev, mennyiseg from termek where tnev like '%{Kereses.Text}%' order by tid";
                MySqlConnection con = new MySqlConnection(kapcsolat);
                con.Open();
                MySqlCommand msqlc = new MySqlCommand(sql, con);
                MySqlDataReader msdr = msqlc.ExecuteReader();
                while (msdr.Read())
                {
                    listView.Items.Add(new Termekek(Convert.ToInt32(msdr[0]), msdr[1].ToString(), Convert.ToInt32(msdr[2])));
                }
                con.Close();
            }
            else
            {
                placehold5.Visibility = Visibility.Visible;
                listviewfel();
            }
        }
    }
}
