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
using System.IO;
using System.Windows.Threading;

namespace projekt
{

    public class Bufe
    {
        public int id { get; set; } 
        public string rendeloneve { get; set; }
        public string mitrendelt { get; set; }
        public int db { get; set; }
        public string megjegyzes { get; set; }
        public int fizet { get; set; }
        public DateTime idopont { get; set; }
        public int AfaOsszeg => (int)(fizet * 1.27);

        public Bufe(int id, string rnev, string termek, int mennyiseg, string megjegy, int fizetni, DateTime idopontja)
        {
            this.id = id;
            rendeloneve = rnev;
            mitrendelt = termek;
            db = mennyiseg;
            megjegyzes = megjegy;
            fizet = fizetni;
            idopont = idopontja;
        }

        public Bufe()
        {
        }

        public override string ToString()
        {
            return $"{rendeloneve} - {idopont:yyyy-MM-dd HH:mm} - {AfaOsszeg} Ft";
        }
    }

    


    public partial class raktaros : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        string kapcsolat = "server=localhost:3307,;uid=root;password=;database=bufe;ssl mode=none";
        string felhasznalonev;
        string neve;
        public raktaros(string fn, string n)
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(30); // 30 másodperc
            timer.Tick += Timer_Tick;
            timer.Start();
            felhasznalonev = fn;
            neve = n;
            nev.Content = neve;
            listviewfel();
           
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            listviewfel();
        }
        
      

        public void listviewfel()
        {
            

            string sql = @"SELECT ko.kkod AS kosarkod,
                            v.nev AS rendeloneve, 
                            GROUP_CONCAT(CONCAT(t.tnev, ' (', k.tmenny, ' db)') SEPARATOR '\n') AS mitrendelt, 
                            ko.megjegyzes AS megjegyzes, 
                            SUM(t.ar * k.tmenny) AS fizet,  -- Itt számoljuk ki a fizetendő összeget
                            ko.idopont AS idopont
                        FROM kosarba k
                        JOIN termek t ON k.tid = t.tid
                        JOIN kosar ko ON k.kkod = ko.kkod
                        JOIN vasarlo v ON ko.email = v.email
                        GROUP BY v.nev, ko.idopont, ko.megjegyzes
                        ORDER BY  ko.idopont;";
            try
            {
                MySqlConnection con = new MySqlConnection(kapcsolat);
                con.Open();
                MySqlCommand msqlc = new MySqlCommand(sql, con);
                MySqlDataReader msdr = msqlc.ExecuteReader();
                listView.Items.Clear(); // Előző elemek törlése

                while (msdr.Read())
                {
                    int fizet = msdr["fizet"] != DBNull.Value ? Convert.ToInt32(msdr["fizet"]) : -1; // -1, ha nincs megadva fizetendő összeg

                    var bufe = new Bufe(
                       Convert.ToInt32(msdr["kosarkod"]),
                       msdr["rendeloneve"].ToString(),
                       msdr["mitrendelt"].ToString(),
                       0, // A db értéke nem használható, mert az egyesítve van a 'mitrendelt' mezőben
                       msdr["megjegyzes"].ToString(),
                       fizet,
                       msdr["idopont"] != DBNull.Value ? Convert.ToDateTime(msdr["idopont"]) : DateTime.MinValue
                    );


                    bool marBenneVan = false;

                    if (listBox.Items.Count > 0)
                    {
                        foreach (Bufe b in listBox.Items)
                        {
                            if (b.rendeloneve == bufe.rendeloneve && b.idopont == bufe.idopont)
                            {
                                marBenneVan = true;
                                break;
                            }
                        }
                    }

                    if (!marBenneVan)
                    {
                        listView.Items.Add(bufe);
                    }



                }



                msdr.Close();
                con.Close();
            }
            catch (Exception)
            {

                
            }
           
           
        }
        
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem is Bufe selectedBufe)
            {
                if (selectedBufe.fizet == -1)
                {
                    MessageBox.Show("Nincs megadva fizetendő összeg.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Példányosítjuk a BufeDetails ablakot
                BufeDetails detailsWindow = new BufeDetails(selectedBufe);
                // Feliratkozunk az OrderCompleted eseményre
                detailsWindow.OrderCompleted += DetailsWindow_OrderCompleted;
                detailsWindow.ShowDialog();
            }
        }
        private void DetailsWindow_OrderCompleted(object sender, BufeEventArgs e)
        {
            // Példa: Eltávolítjuk a rendelést a ListView-ból, és hozzáadjuk a ListBox-hoz
            listView.Items.Remove(e.Order);
            listBox.Items.Add(e.Order);
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

        private void listBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex!=-1)
            {
                kifizetve.IsEnabled = true;
            }
            else
            {
                kifizetve.IsEnabled = false;
            }
        }

        private void kifizetve_Click(object sender, RoutedEventArgs e)
        {

            if (listBox.SelectedItem is Bufe kivalasztott)
            {
                try
                {
                    string email = null;

                    using (MySqlConnection conn = new MySqlConnection(kapcsolat))
                    {
                        conn.Open();

                        // 1. Email lekérése
                        string emailQuery = "SELECT email FROM vasarlo WHERE nev = @nev LIMIT 1";
                        using (MySqlCommand cmd = new MySqlCommand(emailQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@nev", kivalasztott.rendeloneve);
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                                email = result.ToString();
                        }

                        if (email == null)
                        {
                            MessageBox.Show("Nem található e-mail cím a rendeléshez.");
                            return;
                        }

                        // 2. Számla beszúrása
                        string szamlaInsert = "INSERT INTO szamla (osszeg, email, termekek) VALUES (@osszeg, @email, @termekek)";
                        using (MySqlCommand insertCmd = new MySqlCommand(szamlaInsert, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@osszeg", kivalasztott.AfaOsszeg); // az összeg, áfával
                            insertCmd.Parameters.AddWithValue("@email", email);
                            insertCmd.Parameters.AddWithValue("@termekek", kivalasztott.mitrendelt); // terméklista szövegként
                            insertCmd.ExecuteNonQuery();
                        }

                        // 3. Kosárból törlés
                        string deleteQuery = "DELETE FROM kosar WHERE email = @e AND idopont = @i LIMIT 1";
                        using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@e", email);
                            deleteCmd.Parameters.AddWithValue("@i", kivalasztott.idopont);
                            int rows = deleteCmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                MessageBox.Show("Rendelés sikeresen törölve és számlázva.");
                                listviewfel(); // lista frissítése
                            }
                            else
                            {
                                MessageBox.Show("Nem sikerült törölni a rendelést.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Kérlek válassz ki egy rendelést!");
            }

            listBox.Items.Remove(listBox.SelectedItem);
        }
    }
}
