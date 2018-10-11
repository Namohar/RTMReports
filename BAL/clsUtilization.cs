using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using System.Data.SqlClient;

namespace BAL
{
   public class clsUtilization
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        string sQuery;
       public int GetNumberOfWorkingDays(DateTime start, DateTime stop)
       {
           TimeSpan interval = stop - start;
           int totalWorkingDays;
           checked
           {
               int totalWeek = interval.Days / 7;
               totalWorkingDays = 5 * totalWeek;

               int remainingDays = interval.Days % 7;


               for (int i = 0; i <= remainingDays; i++)
               {
                   DayOfWeek test = (DayOfWeek)(((int)start.DayOfWeek + i) % 7);
                   if (test >= DayOfWeek.Monday && test <= DayOfWeek.Friday)
                       totalWorkingDays++;
               }
           }
           

           return totalWorkingDays;
       }

       public DataTable DetailedReport(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "SELECT CONVERT(VARCHAR(12), R_Start_Date_Time, 101) as [Date], R_User_Name as [User], CL_ClientName as [Client], TL_Task as [Task] ,STL_SubTask as [Sub Task],Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as Duration from RTM_Records , RTM_Client_List, RTM_SubTask_List, RTM_Task_List where R_Client = CL_ID and R_SubTask = STL_ID and R_Task = TL_ID and R_User_Name=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'  GROUP BY CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name,CL_ClientName, TL_Task, STL_SubTask Order By CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name";
           }
           else
           {
               sQuery = "SELECT CONVERT(VARCHAR(12), R_Start_Date_Time, 101) as [Date], R_User_Name as [User], CL_ClientName as [Client], TL_Task as [Task] ,STL_SubTask as [Sub Task],Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as Duration from RTM_Records , RTM_Client_List, RTM_SubTask_List, RTM_Task_List where R_Client = CL_ID and R_SubTask = STL_ID and R_Task = TL_ID and R_TeamId=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'  GROUP BY CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name,CL_ClientName, TL_Task, STL_SubTask Order By CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable DetailedReportNew(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], CL_ClientName as [Client], CL_Code, TL_Task as [Task] ,STL_SubTask as [Sub Task], COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as Duration, R_Comments from RTM_Records WITH (NOLOCK) left join  RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID where R_User_Name=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'  GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,CL_ClientName, TL_Task, STL_SubTask, R_Comments, CL_Code Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
           }
           else
           {
               sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], CL_ClientName as [Client], CL_Code, TL_Task as [Task] ,STL_SubTask as [Sub Task],COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as Duration, R_Comments from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID where R_TeamId=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'  GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,CL_ClientName, TL_Task, STL_SubTask, R_Comments, CL_Code Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
               //sQuery = "SELECT CONVERT(VARCHAR(12), R_Start_Date_Time, 101) as [Date], R_User_Name as [User], CL_ClientName as [Client], CL_Code, TL_Task as [Task] ,STL_SubTask as [Sub Task],Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as Duration, R_Comments, T_TeamName from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and T_Location = 'IND'  GROUP BY CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name,CL_ClientName, TL_Task, STL_SubTask, R_Comments, CL_Code, T_TeamName Order By CONVERT(VARCHAR(12), R_Start_Date_Time, 101), R_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }
       
       public DataTable DetailedLogReport(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date],  LA_User_Name, LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') and LA_User_Name =@teamId group by T_TeamName, LA_User_Name, LA_Reason, CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           else
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date],  LA_User_Name, LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') and LA_TeamId =@teamId group by T_TeamName, LA_User_Name, LA_Reason, CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable DetailedLogReportNew(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, LA_Reason, COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as Duration, LA_Comments " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') and LA_User_Name =@teamId group by T_TeamName, LA_User_Name, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_Comments order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
           }
           else
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, LA_Reason, COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as Duration, LA_Comments " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') and LA_TeamId =@teamId group by T_TeamName, LA_User_Name, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_Comments order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";

               //sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date],  LA_User_Name, LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration, LA_Comments, T_TeamName " +
               //         "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call') and LA_TeamId =@teamId group by T_TeamName, LA_User_Name, LA_Reason, CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_Comments, T_TeamName order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable DetailedMeetingReport(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date], LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)) as LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and LA_Reason = 'Meeting' and LA_User_Name =@teamId group by T_TeamName, LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)), CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           else
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date], LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)) as LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration " +
                       "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and LA_Reason = 'Meeting' and LA_TeamId =@teamId group by T_TeamName, LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)), CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable DetailedMeetingReportNew(string type, string teamId, string from, string to)
       {
           dt = new DataTable();

           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date], LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)) as LA_Reason, COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as Duration, LA_Comments " +
                        "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and LA_Reason = 'Meeting' and LA_User_Name =@teamId group by T_TeamName, LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)), CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_Comments order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
           }
           else
           {
               sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date], LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)) as LA_Reason, COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as Duration, LA_Comments " +
                       "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and LA_Reason = 'Meeting' and LA_TeamId =@teamId group by T_TeamName, LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)), CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_Comments order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";

               //sQuery = "select CONVERT(VARCHAR(12), LA_Start_Date_Time, 101) as [date], LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)) as LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) +':'+ Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as Duration, LA_Comments, T_TeamName " +
               //       "from RTM_Log_Actions, dbo.RTM_Team_List where LA_TeamId = T_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) between @from and @to and LA_Reason = 'Meeting' and T_Location = 'IND' group by T_TeamName, LA_User_Name, SUBSTRING(LA_Comments,0,CHARINDEX('-',LA_Comments,0)), CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_Comments, T_TeamName order BY CONVERT(VARCHAR(12), LA_Start_Date_Time, 101), LA_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetTeamTaskHoursBillable(string teamId, string start, string end)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@start", start),
                new SqlParameter("@end", end)
            
            };

           sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Records WITH (NOLOCK) left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID where R_TeamId = @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @start and @end and R_Duration != 'HH:MM:SS' and CL_ClientName <> 'Internal' and R_Task <> 0";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetTeamTaskHoursNonBillable(string teamId, string start, string end)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@start", start),
                new SqlParameter("@end", end)            
            };

           sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Records WITH (NOLOCK) left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID where R_TeamId=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @start and @end and R_Duration != 'HH:MM:SS' and CL_ClientName ='Internal'  and R_Task <> 0";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetTeamLogHoursNonBillable(string teamId, string start, string end)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@start", start),
                new SqlParameter("@end", end)
            };

          // sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Log_Actions WITH (NOLOCK) where LA_TeamId=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) BETWEEN @start and @end and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or  LA_Reason ='Meeting' or LA_Reason = 'Non-Task')";

           sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Log_Actions WITH (NOLOCK) where LA_TeamId=@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) BETWEEN @start and @end and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or  LA_Reason ='Meeting' or LA_Reason = 'Non-Task')";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetTeamLeaves(string teamId, string start, string end)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@from", start),
                new SqlParameter("@to", end)            
            };

           //sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Records WITH (NOLOCK) where R_TeamId = @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) BETWEEN @from and @to and R_Task =0 and R_Duration != 'HH:MM:SS' ";

          // sQuery = "select SUM(CAST(LD_Duration as Float)) as [hours] from dbo.RTM_LeaveDetails left join RTM_User_List on LD_UserName = UL_User_Name where UL_Team_Id =@teamId and LD_Duration is not null and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to";

           sQuery = "SELECT ROUND(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),2) as [hours] from RTM_Records WITH (NOLOCK) where R_TeamId = @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Task =0 and R_Duration != 'HH:MM:SS' ";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetUsersCount(string teamId)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@teamId", teamId)
           };

           sQuery = "select count(UL_User_Name) as [count] from RTM_User_List WITH (NOLOCK) where UL_Team_Id= @teamId and UL_User_Status =1";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       public DataTable GetUsersCountNew(string teamId, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           sQuery = "select COUNT(Distinct R_User_Name) as [count] from RTM_Records WITH (NOLOCK) where R_TeamId= @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       public DataTable GetPTO(string type, string team, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@team", team),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           if (type == "user")
           {
               sQuery = "SELECT CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID where R_User_Name=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and R_Task =0 GROUP BY CL_ClientName";
           }
           else
           {
               sQuery = "SELECT CL_ClientName, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from  RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID where R_TeamID=@team and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and R_Task =0 GROUP BY CL_ClientName";
           }

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       // Task %

       public DataTable getTotalTaskHours(string team, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@team", team),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           sQuery = "Select "+
                    "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) from RTM_Records WITH (NOLOCK) where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId =@team and R_Task <> 0) " +
                    "+ "+
                    "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) from RTM_Records WITH (NOLOCK) where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId =@team and R_Task = 0) " +
                    "+ "+
                    "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) from RTM_Log_Actions WITH (NOLOCK) where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' ) and LA_TeamId =@team) " +
                    "as [Total]";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       public DataTable getTaskHours(string team, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@team", team),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           sQuery = "select TL_Task, COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as [hours] from RTM_Records WITH (NOLOCK) left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId =@team and R_Task <> 0 group by TL_Task";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       public DataTable getTaskPTOHours(string team, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@team", team),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           sQuery = "select COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as [hours] from RTM_Records WITH (NOLOCK) where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId =@team and R_Task = 0";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

       public DataTable getTaskLogHours(string team, string from, string to)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@team", team),
               new SqlParameter("@from",from),
               new SqlParameter("@to", to)
           };

           sQuery = "select LA_Reason, COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as [hours] from RTM_Log_Actions WITH (NOLOCK) where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' ) and LA_TeamId =@team group by LA_Reason";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }


       //Resource Utilization for DashBoard
       DataTable dtResult = new DataTable();
       private void BuildRUTable()
       {
           dtResult = new DataTable();           
           try
           {               
               dtResult.Columns.Add("Employee Name");               
               dtResult.Columns.Add("Team Name");               
               dtResult.Columns.Add("Total Hours");               
               dtResult.Columns.Add("Absent Hours");               
               dtResult.Columns.Add("RU%", typeof(double));              
               dtResult.Columns.Add("NonTask");
           }
           catch (Exception)
           {
           }
       }

       public DataTable GetRUUsers(string team, string from, string to, string teamName)
       {
           BuildRUTable();
           DataRow dr;
           //int _workingDays = GetNumberOfWorkingDays(Convert.ToDateTime("12/01/2017"), Convert.ToDateTime("12/31/2017"));
           int _workingDays = GetNumberOfWorkingDays(Convert.ToDateTime(from), Convert.ToDateTime(to));
           int _workingHours;
           checked
           {
               _workingHours = _workingDays * 8;
           }

           //dt = GetTotalHoursForRU(team, "12/01/2017", "12/31/2017");
           dt = GetTotalHoursForRU(team, from, to);
           if (dt.Rows.Count > 0)
           {
               foreach (DataRow drRow in dt.Rows)
               {                   
                   double totalWorkHours = Convert.ToDouble(drRow["Total"]);
                   double leaveHours = Convert.ToDouble(drRow["LeaveHours"]);
                   double RUPercent = (totalWorkHours / (_workingHours - leaveHours)) * 100;
                   RUPercent = Math.Round(RUPercent, 2, MidpointRounding.AwayFromZero);
                   double totalNonTaskHours = Convert.ToDouble(drRow["NontaskHours"]);

                   dr = dtResult.NewRow();

                   dr["Employee Name"] = drRow["user"].ToString();
                   dr["Team Name"] = teamName;
                   dr["Total Hours"] = totalWorkHours;
                   dr["Absent Hours"] = leaveHours;
                   dr["RU%"] = RUPercent;
                   dr["NonTask"] = totalNonTaskHours;

                   dtResult.Rows.Add(dr);
               }
           }

           DataView dv = dtResult.DefaultView;
           dv.Sort = "[RU%] Desc";
           dtResult = dv.ToTable(); 

           return dtResult;
       }

       public DataTable GetTotalHoursForRU(string teamId, string from, string to)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

           sQuery = "Select A.[user], ROUND(SUM(COALESCE(A.hours1,0) + COALESCE(B.hours2,0)),2) as [Total], COALESCE(C.[hours3],0) as [LeaveHours], ROUND(SUM(COALESCE(D.hours4,0) + COALESCE(E.hours5,0)),2) as [NontaskHours] FROM "+
                    "(select R_User_Name as [user], COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as [hours1] from RTM_Records with (nolock) left join RTM_SubTask_List with (nolock) ON R_SubTask = STL_ID WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and STL_SubTask != 'NON-TASK' and R_Task <> 0 and R_TeamId = @teamId GROUP BY R_User_Name) A " +
                    "LEFT JOIN "+
                    "(select LA_User_Name as [user], COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as [hours2] from RTM_Log_Actions with (nolock) WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) BETWEEN @from and @to and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or  LA_Reason ='Meeting') and LA_TeamId = @teamId GROUP BY LA_User_Name) B " +
                    "ON A.[user] = B.[user] "+
                    "LEFT JOIN "+
                    "(select R_User_Name as [user], COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as [hours3] from RTM_Records with (nolock) WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and R_Task = 0 and R_TeamId = @teamId GROUP BY R_User_Name) C " +
                    "ON A.[user] = C.[user] "+
                    "LEFT JOIN "+
                    "(select R_User_Name as [user], COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) as [hours4] from RTM_Records with (nolock) left join RTM_SubTask_List with (nolock) ON R_SubTask = STL_ID WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and STL_SubTask = 'NON-TASK' and R_TeamId = @teamId GROUP BY R_User_Name) D " +
                    "ON A.[user] = D.[user] "+
                    "LEFT JOIN "+
                    "(select LA_User_Name as [user], COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) as [hours5] from RTM_Log_Actions with (nolock) WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) BETWEEN @from and @to and LA_Duration != 'HH:MM:SS' and LA_Reason = 'Non-Task' and LA_TeamId = @teamId GROUP BY LA_User_Name) E " +
                    "ON A.[user] = E.[user] GROUP BY A.[user], C.[hours3]";

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }
    }
}
