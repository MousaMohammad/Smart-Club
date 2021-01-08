﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace GUIPROJECT
{
    class MemberController
    {
        DBManager dbMan;
        public MemberController()
        {
            dbMan = new DBManager();
        }


        public void TerminateConnection()
        {
            dbMan.CloseConnection();
        }

        public int Addevent(string name, string place, string date, int fees, int memberid)
        {
            string query = "Insert into Events values('" + name + "','" + place + "','" + date + "',null," + fees + "," + memberid + ",'Pending')";
            return dbMan.ExecuteNonQuery(query);
        }

        public int ParkingSubscribe(string startdate, int fees, string enddate, int memberid)
        {
            string query = "Insert into Parking values(" + memberid + ",'" + startdate + "'," + fees + ",'" + enddate + "','Pending')";
            return dbMan.ExecuteNonQuery(query);
        }

        public DataTable GetAllActivityNames()
        {
            string query = "select Name from Activites";
            return dbMan.ExecuteReader(query);
        }

        public DataTable GetPlaceOfCertainActivity(string x)
        {
            string query = "select place from Activites where Name='" + x + "' ;";
            return dbMan.ExecuteReader(query);
        }



        // we get coaches from RELATION:COACHES NOT from relation including Acttivity,Team,Employee 
        // as we need all coaches even not coaching a team as we may need to constrcut a new team
        // if no such team in a certain activity

        public DataTable GetCoachesofCertainActivity(string x)
        {
            string query = "select fname from Coaches join Activites on Activity_ID=Activites.ID join employee on employee.SSN=Coaches.SSN where " +
                " Activites.Name='" + x + "' ;";
            return dbMan.ExecuteReader(query);
        }

        public DataTable GetTeamsofCertainActivityandCoach(string actname, string empname)
        {
            string query = "select Team_ID from Team_Activity join Activites on Activity_ID=Activites.ID  join employee on employee.SSN=Team_Activity.SSN "
            + "where Activites.Name='" + actname + "' and employee.Fname='" + empname + "' ;";
            return dbMan.ExecuteReader(query);
        }

        public DataTable GetCoachesofCertainActivityandteams(string actname, string teamid)
        {
            string query = "select employee.fname from Team_Activity join Activites on Activity_ID=Activites.ID  join employee on employee.SSN=Team_Activity.SSN "
            + "where Activites.Name='" + actname + "' and Team_ID=" + int.Parse(teamid) + " ;";
            return dbMan.ExecuteReader(query);
        }

        public DataTable GetTeamsofCertainActivity(string x)
        {
            string query = "select Team_ID from Team_Activity join Activites on Activity_ID=Activites.ID where " +
                " Activites.Name='" + x + "' ;";
            return dbMan.ExecuteReader(query);
        }

        public DataTable SelectAllofEmpbyusername(string us)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("@username", us);
            string StoredProcedureName = StoredProcedures.EMPLOYEEBYUSERNAME;
            return dbMan.ExecuteReader(StoredProcedureName, Parameters);
        }

        public int EnrollMemberinTeam(string memberid, string teamid)
        {
            string query = "Insert into Members_Teams Values(" + int.Parse(memberid) + "," + int.Parse(teamid) + ") ; ";
            return dbMan.ExecuteNonQuery(query);
        }

 
        public int CreateNewTeamforaCertainActivity(string trainingtime)
        {
            // will initialize team level and size by 1
            // and we inserted null for id as its auto made
            string query = "Insert into Teams Values('" + trainingtime + "',1,1);";
            return dbMan.ExecuteNonQuery(query);
        }
        public string get_team_id(string trainingtime, string size, string level)
        {
            string query = "select ID from Teams where Training_Time='" + trainingtime + "' and Size = " + size +
                " and level=" + level + " ; ";
            return ((int)dbMan.ExecuteReader(query).Rows[0][0]).ToString();
        }

        public int insertinTeamActivityCoach(string teamid, string coachname, string activityname)
        {
            // getting activityID From ActivityName
            string actid = "select Id from Activites where Name='" + activityname + "' ; ";
            actid = ((int)dbMan.ExecuteReader(actid).Rows[0][0]).ToString();

            // getting CoachId (SSN)
            string coach_id = "select Employee.SSN from Coaches join Employee on Employee.SSN=Coaches.SSN where Activity_ID=" +
                actid + " and Employee.Fname='" + coachname + "' ; ";
            coach_id = ((int)dbMan.ExecuteReader(coach_id).Rows[0][0]).ToString();


            string query = "Insert into Team_Activity Values(" + int.Parse(teamid) + "," +
            int.Parse(coach_id) + "," + int.Parse(actid) + ") ; ";
            return dbMan.ExecuteNonQuery(query);
        }



        public DataTable GetActivityofanmember(string memberid)
        {
            string query = "select  distinct Activites.Name " +
          " from Team_Activity join Activites on Activity_ID=Activites.ID join Members_Teams on Members_Teams.Team_ID=Team_Activity.Team_ID"
          + " where Member_ID=" + memberid + ";";
            return dbMan.ExecuteReader(query);
        }


        // Mustafaa's functions
        public string get_enddate(int memberid)
        {
            string query = "select CONVERT(VARCHAR(10),End_Date,101) from Members where ID=3;";//id will be adjusted when we do the login
            return (dbMan.ExecuteReader(query).Rows[0][0]).ToString();
        }

        public int Update_Membership(string enddate)
        {

            DateTime d = Convert.ToDateTime(enddate);
            d = d.AddYears(1);

            string query = "Update Members Set End_Date='" + d + "' where ID=1;"; //id will be adjusted when we do the login
            return dbMan.ExecuteNonQuery(query);
        }
        public int Terminate_Membership(string enddate)
        {

            DateTime d = DateTime.Today;
            string query = "Update Members Set End_Date='" + d + "',Status='Terminated' where ID=3;"; //id will be adjusted when we do the login
            return dbMan.ExecuteNonQuery(query);
        }

        public DataTable Selectteam()
        {
            string query = "SELECT ID FROM Teams;";
            return dbMan.ExecuteReader(query);
        }

        public DataTable selectNfromteam(int teamid) // UnComplete
        {

            string query = "SELECT distinct Fname, Lname From[Smart_Club].[dbo].[Members] join[Smart_Club].[dbo].[Members_Teams] on Members.ID = Members_Teams.Member_ID where Team_ID="+teamid+";";
            return dbMan.ExecuteReader(query);
        }
        public int turntomember(string Fname, string Lname, string sex, int age)
        {
            string query = "Insert into Members (Fname,Lname,Age,Sex,Start_Date,End_Date,MemberShip_Price,Status,Username)" +
                "values ('" + Fname+"','"+Lname+"',"+age+",'"+sex+ "',null,null,null,'pending',null)";
            return dbMan.ExecuteNonQuery(query);

        }
        public DataTable selectreviews() // UnComplete
        {
            string query = "select Comment, Rating from Reviews; ";
            return dbMan.ExecuteReader(query);
        }

        //
        // ahmed ehab new 

        public string GetActivityidFromName(string actname)
        {
            string query = "select ID from Activites Where Name='" + actname + "' ;";
            return ((int)dbMan.ExecuteReader(query).Rows[0][0]).ToString();
        }

        public DataTable GetTeamsOfaMemberInaCertainActivity(string memberid, string activityid)
        {

            string query = "select Members_teams.Team_ID" +
            " from Members_teams join Team_Activity on Team_Activity.Team_ID=Members_teams.Team_ID" +
            " where Member_ID=" + memberid + " and activity_id=" + activityid + ";";
            return dbMan.ExecuteReader(query);
        }

        public int DropMefromCertainTeam(string memberid, string teamid)
        {
            string query = "delete from Members_teams" +
                            " where Member_ID=" + memberid + " and Team_ID=" + teamid + " ; ";
            return dbMan.ExecuteNonQuery(query);
        }

        public DataTable GetTrainingTimeofTeam(string teamid)
        {
            string query = "select Training_Time from teams where ID=" + teamid + " ; ";
            return dbMan.ExecuteReader(query);
        }


        public int changeTrainingTimeofTeam(string teamid, string training_time)
        {
            string query = "UPDATE teams SET Training_Time='" + training_time +
                           "' WHERE id=" + teamid + " ; ";
            return dbMan.ExecuteNonQuery(query);
        }


        public DataTable GetLevelofTeam(string teamid)
        {
            string query = "select level from teams where id=" + teamid + " ; ";
            return dbMan.ExecuteReader(query);
        }

        public int likeActivity(string actname)
        {
            string query = " Update Activites" +
                            " Set likes = likes + 1" +
                            " Where Name='" + actname + "' ; ";
            return dbMan.ExecuteNonQuery(query);
        }


        public DataTable GetReviewsOfCertainActivity(string actname)
        {
          string query ="select Reviews.*"+
                       " from Reviews join Activites_Reviews on Activites_Reviews.Review_ID=Reviews.Review_ID join Activites on ID=Activites_Reviews.Activity_ID"
                      + " where Activites.Name='" + actname + "' ; ";
          return dbMan.ExecuteReader(query);
        }

        public DataTable GetLikesOfActivity(string actname)
        {
            string query = "select likes from activites where name='" + actname + "' ; ";
            return dbMan.ExecuteReader(query);
        }

        public int insertaReview(string comment, int rating, string date)
        {
            string query = "insert into Reviews" +
                          " values ('" + comment + "'," + rating + "," + date + ");";
            return dbMan.ExecuteNonQuery(query);
        }

        public string GetReviewID(string comment, int rating,string date)
        {
        string query="select Review_id"+
                     " from Reviews "+
                      " where comment like '"+comment+"' and rating="+rating+" and date="+date+" ;" ;
        return ((int)dbMan.ExecuteReader(query).Rows[0][0]).ToString();
        }

        public int insertintoActivityReview(int reviewID, int ActId,int MemberID)
        {
            string query = "insert into Activites_Reviews" +
                          " values (" + MemberID + "," + reviewID+ "," + ActId+ ");";
            return dbMan.ExecuteNonQuery(query);
        }

        public int GetTeamSize(string teamid)
        {
            string query = "select size from Teams where ID=" + Convert.ToInt32(teamid) + " ; ";
            return ((int)dbMan.ExecuteReader(query).Rows[0][0]);
        }

        public int GetTeamActualSize(string teamid)
        {
            string query = "select count(*) from Members_Teams where Team_ID=" + Convert.ToInt32(teamid) + " ; ";
            return ((int)dbMan.ExecuteReader(query).Rows[0][0]);
        }

        public int EnrollGuestinTeam(string guestid, string teamid)
        {
            string query = "Update Guest Set Team_ID =" + int.Parse(teamid) + " where SSN=" + int.Parse(guestid) + " ; ";
            return dbMan.ExecuteNonQuery(query);
        }

        public DataTable GetGuestTeam( string guestid){
              string query = "Select Team_ID FROM GUEST WHERE SSN="+int.Parse(guestid)+";" ;
              return dbMan.ExecuteReader(query);
        }
    }
}
