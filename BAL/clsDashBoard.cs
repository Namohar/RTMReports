using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using System.Data.SqlClient;

namespace BAL
{
    public class clsDashBoard
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        SqlDBHelper objDB = new SqlDBHelper();
        string sQuery;

        public DataSet GetTeamName(string _teamId, string dtName)
        {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId)  
           };
            sQuery = "select T_TeamName from RTM_Team_List where T_ID= @teamId";
            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            //ds = objDB.DBExecDataSet(sQuery, dtName);
            return ds;
        }

        public DataTable LoadClient(string _teamId, string fromDate, string toDate)
        {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@fromDate", fromDate),
               new SqlParameter("@toDate", toDate)
           };
           sQuery = "SELECT TOP 5 C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_TeamId=@teamId  and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @fromDate AND @toDate GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            //dt = objDB.DBExecDataTable(sQuery);
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataSet LoadUserCleints(string _userName, string fromDate, string toDate, string _preference, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@userName", _userName),
               new SqlParameter("@fromDate", fromDate),
               new SqlParameter("@toDate", toDate),
               new SqlParameter("@date", DateTime.Now.AddDays(-1).ToShortDateString()) 
           };
            if (_preference == "2")
            {
                sQuery = "SELECT TOP 5 C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_User_Name=@userName  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            }
            else
            {
                sQuery = "SELECT TOP 5 C.CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Client_List C where  B.R_Client = C.CL_ID and R_User_Name=@userName  and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @fromDate and @toDate GROUP BY C.CL_ClientName ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            }
            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            //ds = objDB.DBExecDataSet(sQuery, dtName);
            return ds;
        }

        public DataSet LoadUserTasks(string _access, string _team, string _userName, string fromDate, string toDate, DateTime _login, DateTime _logout, string _preference, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@userName", _userName),
               new SqlParameter("@fromDate", fromDate),
               new SqlParameter("@toDate", toDate),
               new SqlParameter("@teamId", _team),
               new SqlParameter("@login", _login),
               new SqlParameter("@logout", _logout),
               new SqlParameter("@date", DateTime.Now.AddDays(-1).ToShortDateString())
           };
            if (_access == "3" || _access == "2")
            {
                sQuery = "SELECT TOP 5 C.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_TeamId=@teamId  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @fromDate and @toDate GROUP BY C.TL_Task  ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
            }
            else
            {
                if (_preference == "2")
                {
                    sQuery = "SELECT TOP 5 C.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_User_Name=@userName  and R_Duration != 'HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date GROUP BY C.TL_Task ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
                }
                else
                {
                    sQuery = "SELECT TOP 5 C.TL_Task, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records B, RTM_Task_List C where  B.R_Task = C.TL_ID and R_User_Name=@userName  and R_Duration != 'HH:MM:SS' and R_Start_Date_Time BETWEEN @login and @logout GROUP BY C.TL_Task ORDER BY sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 DESC";
                }
            }

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            //ds = objDB.DBExecDataSet(sQuery, dtName);
            return ds;
        }

        public DataSet LastLogin1(string _preference, string _username, DateTime date, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@username", _username),
               new SqlParameter("@date",  date)
           };
            if (_preference == "2")
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @username and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) <= @date ORDER BY LA_ID DESC";
            }
            else
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @username and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) <= @date and LA_Log_Action = 'First Activity' ORDER BY LA_ID DESC";
            }

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;

        }

        public DataSet LastLogout1(string _preference, string _username, DateTime date, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
               new SqlParameter("@username", _username),
               new SqlParameter("@date",  date)
             };

            if (_preference == "2")
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @username and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date ORDER BY LA_ID";
            }
            else
            {
                sQuery = "SELECT TOP 1 LA_Start_Date_Time From RTM_Log_Actions where LA_User_Name = @username and LA_Start_Date_Time >= @date and LA_Log_Action = 'Last Activity' ORDER BY LA_ID";
            }

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;

        }
    }
}
