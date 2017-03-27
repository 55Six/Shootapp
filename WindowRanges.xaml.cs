using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;

namespace ShootingCompetition
{
    public partial class WindowRanges : Window
    {
        WindowOpenMode mode;
        DBManager dBManager;
        DataGrid dataGridRanges;
        DataGrid dataGridPlaces;
        long id;

        public WindowRanges(WindowOpenMode mode, DBManager dBManager, DataGrid dataGridRanges, DataGrid dataGridPlaces)
        {
            InitializeComponent();
            this.mode = mode;
            this.dBManager = dBManager;
            this.dataGridRanges = dataGridRanges;
            this.dataGridPlaces = dataGridPlaces;
            initWindow();
        }

        void initWindow()
        {
            comboBoxPlace.ItemsSource = dataGridPlaces.ItemsSource;
            comboBoxPlace.DisplayMemberPath = "pname";
            comboBoxPlace.SelectedValuePath = "id";
            comboBoxPlace.SelectedIndex = 0;

            switch (mode)
            {
                case WindowOpenMode.Insert:
                    Title = "Insert Range Into Database";
                    buttonAction.Content = "Insert";
                    break;
                case WindowOpenMode.Update:
                    Title = "Update Range In Database";
                    buttonAction.Content = "Update";
                    DataRowView view = (DataRowView)dataGridRanges.SelectedItem;
                    id = (long)view[0];
                    textBoxRange.Text = view[1].ToString();
                    comboBoxPlace.SelectedValue = (long)view[2];
                    textBoxZip.Text = view[2].ToString();
                    break;
            }
        }

        // Insert or update club
        private void buttonAction_Click(object sender, RoutedEventArgs e)
        {
            // Check inpout
            if (textBoxRange.Text == string.Empty)
            {
                MessageBox.Show("Range box can't be empty!", "Warning!");
                return;
            }
            else if (comboBoxPlace.Text == string.Empty)
            {
                MessageBox.Show("Place combo box can't be empty!", "Warning!");
                return;
            }

            // Select mode: inserting or updating
            switch (mode)
            {
                case WindowOpenMode.Insert:
                    InsertRangeRecord((long)comboBoxPlace.SelectedValue, textBoxRange.Text);
                    break;
                case WindowOpenMode.Update:
                    UpdateRangeRecord((long)comboBoxPlace.SelectedValue, textBoxRange.Text);
                    break;
            }
            dataGridRanges.ItemsSource = dBManager.GetView(dBManager.sqlLibrary["selectRanges"], "Ranges");
            Close();
        }

        private void comboBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxZip.Text = comboBoxPlace.SelectedValue.ToString();
        }

        // Insert range record
        public void InsertRangeRecord(long placeid, string rname)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["insertRange"];
                command.Parameters.Add(new SQLiteParameter("rname", rname));
                command.Parameters.Add(new SQLiteParameter("placeid", placeid));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        // Update range record
        public void UpdateRangeRecord(long placeid, string rname)
        {
            using (SQLiteConnection conn = dBManager.GetConnection())
            {
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = dBManager.sqlLibrary["updateRange"];
                command.Parameters.Add(new SQLiteParameter("rname", rname));
                command.Parameters.Add(new SQLiteParameter("placeid", placeid));
                command.Parameters.Add(new SQLiteParameter("id", id));
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
