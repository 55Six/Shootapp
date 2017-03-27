using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;

namespace ShootingCompetition
{
    public partial class WindowCompetitions : Window
    {
        WindowOpenMode mode;
        DBManager dBManager;
        DataGrid dataGridPCompetitions;
        DataGrid dataGridRanges;
        long id;
        bool editable;

        public WindowCompetitions(DBManager dBManager)
        {
            InitializeComponent();
            this.dBManager = dBManager;
            initWindow();
        }

        public WindowCompetitions(WindowOpenMode mode, DBManager dBManager, DataGrid dataGridPCompetitions, DataGrid dataGridRanges)
        {
            InitializeComponent();
            this.mode = mode;
            this.dBManager = dBManager;
            this.dataGridPCompetitions = dataGridPCompetitions;
            this.dataGridRanges = dataGridRanges;
            initWindow();
        }

        // Initialize window
        void initWindow()
        {
            comboBoxRange.ItemsSource = dataGridRanges.ItemsSource;
            comboBoxRange.DisplayMemberPath = "rname";
            comboBoxRange.SelectedValuePath = "id";
            comboBoxRange.SelectedIndex = 0;

            switch (mode)
            {
                case WindowOpenMode.Insert:
                    Title = "Insert Precise Competition Into Database";
                    buttonAction.Content = "Insert";
                    break;
                case WindowOpenMode.Update:
                    Title = "Update Precise Competition In Database";
                    buttonAction.Content = "Update";
                    DataRowView view = (DataRowView)dataGridPCompetitions.SelectedItem;
                    id = (long)view[0];
                    textBoxTitle.Text = view[1].ToString();
                    comboBoxRange.SelectedValue = (long)view[2];
                    datePickerDate.Text = view[3].ToString();
                    textBoxTType.Text = view[4].ToString();
                    textBoxNShots.Text = view[5].ToString();
                    textBoxRounds.Text = view[6].ToString();
                    editable = (bool)view[7];
                    break;
            }
        }

        // Insert or update
        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            int tartype;
            // Check input
            if (editable)
            {
                MessageBox.Show("Competition is finished and is not editable", "Warning!");
                return;
            }
            else if (textBoxTitle.Text == string.Empty)
            {
                MessageBox.Show("Title box can't be empty!", "Warning!");
                return;
            }
            else if (textBoxTType.Text == string.Empty)
            {
                MessageBox.Show("Target box can't be empty!", "Warning!");
                return;
            }
            else if (comboBoxRange.Text == string.Empty)
            {
                MessageBox.Show("Range combo box can't be empty!", "Warning!");
                return;
            }
            else if (!(datePickerDate.SelectedDate is DateTime))
            {
                MessageBox.Show("Pick valid date!", "Warning!");
                return;
            }

            // Convert tartype text to int
            else if (!int.TryParse(textBoxTType.Text, out tartype) && !(tartype > 10 || tartype < 1))
            {
                MessageBox.Show("Target type has to be numeric, maximum 10 and minimum 1!", "Warning!");
                return;
            }

            // Convert shots text to int
            int shots;
            if (!int.TryParse(textBoxNShots.Text, out shots) && !(shots > 20 || shots < 1))
            {
                MessageBox.Show("Number of shots has to be numeric, maximum 10 and minimum 1!", "Warning!");
                return;
            }

            int nRounds;
            if (!int.TryParse(textBoxRounds.Text, out nRounds) && !(nRounds > 5 || nRounds < 1))
            {
                MessageBox.Show("Number of rounds has to be numeric, maximum 5 and minimum 1!", "Warning!");
                return;
            }

            //Convert date
            DateTime dt = (DateTime)datePickerDate.SelectedDate;
            string date = dt.ToString("yyyy MM dd").Replace(" ", "-");

            switch (mode)
            {
                case WindowOpenMode.Insert:
                    InsertPCompetitionRecord(textBoxTitle.Text, (long)comboBoxRange.SelectedValue, date, tartype, shots, nRounds);
                    break;
                case WindowOpenMode.Update:
                    UpdatePCompetitionRecord(textBoxTitle.Text, (long)comboBoxRange.SelectedValue, date, tartype, shots, nRounds);
                    break;
            }

            dataGridPCompetitions.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectPCompetitions"], "PCompetitions");
            Close();
        }

        // Insert tekma record
        void InsertPCompetitionRecord(string title, long rangeid, string date, int tartype, int shots, int nRounds)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["insertPCompetition"];
                command.Parameters.Add(new SQLiteParameter("title", title));
                command.Parameters.Add(new SQLiteParameter("rangeid", rangeid));
                command.Parameters.Add(new SQLiteParameter("date", date));
                command.Parameters.Add(new SQLiteParameter("tartype", tartype));
                command.Parameters.Add(new SQLiteParameter("shots", shots));
                command.Parameters.Add(new SQLiteParameter("rounds", nRounds));
                command.Parameters.Add(new SQLiteParameter("finished", false));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update tekma record
        void UpdatePCompetitionRecord(string title, long rangeid, string date, int tartype, int shots, int nRounds)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updatePCompetition"];
                command.Parameters.Add(new SQLiteParameter("id", id));
                command.Parameters.Add(new SQLiteParameter("title", title));
                command.Parameters.Add(new SQLiteParameter("rangeid", rangeid));
                command.Parameters.Add(new SQLiteParameter("date", date));
                command.Parameters.Add(new SQLiteParameter("tartype", tartype));
                command.Parameters.Add(new SQLiteParameter("shots", shots));
                command.Parameters.Add(new SQLiteParameter("rounds", nRounds));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Close window
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
