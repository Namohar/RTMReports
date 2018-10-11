using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace BAL
{
    public class clsClock
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        bool result;
        string sQuery;

        public int AddClockIn(int id, string teamId, string empId, string userName, int client, int task, int subTask, string comments, string duration, DateTime startTime, string status)
        {
            int rId = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["conString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("sp_AddClockIn", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@TeamId", teamId);
                cmd.Parameters.AddWithValue("@EmployeeId", empId);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Client", client);
                cmd.Parameters.AddWithValue("@Task", task);
                cmd.Parameters.AddWithValue("@SubTask", subTask);
                cmd.Parameters.AddWithValue("@Comments", comments);
                cmd.Parameters.AddWithValue("@Duration", duration);
                cmd.Parameters.AddWithValue("@StartDateTime", startTime);
                cmd.Parameters.AddWithValue("@Status",status);
                cmd.Parameters.AddWithValue("@R_System", "PC");
                cmd.Parameters.AddWithValue("@R_Approve", "0");

                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@result";
                outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                cmd.ExecuteNonQuery();

                string result = outPutParameter.Value.ToString();

                if (!string.IsNullOrEmpty(result))
                {
                    rId = Convert.ToInt32(result);
                }
                else
                {
                    rId = 0;
                }
            }
            return rId;
        }

        public DataTable getClickInTask(string user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user) 
            };

            sQuery = "select * from RTM_Records, RTM_Client_List where R_Client=  CL_ID and R_User_Name =@user and R_Status='Running' and R_System='PC'";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool AddClockOut(string recId, DateTime endTime, string duration, string comments)
        {
            
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@recId", recId),
               new SqlParameter("@endTime", endTime),
               new SqlParameter("@duration", duration),
               new SqlParameter("@comments", comments)
            };

            sQuery = "update RTM_Records set R_Duration=@duration, R_CreatedOn=@endTime, R_Status ='Completed', R_Comments=@comments where R_ID =@recId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool UpdateTaskSubTask(string recId, string taskId, string subTaskId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@recId", recId),
               new SqlParameter("@taskId", taskId),
               new SqlParameter("@subTaskId", subTaskId)               
            };

            sQuery = "update RTM_Records set R_Task=@taskId, R_SubTask=@subTaskId where R_ID =@recId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable PastChoices(string user, string date, string teamId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@date", date),
               new SqlParameter("@teamId", teamId)
            };

            sQuery = "select CL_ID, CL_ClientName, TL_Task, STL_SubTask from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where R_User_Name =@user and R_Start_Date_Time >=@date and R_System ='PC' and R_TeamId=@teamId group by CL_ID, CL_ClientName, TL_Task, STL_SubTask order by CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable PastTaskSelection(string user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user)
               
            };

            sQuery = "select top 1 R_Task, R_SubTask from RTM_Records where R_User_Name =@user order by R_Start_Date_Time desc";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable getTodaysHours(string user, string date)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@date", date)
            };

            sQuery = "SELECT Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_Duration != 'HH:MM:SS'";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable getWeeksHours(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            sQuery = "SELECT Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }


    }
}
