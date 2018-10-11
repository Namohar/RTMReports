using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;

namespace BAL
{

    public class clsRecords
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        bool result;
        string sQuery;
        public DataTable GetData(string date, string empId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@date", date),
                new SqlParameter("@empId", empId)
            };

            sQuery = "select * from RTM_Records WITH (NOLOCK) where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) = @date and R_Employee_Id=@empId";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetLastWeekData(string from, string to, string empId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@empId", empId)
            };

            sQuery = "select * from RTM_Records WITH (NOLOCK) where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_Employee_Id=@empId";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable CopyLastWeekData(string from, string to, string empId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@empId", empId)
            };

            sQuery = "select R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, DATEADD(day,7,R_TimeDate) as R_Start_Date_Time, DATEADD(day,7,R_CreatedOn) as R_CreatedOn,R_Status from RTM_Records WITH (NOLOCK) where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) between @from and @to and R_Employee_Id =@empId order By R_TimeDate";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool DeleteData(string date, string empId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@date", date),
                new SqlParameter("@empId", empId)
            };

            sQuery = "delete from  RTM_Records where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) = @date and R_Employee_Id=@empId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool DeleteWeekData(string from, string to, string empId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@empId", empId)
            };

            sQuery = "delete from  RTM_Records where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) between @from and @to and R_Employee_Id=@empId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool DeleteRecord(string recId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@recId", recId)
            };

            sQuery = "delete from  RTM_Records where R_ID=@recId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool AddRecord(string teamId, string empId, string userName, string client, string task, string subTask, string duration, string comments, string date, string createdDate, string status, string system)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@userName", userName),
                new SqlParameter("@empId", empId),
                new SqlParameter("@client",client),
                new SqlParameter("@task",task),
                new SqlParameter("@subTask",subTask),
                new SqlParameter("@duration",duration),
                new SqlParameter("@comments",comments),
                new SqlParameter("@date",date),
                new SqlParameter("@createdDate",createdDate),
                new SqlParameter("@status",status),
                new SqlParameter("@system",system),
                new SqlParameter("@timedate", date)
            };

            sQuery = "insert into RTM_Records (R_TeamId, R_Employee_Id, R_User_Name, R_Client, R_Task, R_SubTask, R_Comments, R_Duration, R_Start_Date_Time, R_CreatedOn,R_Status, R_System, R_TimeDate) values (@teamId, @empId,@userName, @client, @task, @subTask, @comments, @duration, @date, @createdDate, @status, @system, @timedate )";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        //Submit

        public bool SubmitRecordsData(string user, string from, string to, string submittedBy, string submittedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@submittedBy", submittedBy),
                new SqlParameter("@submittedOn", submittedOn),
            };

            sQuery = "Update RTM_Records set R_Submit =1 , R_SubmittedBy=@submittedBy, R_SubmittedOn=@submittedOn where R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) <= @to and (R_Submit is null or R_Submit =0 or R_Submit ='')";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool SubmitLogsData(string user, string from, string to, string submittedBy, string submittedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@submittedBy", submittedBy),
                new SqlParameter("@submittedOn", submittedOn),
            };

            sQuery = "Update RTM_Log_Actions set LA_Submit =1 , LA_SubmittedBy=@submittedBy, LA_SubmittedOn=@submittedOn where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) <= @to and (LA_Submit is null or LA_Submit =0 or LA_Submit ='')";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool SubmitLeavesData(string user, string from, string to, string submittedBy, string submittedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@submittedBy", submittedBy),
                new SqlParameter("@submittedOn", submittedOn),
            };

            sQuery = "Update RTM_LeaveDetails set LD_Submit =1 , LD_SubmittedBy=@submittedBy, LD_SubmittedOn=@submittedOn where LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) <= @to and (LD_Submit is null or LD_Submit =0 or LD_Submit ='')";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable CheckSubmitStatus(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)                
            };

            sQuery = "select Distinct R_Submit, R_Approve from RTM_Records WITH (NOLOCK) where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable CheckPastRecords(string user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user)
                       
            };

            sQuery = "select Top 1 R_User_Name from RTM_Records WITH (NOLOCK) where R_User_Name=@user";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }        

        public DataTable CheckLastWeekSubmitStatus(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)                
            };

            sQuery = "select Distinct R_Submit from RTM_Records WITH (NOLOCK) where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable CheckLeaveSubmitStatus(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)                
            };
            sQuery = "select Distinct LD_Submit from RTM_LeaveDetails WITH (NOLOCK) where LD_UserName=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        //Approve

        public bool ApproveRecordsData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_Records set R_Submit =1, R_Approve =1 , R_ApprovedBy=@approvedBy, R_ApprovedOn=@approvedOn where R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool ApproveLogsData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_Log_Actions set LA_Submit =1 , LA_Approve=1, LA_ApprovedBy=@approvedBy, LA_ApprovedOn=@approvedOn where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool UpdateSubmittedOn(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };
            sQuery = "Update RTM_Records set R_SubmittedOn = R_ApprovedOn where R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_Submit = 1 and R_SubmittedOn is Null";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool ApproveLeavesData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_LeaveDetails set LD_Submit =1 ,LD_Approve=1, LD_ApprovedBy=@approvedBy, LD_ApprovedOn=@approvedOn where LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        //Reject

        public bool RejectRecordsData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_Records set R_Submit =0, R_Approve =0 , R_RejectedBy=@approvedBy, R_RejectedOn=@approvedOn where R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool RejectLogsData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_Log_Actions set LA_Submit =0 , LA_Approve=0, LA_RejectedBy=@approvedBy, LA_RejectedOn=@approvedOn where LA_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool RejectLeavesData(string user, string from, string to, string approvedBy, string approvedOn)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@approvedBy", approvedBy),
                new SqlParameter("@approvedOn", approvedOn),
            };

            sQuery = "Update RTM_LeaveDetails set LD_Submit =0 ,LD_Approve=0, LD_RejectedBy=@approvedBy, LD_RejectedOn=@approvedOn where LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) Between @from and @to";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable GetTaskAndSubtask(string teamId, string subtask)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@subtask",subtask)
            };

            sQuery = "select TL_TeamId, TL_ID, STL_ID, TL_Task, STL_Subtask from RTM_Task_List left join RTM_SubTask_List on TL_ID = STL_Task_Id where STL_SubTask LIKE '%' + @subtask + '%'  and TL_TeamId = @teamId";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool DeleteLog(string logId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@logId", logId)
            };

            sQuery = "Delete from RTM_Log_Actions where LA_ID = @logId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable GetLeaves(string user, string from, string to)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            sQuery = "select CASE CL_ClientName WHEN 'Personal/Sick Time' THEN 1 WHEN 'Vacation' THEN 1 WHEN 'Maternity Leave' THEN 3 ELSE 4 END as CL_ClientName, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [Duration], R_TimeDate from RTM_Records WITH (NOLOCK) " +
                    "left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID "+
                    "where R_User_Name =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to " +
                    "and R_Task =0 and (CL_ClientName = 'Personal/Sick Time' or CL_ClientName ='Vacation' or CL_ClientName ='Maternity Leave' or CL_ClientName ='Paternity Leave') " +
                    "Group by CL_ClientName, R_TimeDate "+
                    "HAVING SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) >= 4";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        //Get Tasks
        public DataTable GetTasks(int teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeamId", teamId)
            };
            
            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetTask", CommandType.StoredProcedure, parameters);
            return dt;
        }

        //Get Subtasks
        public DataTable GetSubTasks(int taskId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskId", taskId)
            };
            
            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetSubTasks", CommandType.StoredProcedure, parameters);
            return dt;
        }

        //Get Clients
        public DataTable GetClients(int teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeamId", teamId)
            };

            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetClientList", CommandType.StoredProcedure, parameters);
            return dt;
        }

        public DataTable GetClientTasks(int teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeamId", teamId)
            };
            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetClientTasks", CommandType.StoredProcedure, parameters);
            return dt;
        }

        public DataTable GetGeneralTasks(int teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TeamId", teamId)
            };
            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetGeneralTasks", CommandType.StoredProcedure, parameters);
            return dt;
        }

        public DataTable GetClientSubTasks(int taskId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskId", taskId)
            };

            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetClientSubTasks", CommandType.StoredProcedure, parameters);
            return dt;
        }

        public DataTable GetGeneralSubTasks(int taskId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TaskId", taskId)
            };

            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetGeneralSubTasks", CommandType.StoredProcedure, parameters);
            return dt;
        }
    }
}
