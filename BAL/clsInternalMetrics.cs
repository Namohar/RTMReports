using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using System.Data.SqlClient;

namespace BAL
{
    public class clsInternalMetrics
    {
        public DataTable fetchAllTeams(string from, string to)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select T_ID, T_TeamName from RTM_Records left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to Group By T_ID, T_TeamName order by T_TeamName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchAllClients(string from, string to)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct CL_ClientName from RTM_Records left join RTM_Client_List on R_Client = CL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to order by CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchAllUsers(string from, string to)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct R_User_Name from RTM_Records where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to order by R_User_Name";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchAllTasks(string from, string to)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct STL_ServiceCode from RTM_Records left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to order by STL_ServiceCode";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchTeams(string from, string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select T_ID, T_TeamName from RTM_Records left join RTM_Team_List on R_TeamId = T_ID left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to " + sb.ToString() + " Group By T_ID, T_TeamName order by T_TeamName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchClients(string from, string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct CL_ClientName from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Team_List on R_TeamId = T_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to " + sb.ToString() + " order by CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }



        public DataTable fetchUsers(string from , string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct R_User_Name from RTM_Records left join RTM_Team_List on R_TeamId = T_ID left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to " + sb.ToString() + " order by R_User_Name";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchTasks(string from, string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select Distinct STL_ServiceCode from RTM_Records left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Team_List on R_TeamId = T_ID left join RTM_Client_List on R_Client = CL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to " + sb.ToString() + " order by STL_ServiceCode";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable DisplayData(string from, string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select T_TeamName, CL_ClientName,R_User_Name, STL_ServiceCode, CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) +':'+ CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) +':'+ CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as [duration] from RTM_Records left join RTM_Team_List on R_TeamId = T_ID left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' " + sb.ToString() + " Group By T_TeamName, CL_ClientName,R_User_Name, STL_ServiceCode Order By T_TeamName, R_User_Name";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetTotalDuration(string from, string to, StringBuilder sb)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
            string sQuery = "select CONVERT(varchar(10), sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) +':'+ CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) +':'+ CONVERT(varchar(10),(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as [duration] from RTM_Records left join RTM_Team_List on R_TeamId = T_ID left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' " + sb.ToString() + " ";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

    }
}
