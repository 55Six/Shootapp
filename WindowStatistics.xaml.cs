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

namespace ShootingCompetition
{
    /// <summary>
    /// Interaction logic for WindowStatistics.xaml
    /// </summary>
    public partial class WindowStatistics : Window
    {
        PreciseShooting competition;

        public WindowStatistics(PreciseShooting competition)
        {
            InitializeComponent();
            this.competition = competition;
            dataGridIndivi.ItemsSource = competition.Shooters;
            dataGridShooters.ItemsSource = competition.Shooters;

            CollectionView viewDG = (CollectionView)CollectionViewSource.GetDefaultView(dataGridShooters.ItemsSource);            
            viewDG.GroupDescriptions.Add(new PropertyGroupDescription("Team"));

            PIndividual();
            PTeam();
            SetLabels();
        }

        // Sets Labels
        private void SetLabels()
        {
            lblCid.Content = competition.Id;
            lblTitle.Content = competition.Title;
            lblRange.Content = competition.Range;
            lblDate.Content = competition.CDate.ToString();
            lblTarget.Content = competition.TarType;
            lblShots.Content = competition.Shots;
            lblRounds.Content = competition.NRounds;
        }

        // Sets team printing text
        private void PTeam()
        {
            tbTeamStat.Text = "\n";
            int rank = 0;
            for (int i = 0; i < competition.Teams.Count; i++)
            {
                rank++;
                tbTeamStat.Text += rank.ToString() + ". ";
                tbTeamStat.Text += competition.Teams[i].PrintTeam(competition.TarType);
                tbTeamStat.Text += "\n";
            }
        }

        // Sets individual printing text
        private void PIndividual()
        {
            tbIndStat.Text = "\n";
            int rank = 0;
            for (int i = 0; i < competition.Shooters.Count; i++)
            {
                rank++;
                tbIndStat.Text += rank.ToString() + ". ";
                tbIndStat.Text += competition.Shooters[i].ShoPrint(competition.TarType);
                tbIndStat.Text += "\n\n";
            }
        }

        private void MenuItem_Click_Print(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                switch (tbcRoot.SelectedIndex)
                {
                    case 0:
                        printDialog.PrintVisual(tbIndStat, "Print Individual");
                        break;
                    case 1:
                        printDialog.PrintVisual(tbTeamStat, "Print Team");
                        break;
                    case 2:
                        printDialog.PrintVisual(dataGridIndivi, "Print Individual Grid");
                        break;
                    case 3:
                        printDialog.PrintVisual(dataGridShooters, "Print Team Grid");
                        break;
                    case 4:
                        printDialog.PrintVisual(stackRoot, "Print Competition Details");
                        break;
                }
            }
        }
    }
}
