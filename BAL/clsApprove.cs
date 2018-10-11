using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;

namespace BAL
{
    public class clsApprove
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        string sQuery;

        public DataTable GetTotalWorkHoursForApproval(string type,string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };
            if (type == "user")
            {
                sQuery = "SELECT Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records WITH (NOLOCK) where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'";
            }
            else
            {
                sQuery = "SELECT R_User_Name, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records WITH (NOLOCK) where R_TeamId=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' group by R_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetApprovalStatus(string type, string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };
            if (type == "user")
            {
                sQuery = "SELECT R_SubmittedOn, R_ApprovedOn,R_Approve,R_Submit from RTM_Records WITH (NOLOCK) where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' group by R_SubmittedOn, R_ApprovedOn,R_Approve,R_Submit";
            }
            else
            {
                sQuery = "SELECT R_User_Name, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records WITH (NOLOCK) where R_TeamId=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' group by R_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalLogHoursForApproval(string type, string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };
            if (type == "user")
            {
                sQuery = "select Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds, LA_SubmittedOn, LA_ApprovedOn, LA_Approve,LA_Submit from RTM_Log_Actions WITH (NOLOCK) where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_User_Name =@user group by LA_SubmittedOn, LA_ApprovedOn, LA_Approve,LA_Submit";
            }
            else
            {
                sQuery = "select LA_User_Name, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK) where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_TeamId =@user group by LA_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalLeaveHoursForApproval(string type, string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            if (type == "user")
            {
                sQuery = "select sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as decimal) end) as LD_Duration, LD_SubmittedOn, LD_ApprovedOn, LD_Approve,LD_Submit  from RTM_LeaveDetails where LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to group by LD_SubmittedOn, LD_ApprovedOn, LD_Approve,LD_Submit";
            }
            else
            {
                sQuery = "select LD_UserName, sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as decimal) end) as LD_Duration  from RTM_LeaveDetails, RTM_User_List where LD_UserName= UL_User_Name and UL_Team_Id= @user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to group by LD_UserName";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalHoursforApproval(string type, string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@type", type),
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };          

            dt = objDB.ExecuteParamerizedSelectCommand("sp_GetDataForApproval", CommandType.StoredProcedure, parameters);
            return dt;
        }

        public DataTable GetTeam(string user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user)              
            };

            sQuery = "select UL_Team_Id, UL_EmailId from RTM_User_List where UL_User_Name=@user";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }
    }
}
