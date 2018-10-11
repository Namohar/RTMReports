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
    public class clsADM
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        string sQuery;

        public DataTable GetAvgHours(string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            if (location == "All")
            {
                sQuery = "select T_TeamName, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600))/(Count(Distinct R_User_Name)) as [Avg] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to group by T_TeamName order by T_TeamName";
            }
            else
            {
                sQuery = "select T_TeamName, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600))/(Count(Distinct R_User_Name)) as [Avg] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location=@location group by T_TeamName order by T_TeamName";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTeamTotal(string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            if (location == "All")
            {
                sQuery = "SELECT A.team as [Team1], B.team as [Team2], SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)) as [Total] "+
                        "FROM "+
                        " ( select T_TeamName as [team], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  Group by T_TeamName  ) A " +
                        "Left join "+
                        " ( select T_TeamName as [team], SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List where LA_TeamId = T_ID and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to  and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName) B " +
                         "on A.team = B.team group by A.team, B.team  order by A.team"; 
                //sQuery = "select T_TeamName, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600))/(Count(Distinct R_User_Name)) as [Avg] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to group by T_TeamName order by T_TeamName";
            }
            else
            {
                sQuery = "SELECT A.team as [Team1], B.team as [Team2], SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)) as [Total] " +
                        "FROM " +
                        " ( select T_TeamName as [team], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location = @location Group by T_TeamName  ) A " +
                        "Left join " +
                        " ( select T_TeamName as [team], SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List where LA_TeamId = T_ID and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and T_Location = @location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName) B " +
                         "on A.team = B.team group by A.team, B.team  order by A.team"; 
               // sQuery = "select T_TeamName, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600))/(Count(Distinct R_User_Name)) as [Avg] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location=@location group by T_TeamName order by T_TeamName";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetUserTotal(string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            if (location == "All")
            {
                sQuery = " SELECT A.team as [Team1], B.team as [Team2], A.username as [user1], B.username as [user2], SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)) as [Total] "+
                        "FROM "+
                         "( select T_TeamName as [team], R_User_Name as [username],  SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to Group by T_TeamName, R_User_Name  ) A "+
                        "Left join "+
                         "( select T_TeamName as [team], LA_User_Name as [username], SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List where LA_TeamId = T_ID and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name) B "+
                         "on A.username = B.username Group by A.team, B.team,A.username, B.username order by A.team, A.username";
            }
            else
            {
                sQuery = " SELECT A.team as [Team1], B.team as [Team2], A.username as [user1], B.username as [user2], SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)) as [Total] " +
                        "FROM " +
                         "( select T_TeamName as [team], R_User_Name as [username],  SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location=@location Group by T_TeamName, R_User_Name  ) A " +
                        "Left join " +
                         "( select T_TeamName as [team], LA_User_Name as [username], SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List where LA_TeamId = T_ID and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and T_Location=@location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name) B " +
                         "on A.username = B.username Group by A.team, B.team,A.username, B.username order by A.team, A.username";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalHours(string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };

            if (location == "All")
            {
                sQuery = "select T_TeamName, R_User_Name, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [Total] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to group by T_TeamName, R_User_Name order by T_TeamName";
            }
            else
            {
                sQuery = "select T_TeamName, R_User_Name, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [Total] from RTM_Records, RTM_Team_List where R_TeamId = T_ID and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location = @location group by T_TeamName, R_User_Name order by T_TeamName";
            }
            

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }        

        public DataTable GetTotalWorkHours(string type, string from, string to, string name)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@name", name)
            };
            if (type == "user")
            {
                sQuery = "Select "+
                        "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) from RTM_Records where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_User_Name =@name) " +
                        "+ "+
                        "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) from RTM_Log_Actions where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_User_Name =@name) " +
                        "as [Total]";
            }
            else
            {
                sQuery = "Select " +
                       "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600),0) from RTM_Records where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId =@name) " +
                       "+ " +
                       "(select COALESCE(SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600),0) from RTM_Log_Actions where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_TeamId =@name) " +
                       "as [Total]";
            }


            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }        

        public DataTable GetUserCount(string team)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {               
               new SqlParameter("@team", team)
            };

            sQuery = "select Count(UL_User_Name) as [Count] from RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_TeamName = @team and UL_User_Status =1";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetAvgHoursByClient(string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            if (location == "All")
            {
                sQuery = "select UL_Emp_Location, CL_Code, CL_ClientName, dbo.SplitColumnValue(STL_ServiceCode, '|', 1) as STL_ServiceCode, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records left join RTM_User_List on R_User_Name = UL_User_Name left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) between @from and @to and R_Duration !='HH:MM:SS' group by UL_Emp_Location, CL_Code, CL_ClientName, STL_ServiceCode order by UL_Emp_Location";
            }
            else
            {
                sQuery = "select UL_Emp_Location, CL_Code, CL_ClientName, dbo.SplitColumnValue(STL_ServiceCode, '|', 1) as STL_ServiceCode, (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records left join RTM_User_List on R_User_Name = UL_User_Name left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) between @from and @to and R_Duration !='HH:MM:SS' and T_Location= @location group by UL_Emp_Location, CL_Code, CL_ClientName, STL_ServiceCode order by UL_Emp_Location";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable SummaryClientHoursbyEmployees(string team, string actuallocation, string seleLocation, string from, string to)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@team", team),
                new SqlParameter("@seleLocation", seleLocation),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            if (actuallocation == "ADM")
            {
                if (team == "All")
                {
                    sQuery = "select R_User_Name as [Employee Name],CL_ClientName as [Client Name], CL_Code as [Client Code], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours], UL_Employee_Id as [Employee Id], UL_Job_Title as [Job Title], UL_RepMgrEmail as [Manager Email], UL_Director as [Director] from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by R_User_Name,CL_ClientName,CL_Code, UL_Employee_Id, UL_Job_Title, UL_RepMgrEmail, UL_Director order by R_User_Name";
                }
                else
                {
                    sQuery = "select R_User_Name as [Employee Name],CL_ClientName as [Client Name], CL_Code as [Client Code], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours], UL_Employee_Id as [Employee Id], UL_Job_Title as [Job Title], UL_RepMgrEmail as [Manager Email], UL_Director as [Director] from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where T_Location =@seleLocation and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by R_User_Name,CL_ClientName,CL_Code, UL_Employee_Id, UL_Job_Title, UL_RepMgrEmail, UL_Director order by R_User_Name";
                }
            }
            else
            {
                if (team == "All")
                {
                    sQuery = "select R_User_Name as [Employee Name],CL_ClientName as [Client Name], CL_Code as [Client Code], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours], UL_Employee_Id as [Employee Id], UL_Job_Title as [Job Title], UL_RepMgrEmail as [Manager Email], UL_Director as [Director] from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where T_Location =@seleLocation and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by R_User_Name,CL_ClientName,CL_Code, UL_Employee_Id, UL_Job_Title, UL_RepMgrEmail, UL_Director order by R_User_Name";
                }
                else
                {
                    sQuery = "select R_User_Name as [Employee Name],CL_ClientName as [Client Name], CL_Code as [Client Code], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours], UL_Employee_Id as [Employee Id], UL_Job_Title as [Job Title], UL_RepMgrEmail as [Manager Email], UL_Director as [Director] from RTM_Records, RTM_Client_List, RTM_User_List where R_Client = CL_ID and R_User_Name= UL_User_Name and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId=@team  group by R_User_Name,CL_ClientName,CL_Code, UL_Employee_Id, UL_Job_Title, UL_RepMgrEmail, UL_Director order by R_User_Name";
                }
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);

            return dt;
        }


        public DataTable SummaryClientHoursbyRoles(string team, string actuallocation, string seleLocation, string from, string to)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@team", team),
                new SqlParameter("@seleLocation", seleLocation),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            if (actuallocation == "ADM")
            {
                if (team == "All")
                {
                    sQuery = "select UL_Job_Title as [Job Title],CL_ClientName as [Client Name], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records, RTM_Client_List, RTM_User_List where R_Client = CL_ID and R_User_Name = UL_User_Name and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by UL_Job_Title,CL_ClientName order by UL_Job_Title,CL_ClientName";
                }
                else
                {
                    sQuery = "select UL_Job_Title as [Job Title],CL_ClientName as [Client Name], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records, RTM_Client_List, RTM_Team_List, RTM_User_List where R_Client = CL_ID and R_TeamId = T_ID and R_User_Name = UL_User_Name and T_Location =@seleLocation and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by UL_Job_Title,CL_ClientName order by UL_Job_Title,CL_ClientName";
                }
            }
            else
            {
                if (team == "All")
                {
                    sQuery = "select UL_Job_Title as [Job Title],CL_ClientName as [Client Name], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records, RTM_Client_List, RTM_Team_List, RTM_User_List where R_Client = CL_ID and R_TeamId = T_ID and R_User_Name = UL_User_Name and T_Location =@seleLocation and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to  group by UL_Job_Title,CL_ClientName order by UL_Job_Title,CL_ClientName";
                }
                else
                {
                    sQuery = "select UL_Job_Title as [Job Title],CL_ClientName as [Client Name], (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)) as [Total Hours] from RTM_Records, RTM_Client_List, RTM_User_List where R_Client = CL_ID and R_User_Name = UL_User_Name and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId=@team  group by UL_Job_Title,CL_ClientName order by UL_Job_Title,CL_ClientName";
                }
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);

            return dt;
        }

        public DataTable GetWeeks(string from, string to)
        {
            SqlParameter[] param = new SqlParameter[]{              
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            sQuery = "WITH hier(num, lvl) AS (SELECT  0, 1 UNION ALL SELECT  100, 1 UNION ALL SELECT  num + 1, lvl + 1 FROM    hier WHERE   lvl < 100 ) "+
                    "SELECT  DATEADD(dw, -DATEPART(dw, @from), @from) + num  * 7 as [From], DATEADD(dw, -DATEPART(dw, @from), @from) + (num + 1) * 7 as [To] FROM hier " +
                    "WHERE   DATEADD(dw, -DATEPART(dw, @from), @from) + num * 7 < @to ORDER BY num";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;

        }

        public DataTable EmployeeHoursByWeek(string location, string from, string to)
        {
            SqlParameter[] param = new SqlParameter[]{  
                new SqlParameter("@location", location),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            if (location == "All")
            {
                sQuery = "select R_User_Name, UL_Employee_ID,UL_Job_Title,T_TeamName, UL_Emp_Location, CL_ClientName, CL_Code, TL_Task, STL_Subtask, STL_ServiceCode, COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0) as [Total Hours], UL_RepMgrEmail, UL_Director from RTM_Records WITH (NOLOCK)  " +
                    "left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name "+
                    "left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID "+
                    "left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID "+
                    "left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID " +
                    "left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID " +
                    "where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to " +
                    "and CL_ClientName <> 'Personal/Sick Time' and CL_ClientName <> 'Holiday' and CL_ClientName <> 'Public Holiday' and CL_ClientName <> 'Vacation' and CL_ClientName <> 'Bereavement' and CL_ClientName <> 'Jury Duty' and CL_ClientName <> 'Inclement Weather' " +
                    "group by R_User_Name,UL_Employee_ID,UL_Job_Title,T_TeamName,UL_Emp_Location, CL_ClientName, CL_Code, UL_RepMgrEmail, UL_Director, TL_Task, STL_Subtask, STL_ServiceCode";
            }
            else
            {
                sQuery = "select R_User_Name, UL_Employee_ID,UL_Job_Title,T_TeamName,UL_Emp_Location,  CL_ClientName, CL_Code, TL_Task, STL_Subtask, STL_ServiceCode, COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0) as [Total Hours], UL_RepMgrEmail, UL_Director from RTM_Records WITH (NOLOCK)  " +
                    "left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name " +
                    "left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID " +
                    "left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID " +
                    "left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID " +
                    "left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID " +
                    "where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location =@location " +
                    "and CL_ClientName <> 'Personal/Sick Time' and CL_ClientName <> 'Holiday' and CL_ClientName <> 'Public Holiday' and CL_ClientName <> 'Vacation' and CL_ClientName <> 'Bereavement' and CL_ClientName <> 'Jury Duty' and CL_ClientName <> 'Inclement Weather' " +
                    "group by R_User_Name,UL_Employee_ID,UL_Job_Title,T_TeamName,UL_Emp_Location,  CL_ClientName, CL_Code, UL_RepMgrEmail, UL_Director, TL_Task, STL_Subtask, STL_ServiceCode";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable EmployeeHoursByMonth(string location, string from, string to)
        {
            SqlParameter[] param = new SqlParameter[]{  
                new SqlParameter("@location", location),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };

            if (location == "All")
            {
                sQuery = "select R_User_Name, UL_Employee_ID,UL_Job_Title,T_TeamName, UL_Emp_Location, CL_ClientName, CL_Code, TL_Task, STL_Subtask, STL_ServiceCode, COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0) as [Total Hours], UL_RepMgrEmail, UL_Director from RTM_Records WITH (NOLOCK)  " +
                    "left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name " +
                    "left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID " +
                    "left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID " +
                    "left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID " +
                    "left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID " +
                    "where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to " +
                    "and CL_ClientName <> 'Personal/Sick Time' and CL_ClientName <> 'Holiday' and CL_ClientName <> 'Public Holiday' and CL_ClientName <> 'Vacation' and CL_ClientName <> 'Bereavement' and CL_ClientName <> 'Jury Duty' and CL_ClientName <> 'Inclement Weather' " +
                    "group by R_User_Name,UL_Employee_ID,UL_Job_Title,T_TeamName, UL_Emp_Location, CL_ClientName, CL_Code, UL_RepMgrEmail, UL_Director, TL_Task, STL_Subtask, STL_ServiceCode";
            }
            else
            {
                sQuery = "select R_User_Name, UL_Employee_ID,UL_Job_Title,T_TeamName, UL_Emp_Location, CL_ClientName, CL_Code, TL_Task, STL_Subtask, STL_ServiceCode, COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0) as [Total Hours], UL_RepMgrEmail, UL_Director from RTM_Records WITH (NOLOCK)  " +
                    "left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name " +
                    "left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID " +
                    "left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID " +
                    "left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID " +
                    "left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID " +
                    "where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location =@location " +
                    "and CL_ClientName <> 'Personal/Sick Time' and CL_ClientName <> 'Holiday' and CL_ClientName <> 'Public Holiday' and CL_ClientName <> 'Vacation' and CL_ClientName <> 'Bereavement' and CL_ClientName <> 'Jury Duty' and CL_ClientName <> 'Inclement Weather' " +
                    "group by R_User_Name,UL_Employee_ID,UL_Job_Title,T_TeamName, UL_Emp_Location, CL_ClientName, CL_Code, UL_RepMgrEmail, UL_Director, TL_Task, STL_Subtask, STL_ServiceCode";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable ContentMgmntReport(string managers, string from, string to)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]{  
                new SqlParameter("@managers", managers),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to)
            };
            sQuery = "; with  CTE as "+
                        "( "+
                            "select  UL_ID, UL_Team_Id, UL_Employee_Id , UL_RepMgrId, UL_User_Name,UL_RepMgrEmail, 1 as level from RTM_User_List where UL_User_Name IN (SELECT CAST(Value AS Varchar) FROM F_Split(@managers, ',')) " +
                            "union all "+
                            "select  child.UL_ID , child.UL_Team_Id , child.UL_Employee_Id , child.UL_RepMgrId, child.UL_User_Name,child.UL_RepMgrEmail, level + 1 from    RTM_User_List child "+
                            "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1  "+
                        ") "+
                        "select STL_ServiceCode as [Service Code], R_User_Name as [User Name], UL_RepMgrEmail as [Reporting Manager], ROUND(COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0), 4) as [Total Hours], DATENAME(m, R_TimeDate) as [Month] "+
                        "from CTE left join  RTM_Records WITH (NOLOCK) on R_User_Name = CTE.UL_User_Name "+
                        "left join RTM_Subtask_List WITH (NOLOCK) on R_SubTask = STL_Id "+
                        "WHERE R_SubTask<>0 and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_Duration <> 'HH:MM:SS' "+
                        "Group By STL_ServiceCode, R_User_Name,UL_RepMgrEmail,DATENAME(m, R_TimeDate) "+
                        "having ROUND(COALESCE((SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600)),0), 4) >0  "+
                        "order by STL_ServiceCode, R_User_Name";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable GetManagers()
        {
            dt = new DataTable();
            sQuery = "select Distinct U2.UL_User_Name, U2.UL_Employee_Id from RTM_User_List U1 left outer join RTM_User_List U2 on U1.UL_RepMgrId = U2.UL_Employee_Id " +
                                "Where U2.UL_User_Name is not null ";

            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetWeeklyHoursLessThan38(string from, string to, string location, string team)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@team", team),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@location", location)
            };

            if (team == "0")
            {
                sQuery = "SELECT A.UL_Employee_ID AS [Employee ID], A.username as [Employee Name], A.team as [Team Name],   A.UL_RepMgrEmail as [Reporting Manager], ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total Hours] " +
                        "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location = @location Group by T_TeamName, R_User_Name, UL_Employee_ID, UL_RepMgrEmail  ) A  " +
                        "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on LA_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on LA_User_Name = UL_User_Name where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and T_Location = @location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID, UL_RepMgrEmail) B on A.username = B.username " +
                        "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID, A.UL_RepMgrEmail HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) <= 38.75 order by A.UL_RepMgrEmail";
            }
            else
            {
                sQuery = "SELECT A.UL_Employee_ID AS [Employee ID], A.username as [Employee Name], A.team as [Team Name],   A.UL_RepMgrEmail as [Reporting Manager], ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total Hours] " +
                        "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId = @team Group by T_TeamName, R_User_Name, UL_Employee_ID, UL_RepMgrEmail  ) A  " +
                        "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on LA_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on LA_User_Name = UL_User_Name where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and LA_TeamId = @team and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID, UL_RepMgrEmail) B on A.username = B.username " +
                        "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID, A.UL_RepMgrEmail HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) <= 38.75 order by A.UL_RepMgrEmail";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           
            return dt;
        }

        public DataTable GetWeeklyHoursGreaterThan45(string from, string to, string location, string team)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@team", team),
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@location", location)
            };

            if (team == "0")
            {
                sQuery = "SELECT A.UL_Employee_ID AS [Employee ID], A.username as [Employee Name], A.team as [Team Name],   A.UL_RepMgrEmail as [Reporting Manager], ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total Hours] " +
                        "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location = @location Group by T_TeamName, R_User_Name, UL_Employee_ID, UL_RepMgrEmail  ) A  " +
                        "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on LA_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on LA_User_Name = UL_User_Name where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and T_Location = @location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID, UL_RepMgrEmail) B on A.username = B.username " +
                        "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID, A.UL_RepMgrEmail HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) >= 45 order by A.UL_RepMgrEmail";
            }
            else
            {
                sQuery = "SELECT A.UL_Employee_ID AS [Employee ID], A.username as [Employee Name], A.team as [Team Name],   A.UL_RepMgrEmail as [Reporting Manager], ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total Hours] " +
                        "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId = @team Group by T_TeamName, R_User_Name, UL_Employee_ID, UL_RepMgrEmail  ) A  " +
                        "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on LA_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on LA_User_Name = UL_User_Name where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and LA_TeamId = @team and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID, UL_RepMgrEmail) B on A.username = B.username " +
                        "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID, A.UL_RepMgrEmail HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) >= 45 order by A.UL_RepMgrEmail";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }
    }
}
