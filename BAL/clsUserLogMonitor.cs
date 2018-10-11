using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;

namespace BAL
{
    public class clsUserLogMonitor
    {
        DataSet ds = new DataSet();

        public DataTable fetchTeams(int access, string user, int team, string locatioon, string uid)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery="";
           SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@user", user),
               new SqlParameter("@team", team),
               new SqlParameter("@location", locatioon),
               new SqlParameter("@uid", uid)
           };
            if (access == 1)
            {
                sQuery = "SELECT * From RTM_Team_List where T_Active = 1 and T_Location=@location  order by T_TeamName";
            }
            else if (access == 2)
            {
                //sQuery = "; with  CTE as " +
                //           "( " +
                //            " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_ID = @uid " +
                //             "union all " +
                //             "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                //                "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                //            ") " +
                //        "select  T_ID, T_TeamName from CTE, RTM_Team_List Where CTE.UL_Team_Id = T_ID GROUP BY T_ID, T_TeamName order by T_TeamName";
                sQuery = "select * from RTM_Team_List, RTM_AccessPermissions where T_ID = AP_TID and AP_UID =@uid and AP_Status =1 and T_Active =1 order by T_TeamName";
                //sQuery = "SELECT * From RTM_Team_List where T_Manager =@user and T_Location=@location and T_Active = 1 order by T_TeamName";
            }
            else if (access == 3)
            {
                sQuery = "; with  CTE as " +
                           "( " +
                            " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List where UL_ID = @uid " +
                             "union all " +
                             "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                                "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                            ") " +
                        "select  T_ID, T_TeamName from CTE, RTM_Team_List Where CTE.UL_Team_Id = T_ID GROUP BY T_ID, T_TeamName order by T_TeamName";
                //sQuery = "SELECT * From RTM_Team_List where T_ID = @team and T_Location=@location and T_Active = 1 order by T_TeamName";
            }
            else if (access == 4)
            {
                sQuery = "SELECT * From RTM_Team_List where T_ID = @team and T_Location=@location and T_Active = 1 order by T_TeamName";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

    }
}
