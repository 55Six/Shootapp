using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingCompetition
{
    public class Team
    {
        public string TeamName { get; set; }
        public int TotalScore { get; set; }
        public List<int> GroupShots { get; set; }

        private static string[] particles = new string[] { "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "x11" };

        // On Add list auto sort and sum total points
        List<Shooter> shooters;
        public List<Shooter> Shooters
        {
            get
            {
                return shooters;
            }
            set
            {
                Shooter shoo;
                shooters = value;
                int tot = 0;
                for (int i = 0; i < shooters.Count; i++)
                {
                    tot += shooters[i].totalScore();
                    // Ustvari group shots za prikaz
                    if (GroupShots == null)
                    {
                        GroupShots = shooters[i].GetShoTotal();
                    }
                    else
                    {
                        List<int> tmps = shooters[i].GetShoTotal();
                        for (int j = 0; j < tmps.Count; j++)
                            GroupShots[j] += tmps[j];
                    }
                    for (int j = i + 1; j < shooters.Count; j++)
                    {
                        if (shooters[i] < shooters[j])
                        {
                            shoo = shooters[i];
                            shooters[i] = shooters[j];
                            shooters[j] = shoo;
                        }
                    }
                }
                TotalScore = tot;
            }
        }

        public Team(string teamName)
        {
            TeamName = teamName;
        }

        // Prints team with Shooters
        public string PrintTeam(int tarType)
        {
            string str = TeamName + " : (";
            str += GroupShots[0] + particles[0] + " "; //Zero Hits
            int j = tarType;

            for (int i = 1; i < GroupShots.Count; i++)
            {
                str += GroupShots[i].ToString() + particles[j] + " ";
                j++;
            }
            str += ")\n\t";

            for (int i = 0; i < Shooters.Count; i++)
            {
                str += Shooters[i].Name + " " + Shooters[i].Surname + " : (" + Shooters[i].totalScore() + ")\n";
                str += "\t";
            }
            return str;
        }

        //Les then operator for bublesort Team
        public static bool operator <(Team t1, Team t2)
        {
            if (t1.TotalScore < t2.TotalScore)
            {
                return true;
            }
            else if (t1.TotalScore == t2.TotalScore)
            {
                return LTcheck(t1, t2);
            }
            else if (t1.TotalScore > t2.TotalScore)
            {
                return false;
            }

            return false;
        }

        public static bool operator >(Team t1, Team t2)
        {
            return !(t1 < t2);

        }

        //Checks if more than so you can sort
        private static bool LTcheck(Team t1, Team t2)
        {
            for (int i = t1.GroupShots.Count - 1; 0 < i; i--)
            {
                if (t1.GroupShots[i] < t2.GroupShots[i])
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
