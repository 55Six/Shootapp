using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;


namespace ShootingCompetition
{
    /// <summary>
    /// Interaction logic for WindowPrecise.xaml
    /// </summary>
    ///

    public delegate void RoundChangedDel(int round, int tarType);

    public partial class WindowPrecise : Window
    {
        MainWindow mainW;
        DBManager dBManager;
        public PreciseShooting Competition { get; set; }

        RoundChangedDel roundChanged;

        // Remaning shots per round
        ObservableCollection<int> shotsRem;

        // List of comboboxes
        List<ComboBox> shotBoxArray;

        public WindowPrecise(MainWindow mainW, DBManager dBManager)
        {
            InitializeComponent();
            this.mainW = mainW;
            this.dBManager = dBManager;
            shotsRem = new ObservableCollection<int>();

            // Ad comboboxes to list for ease of use
            shotBoxArray = new List<ComboBox>();
            shotBoxArray.Add(comB0);
            shotBoxArray.Add(comB1);
            shotBoxArray.Add(comB2);
            shotBoxArray.Add(comB3);
            shotBoxArray.Add(comB4);
            shotBoxArray.Add(comB5);
            shotBoxArray.Add(comB6);
            shotBoxArray.Add(comB7);
            shotBoxArray.Add(comB8);
            shotBoxArray.Add(comB9);
            shotBoxArray.Add(comB10);
            shotBoxArray.Add(comBX);
        }

        // Close window
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Resets Shots stack panel
        void ResetStackp(UIElementCollection collection)
        {
            foreach (UIElement child in collection)
            {
                child.IsEnabled = false;
                child.Visibility = Visibility.Collapsed;
            }
        }

        // Fill stack panel
        void FillStackPanel()
        {
            ResetStackp(stackPanelShots.Children);
            shotsRem.Clear();

            // Set num of shots
            for (int i = 0; i <= Competition.Shots; i++)
            {
                shotsRem.Add(i);
            }

            // Set score 0
            shotBoxArray[0].ItemsSource = shotsRem;
            shotBoxArray[0].SelectedIndex = 0;
            stackPanelShots.Children[0].IsEnabled = true;
            stackPanelShots.Children[0].Visibility = Visibility.Visible;

            // Set combobox
            for (int i = Competition.TarType; i < shotBoxArray.Count; i++)
            {
                shotBoxArray[i].ItemsSource = shotsRem;
                shotBoxArray[i].SelectedIndex = 0;
                stackPanelShots.Children[i].IsEnabled = true;
                stackPanelShots.Children[i].Visibility = Visibility.Visible;
            }
        }


        // Loads competition from database
        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadPreciseWindow lpwindow = new LoadPreciseWindow(dBManager, this);
            lpwindow.ShowDialog();

            // If lpwindow created Competition then continue
            if (!lpwindow.Created || Competition == null)
            {
                return;
            }

            // Reset Buttons
            btnNext.IsEnabled = true;
            btnBack.IsEnabled = false;
            btnEnd.IsEnabled = false;
            btnResetShots.IsEnabled = true;

            // Enable Butons
            btnSearch.IsEnabled = true;
            btnSearchClear.IsEnabled = true;
            btnSetShots.IsEnabled = true;
            setTeamBtn.IsEnabled = true;

            //Disable Load
            itemLoad.IsEnabled = false;

            // Set info Labels
            lblRound.Content += Competition.Round.ToString();
            lblShooters.Content += Competition.Shooters.Count.ToString();
            lblShotsPR.Content += Competition.Shots.ToString();
            lblTrounds.Content += Competition.NRounds.ToString();

            FillStackPanel();

            /******************************************************************/
            dataGridShooters.ItemsSource = Competition.Shooters;
            dataGridIndivi.ItemsSource = Competition.Shooters;
            CollectionView viewDG = (CollectionView)CollectionViewSource.GetDefaultView(dataGridShooters.ItemsSource);
            //viewDG.GroupDescriptions.Add(new PropertyGroupDescription("Club"));
            viewDG.GroupDescriptions.Add(new PropertyGroupDescription("Team"));
            /******************************************************************/

            listViewCompetitiors.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectLVComp"], "LVComp");
            // Add view
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewCompetitiors.ItemsSource);
            // Grouping
            view.GroupDescriptions.Add(new PropertyGroupDescription("cname"));
            // Sorting
            view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("surname", ListSortDirection.Ascending));
        }

        // Double click to add competitor to the game
        private void listViewCompetitiors_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Competition == null || Competition.Started)
            {
                return;
            }

            DataRowView row = (DataRowView)listViewCompetitiors.SelectedItem;

            long id = (long)row["id"];

            // If id already exists dont add another one
            bool exists = Competition.Shooters.Any(s => s.Id == id);
            if (exists)
            {
                return;
            }

            string name = (string)row["name"];
            string surname = (string)row["surname"];
            string club = (string)row["cname"];
            string team = row["team"].ToString();

            Shooter sho = new Shooter(id, name, surname, club, ref roundChanged, team);
            Competition.AddShooter(sho);
            lblShooters.Content = "Shooters: " + Competition.Shooters.Count.ToString();
        }

        // Search through
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Prevents false search
            if (Competition == null || (txtBoxName.Text == "" && txtBoxSurname.Text == ""))
            {
                return;
            }

            string sql = dBManager.sqlLibrary["serchLVComp"];

            if (!string.IsNullOrEmpty(txtBoxName.Text))
            {
                sql += "name LIKE '" + txtBoxName.Text + "'";
            }
            else
            {
                sql += "1=1";
            }

            if (!string.IsNullOrEmpty(txtBoxSurname.Text))
            {
                sql += " AND surname LIKE '" + txtBoxSurname.Text + "'";
            }
            else
            {
                sql += " AND 1=1";
            }
            sql += ";";

            // Data source
            listViewCompetitiors.ItemsSource = dBManager.GetView(sql, "Search");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dBManager.GetView(sql, "Search"));
            // Grouping
            view.GroupDescriptions.Add(new PropertyGroupDescription("cname"));
            // Sorting
            view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("surname", ListSortDirection.Ascending));
        }

        private void btnSearchClear_Click(object sender, RoutedEventArgs e)
        {
            if (Competition == null)
            {
                return;
            }

            listViewCompetitiors.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectLVComp"], "LVComp");
            // Add view
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewCompetitiors.ItemsSource);
            // Grouping
            view.GroupDescriptions.Add(new PropertyGroupDescription("cname"));
            // Sorting
            view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("surname", ListSortDirection.Ascending));
        }

        // Set Shots
        private void SetShots_Click(object sender, RoutedEventArgs e)
        {

            // If non selected do nothing
            if (tabCompRoot.SelectedIndex == 1 && dataGridShooters.SelectedItem == null)
            {
                return;
            }
            else if (tabCompRoot.SelectedIndex == 0 && dataGridIndivi.SelectedItem == null)
            {
                return;
            }

            setTeamBtn.IsEnabled = false;
            txbTeam.IsEnabled = false;

            //After Competition started you cant add players anymore        
            Competition.Started = true;

            Shooter shooter;

            if (tabCompRoot.SelectedIndex == 0)
            {
                shooter = (Shooter)dataGridIndivi.SelectedItem;
            }
            else if (tabCompRoot.SelectedIndex == 1)
            {
                shooter = (Shooter)dataGridShooters.SelectedItem;
            }
            else
            {
                shooter = null;
            }

            // Check if null or already edited then return
            if (shooter == null)
            {
                return;
            }
            else if (shooter.Score.Count == Competition.Round)
            {
                return;
            }

            // Sets score, shots, missing zeros 
            SetResultPerRound(shooter, false);

            // Creates string to be shown on grid
            shooter.ShowHits(Competition.Round, Competition.TarType);

            ResetShotBoxArray();

            //PrintTest(shooter.ScoresPRound[Competition.Round - 1], shooter.Score[Competition.Round - 1]);
        }

        // Resets all numbers to 0
        private void ResetShotBoxArray()
        {
            foreach (ComboBox cb in shotBoxArray)
            {
                cb.SelectedIndex = 0;
            }
        }

        // Fill HITS and score
        void SetResultPerRound(Shooter shooter, bool rewrite)
        {
            int score = 0;
            int countShots = 0;

            List<int> tmpScores = new List<int>();

            // Add Zeroes
            tmpScores.Add((int)shotBoxArray[0].SelectedValue);

            for (int i = Competition.TarType; i < shotBoxArray.Count; i++)
            {
                tmpScores.Add((int)shotBoxArray[i].SelectedValue);
            }

            // Fill nonzero hits
            for (int i = 1; i < tmpScores.Count; i++)
            {
                countShots += tmpScores[i];
                score += tmpScores[i] * Competition.Scoring[i + (Competition.TarType - 1)];
            }

            // If not stated fill aditional misses as zeros
            if (Competition.Shots != countShots)
            {
                tmpScores[0] += (Competition.Shots - countShots);
            }

            if (countShots > Competition.Shots)
            {
                MessageBox.Show("ERROR! Too many shots. Please set again");
                return;
            }

            if (rewrite && shooter.Score.Count == Competition.Round)
            {
                shooter.RefillrScores(tmpScores, Competition.Round - 1);
                shooter.Score[Competition.Round - 1] = score;
            }
            else if (!rewrite)
            {
                shooter.FillRScores(tmpScores);
                shooter.Score.Add(score);
            }
            else
            {
                return;
            }

            //PrintTest(tmpScores, score);
        }

        void PrintTest(List<int> shotss, double score)
        {
            string fuu = "";

            foreach (int i in shotss)
            {
                fuu += i.ToString() + " ";
            }
            MessageBox.Show(fuu + "\nSCORE: " + score);
        }

        // Removes row in datagrid shooters 
        private void dataGridShooters_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Competition.Started)
            {
                return;
            }

            Shooter shooter;

            if (tabCompRoot.SelectedIndex == 0)
            {
                shooter = (Shooter)dataGridIndivi.SelectedItem;
            }
            else if (tabCompRoot.SelectedIndex == 1)
            {
                shooter = (Shooter)dataGridShooters.SelectedItem;
            }
            else
            {
                shooter = null;
            }
            if (shooter == null)
            {
                return;
            }
            Competition.RemoveShooter(shooter.Id);
            lblShooters.Content = "Shooters: " + Competition.Shooters.Count.ToString();
        }

        // Back button logic
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Competition.Round >= 1)
            {
                Competition.Round--;
            }
            if (btnEnd.IsEnabled && Competition.Round < Competition.NRounds)
            {
                btnEnd.IsEnabled = false;
                btnNext.IsEnabled = true;
            }
            if (btnBack.IsEnabled && Competition.Round <= 1)
            {
                btnBack.IsEnabled = false;
            }
            lblRound.Content = "Round: " + Competition.Round.ToString();
            roundChanged(Competition.Round, Competition.TarType);
        }

        // Next button logic
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            txbTeam.IsEnabled = false;
            txbTeam.IsEnabled = false;

            if (Competition.Shooters.Count == 0)
            {
                return;
            }
            Competition.Started = true;

            FillUnfiled();

            if (Competition.Round <= Competition.NRounds)
            {
                Competition.Round++;
            }
            if (Competition.Round > 1)
            {
                btnBack.IsEnabled = true;
            }
            if (Competition.Round == Competition.NRounds)
            {
                btnNext.IsEnabled = false;
                btnEnd.IsEnabled = true;
            }
            lblRound.Content = "Round: " + Competition.Round.ToString();

            roundChanged(Competition.Round, Competition.TarType);

        }

        //Fill shots for all shooters that empty with zero
        private void FillUnfiled()
        {

            List<int> hits;

            if (Competition.Shooters == null)
            {
                return;
            }

            for (int i = 0; i < Competition.Shooters.Count; i++)
            {
                if (Competition.Shooters[i].ScoresPRound.Count < Competition.Round)
                {
                    hits = new List<int>();
                    // fill with zeros
                    for (int j = 0; j < 13 - Competition.TarType; j++)
                    {
                        hits.Add(0);
                    }
                    hits[0] = Competition.Shots;
                    Competition.Shooters[i].ScoresPRound.Add(hits);
                }
                // Delete Inputs
                Competition.Shooters[i].RoundHits = "";
            }
        }

        // On click ends competition
        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            EndCompetitionMaster();
        }

        // End competition 
        public void EndCompetitionMaster()
        {

            // Disable buttons
            btnNext.IsEnabled = false;
            btnBack.IsEnabled = false;

            btnEnd.IsEnabled = false;
            setTeamBtn.IsEnabled = false;

            // End competiton
            listViewCompetitiors.ItemsSource = null;
            stackPanelShots.IsEnabled = false;
            btnSetShots.IsEnabled = false;
            btnSearch.IsEnabled = false;
            btnSearchClear.IsEnabled = false;
            btnResetShots.IsEnabled = false;

            FillUnfiled();

            //Show Shots
            roundChanged(Competition.Round, Competition.TarType);

            //Sorts by hits & total score
            Competition.EndCompetition();

            //Enable Statistics in the end
            menuItemStat.IsEnabled = true;

            WindowStatistics winstat = new WindowStatistics(Competition);
            winstat.Show();

            WriteToDB();
            /* Write to DB + message */
            MessageBox.Show("Competition ended, oppening statisctics window.", "End", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            mainW.ReloadSource();
            Close();
        }

        // Changes combobox settings
        private void dataGridShooters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Competition.Started)
            {
                return;
            }

            Shooter shooter;

            if (tabCompRoot.SelectedIndex == 0)
            {
                shooter = (Shooter)dataGridIndivi.SelectedItem;
            }
            else if (tabCompRoot.SelectedIndex == 1)
            {
                shooter = (Shooter)dataGridShooters.SelectedItem;
            }
            else
            {
                shooter = null;
            }
            if (shooter == null)
            {
                return;
            }
            txbTeam.Text = shooter.Team;
        }

        // Sets different TEAM
        private void setTeamBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Competition == null && Competition.Started == true)
            {
                return;
            }

            Shooter shooter;

            if (tabCompRoot.SelectedIndex == 0)
            {
                shooter = (Shooter)dataGridIndivi.SelectedItem;
            }
            else if (tabCompRoot.SelectedIndex == 1)
            {
                shooter = (Shooter)dataGridShooters.SelectedItem;
            }
            else
            {
                shooter = null;
            }

            if (txbTeam.Text == string.Empty)
            {
                return;
            }
            shooter.Team = txbTeam.Text;

            //Competition.RemoveShooter(shooter.Id);
            //Shooter sho = new Shooter(shooter.Id, shooter.Name, shooter.Surname, shooter.Club, ref roundChanged, txbTeam.Text.ToUpper());
            //Competition.AddShooter(sho);
        }

        //At the end show statistics
        private void menuItemStat_Click(object sender, RoutedEventArgs e)
        {
            WindowStatistics window = new WindowStatistics(Competition);
            window.Show();
        }

        private void btnResetShots_Click(object sender, RoutedEventArgs e)
        {
            // If non selected do nothing
            if (tabCompRoot.SelectedIndex == 1 && dataGridShooters.SelectedItem == null)
            {
                return;
            }
            else if (tabCompRoot.SelectedIndex == 0 && dataGridIndivi.SelectedItem == null)
            {
                return;
            }

            setTeamBtn.IsEnabled = false;
            txbTeam.IsEnabled = false;

            //After Competition started you cant add players anymore        
            Competition.Started = true;

            Shooter shooter = null;

            if (tabCompRoot.SelectedIndex == 0)
            {
                shooter = (Shooter)dataGridIndivi.SelectedItem;
            }
            else if (tabCompRoot.SelectedIndex == 1)
            {
                shooter = (Shooter)dataGridShooters.SelectedItem;
            }
            else
            {
                shooter = null;
            }

            if (shooter == null)
            {
                return;
            }

            // Sets score, shots, missing zeros 
            SetResultPerRound(shooter, true);

            // Creates string to be shown on grid
            shooter.ShowHits(Competition.Round, Competition.TarType);

            ResetShotBoxArray();
        }

        // IMPOLEMENTACIJA PISANJA V DB
        void WriteToDB()
        {
            // Check if write is meaningful
            if (Competition == null || Competition.Shooters.Count == 0)
            {
                return;
            }

            // Closes Competition
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updateEndGame"];
                command.Parameters.Add(new SQLiteParameter("id", Competition.Id));
                command.Parameters.Add(new SQLiteParameter("finished", true));
                conn.Open();
                command.ExecuteNonQuery();
            }

            for (int i = 0; i < Competition.Shooters.Count; i++)
            {
                // Get total shot list
                List<int> sTotal = Competition.Shooters[i].GetShoTotal();

                using (SQLiteConnection conn = dBManager.GetConnection())
                {
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = dBManager.sqlLibrary["insertShots"];

                    int stotal = sTotal.Count;

                    if (stotal >= 1)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldzero", sTotal[0]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldzero", null));
                    }

                    if (stotal >= 2)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldone", sTotal[1]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldone", null));
                    }

                    if (stotal >= 3)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldtwo", sTotal[2]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldtwo", null));
                    }

                    if (stotal >= 4)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldthree", sTotal[3]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldthree", null));
                    }

                    if (stotal >= 5)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldfour", sTotal[4]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldfour", null));
                    }

                    if (stotal >= 6)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldfive", sTotal[5]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldfive", null));
                    }

                    if (stotal >= 7)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldsix", sTotal[6]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldsix", null));
                    }

                    if (stotal >= 8)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldseven", sTotal[7]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldseven", null));
                    }

                    if (stotal >= 9)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldeight", sTotal[8]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldeight", null));
                    }

                    if (stotal >= 10)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldnine", sTotal[9]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldnine", null));
                    }

                    if (stotal >= 11)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldten", sTotal[10]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldten", null));
                    }

                    if (stotal >= 12)
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldeleven", sTotal[11]));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("fieldeleven", null));
                    }
                    conn.Open();
                    command.ExecuteNonQuery();
                }

                // Get Latest id to identify shots
                long MaxID;
                using (SQLiteConnection conn = dBManager.GetConnection())
                {
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = dBManager.sqlLibrary["selectShoID"];
                    conn.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        MaxID = (long)reader[0];
                    }
                }
                // Closes Competition
                using (SQLiteConnection conn = dBManager.GetConnection())
                {
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = dBManager.sqlLibrary["insertPresults"];
                    command.Parameters.Add(new SQLiteParameter("competitionid", Competition.Id));
                    command.Parameters.Add(new SQLiteParameter("shooterid", Competition.Shooters[i].Id));
                    command.Parameters.Add(new SQLiteParameter("shotsid", MaxID));
                    command.Parameters.Add(new SQLiteParameter("score", Competition.Shooters[i].totalScore()));
                    command.Parameters.Add(new SQLiteParameter("team", Competition.Shooters[i].Team));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

