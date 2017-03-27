using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ShootingCompetition
{
    public class Shooter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static string[] particles = new string[] { "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11" };

        // General info
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Club { get; set; }

        // If more people from same club
        private string team;
        public string Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                OnPropertyChanged("Team");
            }
        }

        private string roundHits;
        public string RoundHits
        {
            get
            {
                return roundHits;
            }
            set
            {
                roundHits = value;
                OnPropertyChanged("RoundHits");
            }
        }

        string total;
        public string Total
        {
            get
            {
                return total;
            }
            set
            {
                total = value;
                OnPropertyChanged("Total");
            }
        }

        // Total score per round
        public List<int> Score { get; set; }

        // Sshots from low to high ex. (0x0, 2x1, 3x2, ...) 
        public List<List<int>> ScoresPRound { get; set; }        

        public int totalScore()
        {
            int d = 0;
            foreach (int dd in Score)
            {

                d += dd;
            }
            return d;

        }

        // Gets list of sum of shots for all rounds
        public List<int> GetShoTotal()
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < ScoresPRound.Count; i++)
            {
                for (int j = 0; j < ScoresPRound[i].Count; j++)
                {
                    if (temp.Count < j + 1)
                    {
                        temp.Add(0);
                    }
                    temp[j] += ScoresPRound[i][j];
                }
            }
            return temp;
        }

        // Knows How To print Itself
        public string ShoPrint(int tarType)
        {

            List<int> temp = GetShoTotal();

            string str = Name + " " + Surname + " " + Club + " : ";

            str += "(" + temp[0] + particles[0] + " "; //Zero Hits

            int j = tarType;

            for (int i = 1; i < temp.Count; i++)
            {
                str += temp[i].ToString() + particles[j] + " ";
                j++;
            }

            str += ")";

            str += " : TOTAL (" + totalScore() + ")";

            return str;
        }

        // Cnstructor
        public Shooter(long id, string name, string surname, string club, ref RoundChangedDel roundChanged, string team)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Club = club;
            ScoresPRound = new List<List<int>>();
            Score = new List<int>();
            roundChanged += ShowHits;

            if (team == string.Empty || team == null)
            {
                Team = "NO TEAM";
            }
            else
            {
                Team = team;
            }
        }

        //Constructor for loading from DB
        public Shooter(long id, string name, string surname, string club, string team)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Club = club;
            ScoresPRound = new List<List<int>>();
            Score = new List<int>();
            Team = team;
        }

        // Method for showing scores
        public void ShowHits(int round, int tarType)
        {
            if (ScoresPRound.Count < round)
                return;

            string s = "( ";
            List<int> hits = null;

            if (ScoresPRound != null)
            {
                hits = ScoresPRound[round - 1];
            }
            else
            {
                return;
            }

            // Calculate starting index that represents TARGET TYPE

            s += hits[0] + particles[0] + " "; //Zero Hits

            int j = tarType;

            for (int i = 1; i < hits.Count; i++)
            {
                s += hits[i].ToString() + particles[j] + " ";
                j++;
            }

            s += ")";

            RoundHits = s;
            Total = totalScore().ToString();
        }

        // Fill scores to ScoresP per round
        public void FillRScores(List<int> scores)
        {
            ScoresPRound.Add(new List<int>(scores));
        }

        // Enables repair
        public void RefillrScores(List<int> scores, int index)
        {
            ScoresPRound[index] = scores;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static bool operator <(Shooter s1, Shooter s2)
        {
            if (s1.totalScore() < s2.totalScore())
            {
                return true;
            }
            else if (s1.totalScore() == s2.totalScore())
            {
                return LTcheck(s1, s2);
            }
            else if (s1.totalScore() > s2.totalScore())
            {
                return false;
            }

            return false;
        }

        public static bool operator >(Shooter s1, Shooter s2)
        {
            return !(s1 < s2);
        }

        // Target hits 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
        // Less than check if total score same
        private static bool LTcheck(Shooter s1, Shooter s2)
        {
            List<int> lt1 = new List<int>();
            List<int> lt2 = new List<int>();

            for (int i = 0; i < s1.ScoresPRound.Count; i++)
            {
                for (int j = 0; j < s1.ScoresPRound[i].Count; j++)
                {
                    if (lt1.Count < j + 1)
                    {
                        lt1.Add(0);
                    }
                    lt1[j] += s1.ScoresPRound[i][j];
                }
            }
            for (int i = 0; i < s2.ScoresPRound.Count; i++)
            {
                for (int j = 0; j < s2.ScoresPRound[i].Count; j++)
                {
                    if (lt2.Count < j + 1)
                    {
                        lt2.Add(0);
                    }
                    lt2[j] += s2.ScoresPRound[i][j];
                }
            }
            for (int i = lt1.Count - 1; 0 < i; i--)
            {
                if (lt1[i] < lt2[i])
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
            return false;
        }
    }
}
