using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAL;

namespace BAL
{
    public class clsAccount
    {
        DataTable dt = new DataTable();
        SqlDBHelper objDB = new SqlDBHelper();
        string sQuery;
        public DataTable LoadCorePlatform()
        {
            sQuery = "select DISTINCT CL_Product from RTM_Client_List where (CL_Product is not null or CL_Product <> '')  order BY CL_Product";
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable CheckJobCode(string jobCode, string teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@jobCode",jobCode),
                new SqlParameter("@teamId", teamId)
            };
            sQuery = "Select * from RTM_Client_List where CL_TSheetClient= @jobCode and CL_TeamId=@teamId";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool AddNewClient(string teamId, string client, string jobCode, string corePlatform, string clientCode, string createdBy)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId",teamId),
                new SqlParameter("@client",client),
                new SqlParameter("@jobCode",jobCode),
                new SqlParameter("@corePlatform",corePlatform),
                new SqlParameter("@clientCode",clientCode),
                new SqlParameter("@createdBy",createdBy),
                new SqlParameter("@createdOn",DateTime.Now),
                new SqlParameter("@status", "1")
            };

            sQuery = "insert into RTM_Client_List (CL_TeamId, CL_ClientName, CL_CreatedOn, CL_Status, CL_TSheetClient, CL_Product, CL_Code, CL_CreatedBy) values (@teamId, @client, @createdOn, @status, @jobCode, @corePlatform, @clientCode, @createdBy)";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable GetClientData(string teamId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId)
            };
            sQuery = "SELECT T_TeamName, CL_ClientName, CL_TSheetClient, CL_Product, CL_Code, CASE CL_Status WHEN '1' THEN 'Active' ELSE 'Deactive' END as [status] from RTM_Client_List, RTM_Team_List where CL_TeamId = T_ID and CL_TeamId = @teamId Order By CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool AddNewUser(string teamId, string empId, string username, string systemName, string gender, string email, string createdBy, string managerId, string managerEmail, string Hourly, string DOJ, string UL_Emp_Type, string UL_Employee_Number, string UL_Payroll_Id)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId",teamId),
               new SqlParameter("@empId",empId),
               new SqlParameter("@username",username),
               new SqlParameter("@systemName",systemName),
               new SqlParameter("@gender",gender),
               new SqlParameter("@email",email),
               new SqlParameter("@createdBy",createdBy),
               new SqlParameter("@createdOn",DateTime.Now),
               new SqlParameter("@status","1"),
               new SqlParameter("@userstatus","1"),
               new SqlParameter("@reminder","60"),
               new SqlParameter("@SLogin","2015-12-17 08:00:00.000"),
               new SqlParameter("@SLogout","2015-12-17 17:00:00.000"),
               new SqlParameter("@managerId", managerId),
               new SqlParameter("@managerEmail", managerEmail),
               new SqlParameter("@Hourly", Hourly),
               new SqlParameter("@DOJ", DOJ),
               new SqlParameter("@UL_Emp_Type", UL_Emp_Type),
               new SqlParameter("@UL_Employee_Number", UL_Employee_Number),
               new SqlParameter("@UL_Payroll_Id", UL_Payroll_Id)
            };

            sQuery = "insert into dbo.RTM_User_List (UL_Team_Id, UL_Employee_Id, UL_User_Name, UL_System_User_Name, UL_CreatedBy, UL_CreatedOn, UL_Status, UL_Gender, UL_User_Status, UL_EmailId, UL_Reminder_Duration, UL_SCH_Login, UL_SCH_Logout, UL_RepMgrId, UL_RepMgrEmail,UL_Hourly,UL_DOJ,UL_EmployeeType,UL_Employee_Number,UL_Payroll_Id) " +
                                             "values(@teamId, @empId, @username,@systemName, @createdBy, @createdOn, @status, @gender, @userstatus, @email, @reminder, @SLogin, @SLogout, @managerId,@managerEmail,@Hourly,@DOJ,@UL_Emp_Type,@UL_Employee_Number,@UL_Payroll_Id)";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool UpdateUser(string empId, string sysName, string managerId, string managerEmail, string teamId, string createdBy, string UL_Gender, string UL_Hourly, string UL_DOJ, string UL_Emp_Type, string UL_Employee_Number, string UL_Payroll_Id)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId",teamId),
                   
               new SqlParameter("@systemName",sysName),              
               new SqlParameter("@createdBy",createdBy),
               new SqlParameter("@createdOn",DateTime.Now),               
               new SqlParameter("@userstatus","1"),
               new SqlParameter("@managerId", managerId),
               new SqlParameter("@managerEmail", managerEmail),
               new SqlParameter("@UL_Gender",UL_Gender),
               new SqlParameter("@UL_Hourly",UL_Hourly),
               new SqlParameter("@UL_DOJ", UL_DOJ),
               new SqlParameter("@UL_Emp_Type", UL_Emp_Type),
               new SqlParameter("@empId",empId),
               new SqlParameter("@UL_Employee_Number", UL_Employee_Number),
               new SqlParameter("@UL_Payroll_Id", UL_Payroll_Id)
            };

          //  sQuery = "update RTM_User_List SET UL_Team_Id=@teamId,UL_System_User_Name='" + sysName + "',UL_ModifiedBy='" + createdBy + "', UL_ModifiedOn='" + DateTime.Now + "',UL_User_Status='1', UL_RepMgrId='" + managerId + "', UL_RepMgrEmail='" + managerEmail + "',UL_Gender='" + UL_Gender + "', UL_Hourly='" + UL_Hourly + "', UL_DOJ='" + UL_DOJ + "', UL_EmployeeType='" + UL_Emp_Type + "' where UL_Employee_Id='" + empId + "'";
            sQuery = "update RTM_User_List SET UL_Team_Id=@teamId,UL_System_User_Name=@systemName,UL_ModifiedBy=@createdBy, UL_ModifiedOn=@createdOn,UL_User_Status=@userstatus, UL_RepMgrId=@managerId, UL_RepMgrEmail=@managerEmail,UL_Gender=@UL_Gender, UL_Hourly=@UL_Hourly, UL_DOJ=@UL_DOJ, UL_EmployeeType=@UL_Emp_Type,UL_Employee_Number=@UL_Employee_Number,UL_Payroll_Id=@UL_Payroll_Id where UL_Employee_Id=@empId";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool AddAccessLevel(string empId, string createdBy)
        {
             bool result;
             SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@empId",empId),
                 new SqlParameter("@createdBy",createdBy),
               new SqlParameter("@createdOn",DateTime.Now),
               new SqlParameter("@access", "4"),
               new SqlParameter("@status", 1)
            };

             sQuery = "insert into dbo.RTM_Access_Level (AL_EmployeeId, AL_AccessLevel, AL_CreatedBy, AL_CreatedOn, AL_Status) values (@empId,@access,@createdBy,@createdOn,@status)";
             result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
             return result;
        }

        public bool CheckEmpId(string empId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@empId",empId)
            };
            sQuery = "select * from dbo.RTM_User_List where UL_Employee_Id= @empId";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            if (dt.Rows.Count > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool CheckUserName(string userName)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userName",userName)
            };
            sQuery = "select * from dbo.RTM_User_List where UL_User_Name= @userName and UL_User_Status=1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            if (dt.Rows.Count > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public DataTable GetUsers(string teamId)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teamId",teamId)
            };

            sQuery = "select * from RTM_User_List where UL_Team_Id=@teamId";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable checkMasterClents(string jc1, string jc0)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@jc1",jc1),
                new SqlParameter("@jc0", jc0)
            };

            sQuery = "select * from dbo.RTM_MasterClient_Db where MCD_ClientName_JCOne= @jc1 and MCD_JCZero= @jc0";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool AddMasterClients(string jl1, string jl0, string projectPlDept, string clientCode, string clientName, string createdBy)
        {
            bool result;

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@jl1", jl1),
                new SqlParameter("@jl0", jl0),
                new SqlParameter("@projectPlDept", projectPlDept),
                new SqlParameter("@clientCode", clientCode),
                new SqlParameter("@clientName", clientName),
                new SqlParameter("@createdBy", createdBy),
                new SqlParameter("@createdOn", DateTime.Now),
                new SqlParameter("@status", 1)
            };

            sQuery = "insert into RTM_MasterClient_Db (MCD_ClientName_JCOne, MCD_JCZero, MCD_ProjectPLDept, MCD_ClientCode, MCD_ClientName_Generic, MCD_AddedDate, MCD_AddedBy, MCD_Status) Values (@jl1, @jl0, @projectPlDept, @clientCode, @clientName, @createdOn, @createdBy, @status)";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable GetMasterClients()
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@status", 1)
            };
            sQuery = "Select MCD_Id, MCD_ClientName_JCOne, MCD_JCZero, MCD_ClientCode from RTM_MasterClient_Db where MCD_Status=@status order By MCD_ClientName_JCOne";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetMasterClientsNew(string jobCode1)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@jobCode1", jobCode1),
                new SqlParameter("@status", 1)
            };
            sQuery = "Select MCD_Id, MCD_ClientName_JCOne, MCD_JCZero, MCD_ClientCode from RTM_MasterClient_Db where MCD_ClientName_JCOne=@jobCode1 and MCD_Status=@status order By MCD_ClientName_JCOne";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool UpdateClientStatus(string _team, string jobCode, string status)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@team", _team),
                new SqlParameter("@jobCode", jobCode),
                new SqlParameter("@status", status)
            };

            sQuery = "update RTM_Client_List set CL_Status=@status where CL_TSheetClient=@jobCode and CL_TeamId=@team";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable SearchHRIS(string empId, string lastName, string searchBy)
        {
            dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@empId", empId),
                new SqlParameter("@lastName", lastName+"%")
            };

            if (searchBy == "empId")
            {
                sQuery = "select M1.MUL_EmployeeId, M1.MUL_FirstName, M1.MUL_LastName, M1.MUL_EmailId, M1.MUL_ManagerID,M1.MUL_ManagerEmail_id, M2.MUL_FirstName +' '+ M2.MUL_LastName as [Manager], UL_Employee_Id, UL_Version, M1.MUL_ActiveStatus, UL_System_User_Name from RTM_Master_UserList M1 left join RTM_Master_UserList M2 on M1.MUL_ManagerID= M2.MUL_EmployeeId left join RTM_User_List on M1.MUL_EmployeeId = UL_Employee_Id where M1.MUL_ActiveStatus = 1 and M1.MUL_EmployeeId=@empId";
            }
            else
            {
                sQuery = "select M1.MUL_EmployeeId, M1.MUL_FirstName, M1.MUL_LastName, M1.MUL_EmailId, M1.MUL_ManagerID,M1.MUL_ManagerEmail_id, M2.MUL_FirstName +' '+ M2.MUL_LastName as [Manager], UL_Employee_Id, UL_Version, M1.MUL_ActiveStatus, UL_System_User_Name from RTM_Master_UserList M1 left join RTM_Master_UserList M2 on M1.MUL_ManagerID= M2.MUL_EmployeeId left join RTM_User_List on M1.MUL_EmployeeId = UL_Employee_Id where M1.MUL_ActiveStatus = 1 and M1.MUL_LastName Like @lastName";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable CheckExistingEmployee()
        {
            DataTable dt = new DataTable();

            sQuery = "select * from RTM_Master_UserList";

            dt = objDB.DBExecDataTable(sQuery);

            return dt;
        }

        public bool DeactivateAllEmployees()
        {
            
            sQuery = "update RTM_Master_UserList set MUL_ActiveStatus =0";
            bool result = objDB.DBExecuteNoNQuery(sQuery);
            return result;

        }

        public bool UpdateDepartment(string employeeId, string managerId)
        {
           
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@employeeId", employeeId),
                new SqlParameter("@managerId", managerId)
            };
            sQuery = "update RTM_Master_UserList set MUL_ActiveStatus =1, MUL_ManagerID=@managerId where MUL_EmployeeId=@employeeId";
            bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool UpdateManagerEmail()
        {
            sQuery = "Update ul Set ul.MUL_ManagerEmail_id = mu.MUL_EmailId From RTM_Master_UserList ul Inner join RTM_Master_UserList mu on ul.MUL_ManagerID = mu.MUL_EmployeeId";
            bool result = objDB.DBExecuteNoNQuery(sQuery);
            return result;
        }

        public bool AddMasterUser(string empId, string first, string last, string emailId, string managerId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@empId", empId),
                new SqlParameter("@first", first),
                new SqlParameter("@last", last),
                new SqlParameter("@emailId", emailId),
                new SqlParameter("@managerId", managerId),
                new SqlParameter("@createdOn", DateTime.Now),
                new SqlParameter("@status", 1)
            };
            string sQuery = "insert into RTM_Master_UserList (MUL_EmployeeId,MUL_FirstName,MUL_LastName,MUL_EmailId,MUL_ManagerID,MUL_CreatedOn,MUL_ActiveStatus) " +
                             "values(@empId, @first,@last,@emailId,@managerId,@createdOn,@status)";
            bool result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
            
        }

        public DataTable EmployeeList()
        {
            DataTable dt = new DataTable();

            sQuery = "select UL_User_Name as [User Name], UL_Employee_Id as [Employee Id], UL_EmailId as [Email Id], T_TeamName as [Team], UL_RepMgrEmail as [Reporting Manager] from RTM_User_List WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on UL_Team_Id= T_ID where UL_User_Status=1 order by UL_User_Name";

            dt = objDB.DBExecDataTable(sQuery);

            return dt;
        }
    }
}
