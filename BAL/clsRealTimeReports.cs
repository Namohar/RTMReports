using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
using System.Data.SqlClient;

namespace BAL
{
   public class clsRealTimeReports
    {
       DataTable dt = new DataTable();
       DataSet ds = new DataSet();
       SqlDBHelper objDB = new SqlDBHelper();
       string sQuery;
       public DataTable LoadEmp(string _access, string _location, string _manager, string _teamId, string _syatemUserName, string uid)
       {
           dt = new DataTable();
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@location", _location),
               new SqlParameter("@manager", _manager),
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@syatemUserName", _syatemUserName),
               new SqlParameter("@uid", uid)
           };
           if (_access == "1")
           {
               sQuery = "SELECT * FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_User_Status =1 ORDER BY UL_User_Name";
           }
           else if (_access == "2")
           {
               //sQuery = "; with  CTE as "+
               //            "( "+
               //             " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_User_Name = @manager " +
               //              "union all "+
               //              "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child "+
               //                 "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 "+
               //             ") "+
               //         "select  UL_ID, UL_User_Name, level from CTE order by UL_User_Name ";
               sQuery = "select * from RTM_User_List, RTM_AccessPermissions where UL_Team_Id = AP_TID and AP_UID =@uid and AP_Status =1 and UL_User_Status =1 order By UL_User_Name";
               //sQuery = "select * from RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_Team_Id IN (Select T_ID from RTM_Team_List where T_Manager = @manager ) and UL_User_Status =1 ";
           }
           else if (_access == "3")
           {
               sQuery = "; with  CTE as " +
                           "( " +
                            " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_User_Name = @manager " +
                             "union all " +
                             "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                                "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                            ") " +
                        "select  UL_ID, UL_User_Name, level from CTE order by UL_User_Name ";
              // sQuery = "SELECT * FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
           }
           else if (_access == "4")
           {
               sQuery = "SELECT * FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_System_User_Name=@syatemUserName and UL_User_Status =1 ORDER BY UL_User_Name";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           //dt = objDB.DBExecDataTable(sQuery);
           return dt;
       }

       public DataTable LoadTeams(string _access, string _location, string _manager, string teamId, string uid)
       {
           dt = new DataTable();
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@location", _location),
               new SqlParameter("@manager", _manager),
               new SqlParameter("@teamId", teamId),
               new SqlParameter("@uid", uid)
           };
           if (_location == "ADM")
           {
               sQuery = "SELECT * From RTM_Team_List WITH (NOLOCK) where T_Active = 1 order by T_TeamName";
           }
           else if (_access == "1")
           {
               sQuery = "SELECT * From RTM_Team_List WITH (NOLOCK) where T_Location = @location and T_Active = 1 order by T_TeamName";
           }
           else if (_access == "2")
           {
               //sQuery = "; with  CTE as " +
               //            "( " +
               //             " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_User_Name = @manager " +
               //              "union all " +
               //              "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
               //                 "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
               //             ") " +
               //         "select  T_ID, T_TeamName from CTE, RTM_Team_List Where CTE.UL_Team_Id = T_ID GROUP BY T_ID, T_TeamName order by T_TeamName";
               sQuery = "select * from RTM_Team_List WITH (NOLOCK) left join RTM_AccessPermissions WITH (NOLOCK) on T_ID = AP_TID where AP_UID =@uid and AP_Status =1 and T_Active =1 order by T_TeamName ";
               //sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_Manager =@manager and T_Active = 1 order by T_TeamName";
           }
           else if (_access == "3")
           {
               sQuery = "; with  CTE as " +
                           "( " +
                            " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List WITH (NOLOCK) where UL_User_Name = @manager " +
                             "union all " +
                             "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                                "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                            ") " +
                        "select  T_ID, T_TeamName from CTE, RTM_Team_List Where CTE.UL_Team_Id = T_ID GROUP BY T_ID, T_TeamName order by T_TeamName";
               //sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_ID = @teamId and T_Active = 1 order by T_TeamName";
           }
           else if (_access == "4")
           {
               sQuery = "SELECT * From RTM_Team_List WITH (NOLOCK) where T_Location = @location and T_ID = @teamId and T_Active = 1 order by T_TeamName";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           //dt = objDB.DBExecDataTable(sQuery);
           return dt;
       }

       public DataSet LoadUsers(string _access, string _location, string _manager, string _teamId, string dtName, string uid)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@location", _location),
               new SqlParameter("@manager", _manager),
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@uid", uid)
           };
           if (_access == "1")
           {
               sQuery = "SELECT UL_User_Name FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location ORDER BY UL_User_Name";
           }
           else if (_access == "2")
           {
               //sQuery = "; with  CTE as " +
               //            "( " +
               //             " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_User_Name = @manager " +
               //              "union all " +
               //              "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
               //                 "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
               //             ") " +
               //         "select UL_User_Name from CTE order by UL_User_Name ";
               sQuery = "select UL_User_Name from RTM_User_List, RTM_AccessPermissions where UL_Team_Id = AP_TID and AP_UID =@uid and AP_Status =1 and UL_User_Status =1 order By UL_User_Name";
               //sQuery = "SELECT UL_User_Name FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_Team_Id IN (Select T_ID from RTM_Team_List where T_Manager =@manager) ORDER BY UL_User_Name";
           }
           else if (_access == "3")
           {
               sQuery = "; with  CTE as " +
                          "( " +
                           " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_User_Name = @manager " +
                            "union all " +
                            "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                               "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                           ") " +
                       "select UL_User_Name from CTE order by UL_User_Name ";
               //sQuery = "SELECT UL_User_Name FROM RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and T_Location = @location and UL_Team_Id =@teamId ORDER BY UL_User_Name";
           }
           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
           //ds = objDB.DBExecDataSet(sQuery, dtName);
           return ds;
       }

       public DataSet GetMeetingsForTSheet(string user, DateTime login, DateTime logout, string dtName)
       {
           SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@user", user),
                new SqlParameter("@login", login),
                new SqlParameter("@logout", logout),
                new SqlParameter("@date", login.ToShortDateString())
            };

           sQuery = "select SUBSTRING(LA_Comments,0,charindex('-',LA_Comments)) as [group], ISNULL(sum(datediff(second,'00:00:00',LA_Duration))/3600,00) as hour,ISNULL((sum(datediff(second,'00:00:00',LA_Duration))/60)%60,00) as minute,ISNULL((sum(datediff(second,'00:00:00',LA_Duration))-(((sum(datediff(second,'00:00:00',LA_Duration))/3600)*3600)-60*((sum(datediff(second,'00:00:00',LA_Duration))/60)%60)))%60,00) as seconds from RTM_Log_Actions where LA_User_Name= @user and LA_Start_Date_Time BETWEEN @login AND @logout  and LA_Duration != 'HH:MM:SS' and (LA_Reason = 'Meetings' or LA_Reason='Meeting' ) Group BY SUBSTRING(LA_Comments,0,charindex('-',LA_Comments))";

           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
           return ds;
       }

       public DataTable GetMeetingServiceCode(string category, string team, string user)
       {
           dt = new DataTable();
           SqlParameter[] parameters = new SqlParameter[]
           {
              new SqlParameter("@category", category),
              new SqlParameter("@team", team),
              new SqlParameter("@user", user)
           };

           if (team == "0")
           {
               sQuery = "select M_ServiceCode from dbo.RTM_MeetingsCategory where M_Category =@category and M_TeamId = (select UL_Team_Id from dbo.RTM_User_List where UL_User_Name=@user)";
           }
           else
           {
               sQuery = "select M_ServiceCode from dbo.RTM_MeetingsCategory where M_Category =@category and M_TeamId = @team";
           }

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetPreference(string type, string team, string user)
       {
           dt = new DataTable();
           SqlParameter[] parameters = new SqlParameter[]
           {
              new SqlParameter("@team", team),
              new SqlParameter("@user", user)
           };
           if (type == "Team")
           {
               sQuery = "select T_Preference from RTM_Team_List where T_ID =@team";
           }
           else if (type == "User")
           {
               sQuery = "select T_Preference from RTM_Team_List, RTM_User_List where T_ID = UL_Team_ID and UL_User_Name =@user";
           }
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataTable GetUsersForCheckingLeaves(string team, string type, string from, string to)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@team", team),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
           };
           if (type == "user")
           {
               sQuery = "select UL_User_Name from RTM_User_List where UL_User_Name Not in (select LA_User_Name from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) Between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday')) and UL_User_Name =@team and UL_User_Status =1 ";
           }
           else
           {
               sQuery = "select UL_User_Name from RTM_User_List where UL_User_Name Not in (select LA_User_Name from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) Between @from and @to and (datename(dw,LA_Start_Date_Time) != 'Saturday' or datename(dw,LA_Start_Date_Time) != 'Sunday')) and UL_Team_Id =@team and UL_User_Status =1 ";
           }

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataSet CheckAbsentUsers(string _teamId, string _date, string _type,  string preference)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@date", _date),
               new SqlParameter("@status", 1)
           };
           if (_type == "user")
           {
               if (preference == "1")
               {
                   sQuery = "select Distinct LA_User_Name from RTM_Log_Actions where LA_User_Name =@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_Log_Action ='First Activity'";
               }
               else
               {
                   sQuery = "select Distinct R_User_Name from RTM_Records where R_User_Name =@teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date";
               }
           }
           else
           {
               if (preference == "1")
               {
                   sQuery = "select UL_User_Name from RTM_User_List where UL_Team_Id = @teamId and UL_User_Status = @status and UL_User_Name NOT IN (select DISTINCT LA_User_Name from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_TeamId = @teamId  and LA_Log_Action ='First Activity') ";
               }
               else
               {
                   sQuery = "select UL_User_Name from RTM_User_List where UL_Team_Id = @teamId and UL_User_Status = @status and UL_User_Name NOT IN (select DISTINCT R_User_Name from RTM_Records where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_TeamId = @teamId) ";
               }
           }
           
           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "absent");

           return ds;
       }

       public DataTable CheckUpdatedAbsents(string _user, string _date)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user", _user),
               new SqlParameter("@date", _date)
           };
           sQuery = "select *  from RTM_LeaveDetails where LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) = @date";
           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           return dt;
       }

       public DataSet TotalTask(string _teamId, string _date, string _type)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@date", _date),
               new SqlParameter("@status", 1)
           };

           if (_type == "user")
           {
               sQuery = "select R_User_Name, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records where R_User_Name= @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_Duration != 'HH:MM:SS' group by R_User_Name";
           }
           else
           {
               sQuery = "select R_User_Name, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Records where R_TeamId= @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_Start_Date_Time))) = @date and R_Duration != 'HH:MM:SS' group by R_User_Name";
           }

           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "task");

           return ds;
       }

       public DataSet TotalLog(string _teamId, string _date, string _type)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@date", _date)               
           };

           if (_type == "user")
           {
               sQuery = "select LA_User_Name, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_User_Name= @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_Duration != 'HH:MM:SS' and LA_Reason !='Break' and LA_Reason !='Idle Time' and LA_Reason !='Unassigned Time' group by LA_User_Name";
           }
           else
           {
               sQuery = "select LA_User_Name, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600 as hour, (sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60 as minute,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60 as seconds from RTM_Log_Actions where LA_TeamId= @teamId and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_Start_Date_Time))) = @date and LA_Duration != 'HH:MM:SS' and LA_Reason !='Break' and LA_Reason !='Idle Time' and LA_Reason !='Unassigned Time' group by LA_User_Name";
           }

           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "log");

           return ds;
       }

       public DataSet GetUsers(string _teamId)
       {
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@status", 1)               
           };

           sQuery = "select UL_User_Name from RTM_User_List WITH (NOLOCK) where UL_Team_Id =@teamId and UL_User_Status =@status order by UL_User_Name";
           ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, "absent");
           return ds;
       }

       public DataTable LoadAllTeams(string _access,string location)
       {
           SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@location", location)
           };

           if (location == "ADM")
           {
               sQuery = "Select * from RTM_Team_List where T_Active = 1 order by T_TeamName";
           }
           else
           {
               sQuery = "Select * from RTM_Team_List where T_Location = @location and T_Active = 1 order by T_TeamName";
           }

           //if (_access == "1")
           //{
           //    sQuery = "Select * from RTM_Team_List where T_Active = 1 order by T_TeamName";
           //}
           //else
           //{
           //    sQuery = "Select * from RTM_Team_List where T_Location = @location and T_Active = 1 order by T_TeamName";
           //}

           dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
           return dt;
       }

    }
}
