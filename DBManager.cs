using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace ShootingCompetition
{
    public class DBManager
    {
        // Url for database
        public string ConnString { get; }

        // Library of sql statements
        public Dictionary<string, string> sqlLibrary;

        public DBManager()
        {
            ConnString = @"Data Source=" + Directory.GetCurrentDirectory() + @"\CompetitionDB.db";
            sqlLibrary = FillSqlLibrary();
        }

        // Fill sql library
        private Dictionary<string, string> FillSqlLibrary()
        {
            return new Dictionary<string, string>()
            {
                // Select statements
                {"selectShooters", "SELECT s.id, s.name, s.surname, s.clubid, c.cname, s.team, s.note FROM Shooters s LEFT OUTER JOIN Clubs c ON s.clubid=c.id;"},
                {"selectPlaces", "SELECT * FROM Places;"},
                {"selectRanges", "SELECT r.*, p.pname FROM Ranges r LEFT OUTER JOIN Places p ON r.placeid=p.id;"},
                {"selectClubs", "SELECT c.*, p.pname FROM Clubs c LEFT OUTER JOIN Places p ON c.placeid=p.id;"},
                {"selectPCompetitions", "SELECT * FROM PCompetitions;"},
                
                // Delete statements
                {"deleteShooter", "DELETE FROM Shooters WHERE id=@id"},
                {"deleteClub", "DELETE FROM Clubs WHERE id=@id;"},

                //Complete event deletion
                {"deletePCompetition", "DELETE FROM PCompetitions WHERE id=@id;"},
                {"deletePResults", "DELETE FROM PResults WHERE competitionid=@id;"},
                {"deletePShots", "DELETE FROM PShots WHERE id=@id;"},
                {"selectPShots", "SELECT ps.id FROM PResults pr INNER JOIN PShots ps ON pr.shotsid = ps.id WHERE competitionid = @sid;"},

                { "deleteRange", "DELETE FROM Ranges WHERE id=@id;"},
                {"deletePlace", "DELETE FROM Places WHERE id=@id;"},

                // Insert statements
                {"insertClub", "INSERT INTO Clubs (cname, placeid) VALUES (@cname, @placeid);"},
                {"insertPCompetition", "INSERT INTO PCompetitions (title, rangeid, date, tartype, shots, rounds, finished) VALUES (@title, @rangeid, @date, @tartype, @shots, @rounds, @finished);"},
                {"insertRange", "INSERT INTO Ranges (rname, placeid) VALUES (@rname, @placeid);"},
                {"insertPlace", "INSERT INTO Places (id, pname) VALUES (@id, @pname);"},
                {"insertShooter", "INSERT INTO Shooters(name, surname, note, clubid, team) VALUES (@name, @surname, @note, @clubid, @team);"},

                // Update statements
                {"updateClub", "UPDATE Clubs SET cname=@cname, placeid=@placeid WHERE id=@id;"},
                {"updatePCompetition", "UPDATE PCompetitions SET title=@title, rangeid=@rangeid, date=@date, tartype=@tartype, shots=@shots, rounds=@rounds WHERE id=@id;"},
                {"updateRange","UPDATE Ranges SET rname=@rname, placeid=@placeid WHERE id=@id;"},
                {"updatePlace", "UPDATE Places SET pname=@pname WHERE id=@id;"},
                {"updateShooter","UPDATE Shooters SET name=@name, surname=@surname, note=@note, clubid=@clubid, team=@team WHERE id=@id;"},

                //Select for LoadPreciseWindow
                {"selectComBox", "SELECT co.*, ra.rname, ra.placeid, p.pname FROM PCompetitions co INNER JOIN Ranges ra ON co.rangeid=ra.id INNER JOIN Places p ON ra.placeid=p.id;"},

                //Select for competitors list view
                {"selectLVComp",  "SELECT s.id, s.name, s.surname, c.cname, s.team FROM Shooters s INNER JOIN Clubs c ON s.clubid=c.id;"},

                //Search LVComp
                {"serchLVComp", "SELECT s.id, s.name, s.surname, c.cname, s.team FROM Shooters s INNER JOIN Clubs c ON s.clubid=c.id WHERE "},

                //DB Data magic at the end
                {"updateEndGame", "UPDATE PCompetitions SET finished=@finished WHERE id=@id;"},
                {"insertPresults", "INSERT INTO PResults(competitionid, shooterid, shotsid, score, team) VALUES (@competitionid, @shooterid, @shotsid, @score, @team);"},
                {"insertShots", "INSERT INTO PShots (fieldzero, fieldone, fieldtwo, fieldthree, fieldfour, fieldfive, fieldsix, fieldseven, fieldeight, fieldnine, fieldten, fieldeleven) VALUES (@fieldzero, @fieldone, @fieldtwo, @fieldthree, @fieldfour, @fieldfive, @fieldsix, @fieldseven, @fieldeight, @fieldnine, @fieldten, @fieldeleven);"},
                {"selectShoID", "SELECT MAX(id) maxid FROM PShots;"},

                //DB load to statistics
                {"selectForStatistics", "SELECT sh.id, sh.name, sh.surname, cl.cname, pr.team, pr.score, ps.fieldzero, ps.fieldone, ps.fieldtwo, ps.fieldthree, ps.fieldfour, ps.fieldfive, ps.fieldsix, ps.fieldseven, ps.fieldeight, ps.fieldnine, ps.fieldten, ps.fieldeleven FROM Shooters sh INNER JOIN Clubs cl ON sh.clubid = cl.id INNER JOIN PResults pr ON sh.id = pr.shooterid INNER JOIN PShots ps ON pr.shotsid = ps.id WHERE pr.competitionid = @cid;"}
                         };
        }
        // Create and return DataView from querry
        public DataView GetView(string querry, string tableName)
        {
            DataTable table = new DataTable(tableName);
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(querry, ConnString))
            {
                adapter.Fill(table);
            }
            return table.AsDataView();
        }

        // Get database connection
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnString);
        }
    }
}
