using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace ShootingCompetition
{
    public class PreciseShooting
    {
        public ObservableCollection<Shooter> Shooters { get; set; }

        public List<Team> Teams { get; set; }

        public int[] Scoring { get; }

        // General competition data
        public long Id { get; set; }
        public string Title { get; set; }
        public string Range { get; set; }        
        public DateTime CDate { get; set; }

        // Target type and number of shots per round
        public int TarType { get; set; }
        public int Shots { get; set; }

        // How many rounds (1, 2, ...?) to compete
        public int NRounds { get; set; }

        //Current round
        public int Round { get; set; }

        // Competition started prevents any further editing
        public bool Started { get; set; }

        // Create Competition constructor
        public PreciseShooting(long id, string title, string range, DateTime cDate, int tarType, int shots, int nRounds)
        {
            Shooters = new ObservableCollection<Shooter>();
            Id = id;
            Title = title;
            Range = range;
            CDate = cDate;
            TarType = tarType;
            Shots = shots;
            NRounds = nRounds;
            Round = 1;
            Started = false;
            Scoring = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10 };
            Teams = new List<Team>();
        }

        // Ad shooter to observable collection
        public void AddShooter(Shooter shooter)
        {
            Shooters.Add(shooter);
        }

        // Removes shooter by id
        public bool RemoveShooter(long id)
        {
            for (int i = 0; i < Shooters.Count; i++)
            {
                if (Shooters[i].Id == id)
                {
                    Shooters.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        //LINQ CREATE TEAMS
        public void GetTeams()
        {
            var teamQuerry = (from s in Shooters
                              select s.Team).Distinct();

            foreach (string team in teamQuerry)
            {
                var shootaz = from shoota in Shooters
                              where shoota.Team == team
                              select shoota;
                // Create team and add shooters
                Team tmpTeam = new Team(team);
                tmpTeam.Shooters = shootaz.ToList<Shooter>();
                Teams.Add(tmpTeam);
            }

            //Bublesort Sort Teams
            Team sTeam;
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int j = i + 1; j < Teams.Count; j++)
                {
                    if (Teams[i] < Teams[j])
                    {
                        sTeam = Teams[i];
                        Teams[i] = Teams[j];
                        Teams[j] = sTeam;
                    }
                }
            }
        }

        // Bublesort :)
        public void EndCompetition()
        {
            Shooter shoo;

            for (int i = 0; i < Shooters.Count; i++)
            {
                for (int j = i + 1; j < Shooters.Count; j++)
                {
                    if (Shooters[i] < Shooters[j])
                    {
                        shoo = Shooters[i];
                        Shooters[i] = Shooters[j];
                        Shooters[j] = shoo;
                    }
                }
            }
            //Create Team list
            GetTeams();
        }
    }
}
