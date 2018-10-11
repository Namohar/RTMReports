using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;

namespace BAL
{
    public class clsTicketing
    {
        SqlDBHelper objDBL = new SqlDBHelper();

        public DataTable fetchEmpId(string username)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@userName", username) 
           };
            string sQuery = "select UL_ID, UL_Employee_Id, UL_Team_Id, UL_User_Name, UL_EmailId,UL_RepMgrEmail from RTM_User_List where UL_System_User_name=@userName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable getRTMSupportTeam()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "select * from RTM_Support_Team WITH (NOLOCK) where Status=1";
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable fetchAccess(string empid)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@empid", empid) 
           };
            string sQuery = "select AL_AccessLevel from RTM_Access_Level where AL_EmployeeId =@empid";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable SearchTickets(int team, int access, string empId, string user, string uid)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@team", team),
                new SqlParameter("@empId", empId),
                new SqlParameter("@user", user),
                new SqlParameter("@uid", uid)
            };
            if (access == 1)
            {
                sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on T_Employee_Id = UL_Employee_Id left join RTM_TicketStatus WITH (NOLOCK) on T_Status = TS_Name where T_Status <> 'Closed' order by TS_Order ASC,T_ID DESC";
            }            
            else if (access == 2)
            {
                sQuery = "select RTM_Tickets.T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on T_Employee_Id = UL_Employee_Id left join RTM_TicketStatus WITH (NOLOCK) on T_Status = TS_Name left join RTM_AccessPermissions WITH (NOLOCK) on UL_Team_Id = AP_TID where AP_UID =@uid and AP_Status =1 order by TS_Order ASC,T_ID DESC";                
            }
            else if (access == 3)
            {
                sQuery = "; with  CTE as " +
                           "( " +
                            " select  UL_ID, UL_Team_Id, UL_Employee_Id, UL_RepMgrId, UL_User_Name , 1 as level from RTM_User_List WITH (NOLOCK) where UL_Employee_Id = @empId " +
                             "union all " +
                             "select  child.UL_ID , child.UL_Team_Id, child.UL_Employee_Id, child.UL_RepMgrId, child.UL_User_Name, level + 1 from RTM_User_List child " +
                                "join    CTE parent on child.UL_RepMgrId = parent.UL_Employee_Id where UL_User_Status =1 " +
                            ") " +
                        "select RTM_Tickets.T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from CTE left join RTM_Tickets WITH (NOLOCK) on T_Employee_Id = UL_Employee_Id left join RTM_TicketStatus WITH (NOLOCK) on T_Status = TS_Name where RTM_Tickets.T_ID is not null order by TS_Order ASC,T_ID DESC ";
            }
            else if (access == 4)
            {
                sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on T_Employee_Id = UL_Employee_Id left join RTM_TicketStatus WITH (NOLOCK) on T_Status = TS_Name where T_Employee_Id=@empId order by TS_Order ASC,T_ID DESC";
            }
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);            
            return dt;
        }

        public DataTable SearchClosedTickets()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "";           

            sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on T_Employee_Id = UL_Employee_Id left join RTM_TicketStatus WITH (NOLOCK) on T_Status = TS_Name where T_Status = 'Closed' order by TS_Order ASC,T_ID DESC";

            dt = objDB.DBExecDataTable(sQuery);            
            
            return dt;
        }

        public DataTable ClosedTicketsCount()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@from", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6).ToShortDateString()),
                new SqlParameter("@to", DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString())
            };

            string sQuery = "select "+
                        "( select count(*) from RTM_Tickets WITH (NOLOCK) where T_Status = 'Closed') as [total count], " +
                        "(select count(*) from RTM_Tickets WITH (NOLOCK) where T_Status = 'Closed' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, T_ModifiedOn))) between @from and @to) as [last week]";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            return dt;
        }

        public DataTable SearchData(string emp, string status, int access, int team, string assignedTo, string empId)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "";
            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@emp", emp),
               new SqlParameter("@status", status),
               new SqlParameter("@team", team),
               new SqlParameter("@assignedTo", assignedTo),
               new SqlParameter("@empId", empId)
           };
            if (access == 1)
            {
                if (emp == "Please Select")
                {
                    if (status == "All")
                    {
                        sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List, RTM_TicketStatus where T_Employee_Id = UL_Employee_Id and T_Status = TS_Name order by TS_Order ASC,T_ID DESC  ";
                    }
                    else
                    {
                        sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List where T_Employee_Id = UL_Employee_Id and T_Status =@status order by T_ID DESC";
                    }
                }
                else
                {
                    if (status == "All")
                    {
                        if (emp == empId)
                        {
                            sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List, RTM_TicketStatus where T_Employee_Id = UL_Employee_Id and T_Status = TS_Name and (T_Employee_Id=@emp or T_Assigned_To=@assignedTo) order by TS_Order ASC,T_ID DESC";
                        }
                        else
                        {
                            sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List, RTM_TicketStatus where T_Employee_Id = UL_Employee_Id and T_Status = TS_Name and (T_Employee_Id=@emp) order by TS_Order ASC,T_ID DESC";
                        }
                    }
                    else
                    {
                        if (emp == empId)
                        {
                            sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List where T_Employee_Id = UL_Employee_Id and (T_Employee_Id=@emp or T_Assigned_To=@assignedTo) and T_Status =@status order by T_ID DESC";
                        }
                        else
                        {
                            sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List where T_Employee_Id = UL_Employee_Id and (T_Employee_Id=@emp) and T_Status =@status order by T_ID DESC";
                        }
                    }
                }

            }           
            else
            {
                if (status == "All")
                {
                    sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List, RTM_TicketStatus where T_Employee_Id = UL_Employee_Id and T_Status = TS_Name and T_Employee_Id=@emp order by TS_Order ASC,T_ID DESC";
                }
                else
                {
                    sQuery = "select T_ID, UL_User_Name, T_Ticket_Type,  T_Issued_Date_Time, T_Assigned_To, T_Status, T_ETA_Date_Time, T_Summary from RTM_Tickets, RTM_User_List where T_Employee_Id = UL_Employee_Id and T_Employee_Id=@emp and T_Status =@status order by T_ID DESC";
                }
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public int AddTicket(int id, string empId, string tktType, string summary, string details, DateTime issueDate, string assignedTo, string etaDate, string priority, string status, int modifiedBy)
        {
            int ticketId = objDBL.AddTicket(id, empId, tktType, summary, details, issueDate, assignedTo, etaDate, priority, status, modifiedBy);
            return ticketId;
        }

        public bool UpdateImages(int ticketId, string empId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ticketId", ticketId),
                new SqlParameter("@empId", empId)
            };
            string sQuery = "UPDATE RTM_Ticket_Attachments SET I_TicketId =@ticketId WHERE I_TicketId=0 and I_EmpId =@empId";
 
            //result = objDBL.DBExecuteNoNQuery(sQuery);
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);

            return result;
        }

        public bool DeleteImages(string empId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@empId", empId)
            };
            string sQuery = "DELETE from RTM_Ticket_Attachments where I_TicketId=0 and I_EmpId =@empId";

            //result = objDBL.DBExecuteNoNQuery(sQuery);
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool DeleteAttachment(string id, string name, string empId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@name", name),
                new SqlParameter("@empId", empId)
            };
            string sQuery = "DELETE from RTM_Ticket_Attachments where I_TicketId=0 and I_EmpId =@empId and I_Name= @name";

            //result = objDBL.DBExecuteNoNQuery(sQuery);
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable getTicketDetails(int Tid)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Tid", Tid)
            };
            string sQuery = "select * from dbo.RTM_Tickets WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on T_Employee_Id= UL_Employee_Id left join RTM_Access_Level WITH (NOLOCK) on T_Employee_Id = AL_EmployeeId where T_ID=@Tid";
            dt = objDBL.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable getRTMTeam()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "select * from RTM_User_List where UL_Team_Id='4'";
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable getAttachments(int TID)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TID", TID)
            };
            string sQuery = "select * from RTM_Ticket_Attachments where I_TicketId=@TID";
            dt = objDBL.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public bool UpdateTicket(string status, string assignedTo, string eta, int ticketId, string type)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@status", status),
                new SqlParameter("@assignedTo", assignedTo),
                new SqlParameter("@eta", eta),
                new SqlParameter("@ticketId", ticketId),
                new SqlParameter("@modifiedOn", DateTime.Now),
                new SqlParameter("@type", type)
            };
            string sQuery = "Update RTM_Tickets set T_Status=@status, T_Assigned_To=@assignedTo, T_ETA_Date_Time=@eta, T_ModifiedOn =@modifiedOn, T_Ticket_Type=@type where T_ID=@ticketId";
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            //result = objDBL.DBExecuteNoNQuery(sQuery);

            return result;
        }

        public bool UpdateNotes(string notes, int ticketId, string updateBy)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@notes", notes),
                new SqlParameter("@ticketId", ticketId), new SqlParameter("@updateBy", updateBy),
                new SqlParameter("@updatedOn",  DateTime.Now)
            };
            string sQuery = "insert into RTM_Tickets_Log(TLog_Ticket_Id, TLog_Updated_By, TLog_Updated_Date_Time, TLog_Details) values (@ticketId, @updateBy, @updatedOn, @notes)";
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            //result = objDBL.DBExecuteNoNQuery(sQuery);

            return result;
        }

        public bool UpdateStatus(string status, int ticketId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@status", status),
                new SqlParameter("@ticketId", ticketId),
                new SqlParameter("@modifiedOn",  DateTime.Now)
            };
            string sQuery = "Update RTM_Tickets set T_Status=@status, T_ModifiedOn =@modifiedOn where T_ID=@ticketId";
            result = objDBL.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            //result = objDBL.DBExecuteNoNQuery(sQuery);

            return result;
        }

        public DataTable getNotes(int TID)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TID", TID)
            };
            string sQuery = "select CONVERT(VARCHAR(20), TLog_Updated_Date_Time) + '-' + TLog_Updated_By + CHAR(13) + CHAR(10) +TLog_Details as notes from dbo.RTM_Tickets_Log WITH (NOLOCK) where TLog_Ticket_Id =@TID order by TLog_Updated_Date_Time DESC";
            //dt = objDB.DBExecDataTable(sQuery);
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }
        public DataTable getInternalNotes(int TID)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TID", TID)
            };
            string sQuery = "select CONVERT(VARCHAR(20), INT_CreatedOn) + '-' + INT_CreatedBy + CHAR(13) + CHAR(10) +INT_Message as notes from dbo.RTM_Ticket_Internal_Notes WITH (NOLOCK) where INT_TID =@TID order by INT_CreatedOn DESC";
            //dt = objDB.DBExecDataTable(sQuery);
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable DownloadFile(int id)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };
            string sQuery = "select * from dbo.RTM_Ticket_Attachments where I_ID=@id";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           // dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetLeadEmails(int TID)
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TID", TID)
            };
            string sQuery = "select * from RTM_User_List, dbo.RTM_Access_Level where UL_Employee_Id = AL_EmployeeId and UL_Team_ID =@TID and AL_AccessLevel ='3' and UL_User_Status =1";
            //dt = objDB.DBExecDataTable(sQuery);
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTicketStatus()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "select * from RTM_TicketStatus WITH (NOLOCK) order by TS_Order";
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetTicketType()
        {
            DataTable dt = new DataTable();
            SqlDBHelper objDB = new SqlDBHelper();
            string sQuery = "select * from RTM_TicketType WITH (NOLOCK) order by TP_Order";
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }
    }
}
