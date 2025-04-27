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
    /// Interaction logic for darabhozzaad.xaml
    /// </summary>
    public partial class darabhozzaad : Window
    {
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        int tid;
        private keszlet parentWindow;
        public darabhozzaad(int id, keszlet parent)
        {
            InitializeComponent();
            parentWindow = parent;
            tid = id;
            string sql = $"Select tnev from termek where tid='{id}'";
            MySqlConnection con = new MySqlConnection(kapcsolat);
            con.Open();
            MySqlCommand msqlc = new MySqlCommand(sql, con);
            MySqlDataReader msdr = msqlc.ExecuteReader();
            while (msdr.Read())
            {
                label1.Content=$"{msdr[0]} darabszámának növelése";
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
                int.Parse(TermekTextBox.Text);
                int regidb = 0;
                string sql = $"Select mennyiseg from termek where tid='{tid}'";
                MySqlConnection con = new MySqlConnection(kapcsolat);
                con.Open();
                MySqlCommand msqlc = new MySqlCommand(sql, con);
                MySqlDataReader msdr = msqlc.ExecuteReader();
                while (msdr.Read())
                {
                    regidb = Convert.ToInt32(msdr[0]);
                }
                con.Close();
                string sql2 = $"UPDATE `termek` SET `mennyiseg`='{(int.Parse(TermekTextBox.Text)+regidb)}' WHERE tid='{tid}'";
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
                TermekTextBox.Text = "";
                MessageBox.Show("Hibás formátum");
            }
        }

        private void TermekTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TermekTextBox.Text!="")
            {
                Mentes.IsEnabled = true;
            }
            else
            {
                Mentes.IsEnabled = false;
            }
        }
    }
}
