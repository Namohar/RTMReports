using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using DAL;
using System.Data.SqlClient;

namespace BAL
{
    public class clsTeam
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        string sQuery;
        public string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "RTMEDURL";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "RTMEDURL";
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                
            }
            catch (Exception)
            {
                
            }
            return cipherText;
        }

        public DataTable LoadEmp(string _access, string _teamId, string _user)
        {
            dt = new DataTable();
           

            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@user", _user)
            };
            if (_access == "1")
            {
                sQuery = "SELECT * FROM RTM_User_List WITH (NOLOCK) where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else if (_access == "2")
            {
                sQuery = "select * from RTM_User_List WITH (NOLOCK) where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else if (_access == "3")
            {
                sQuery = "select * from RTM_User_List WITH (NOLOCK) where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
                //sQuery = "SELECT * FROM RTM_User_List where UL_System_User_Name=@user and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else
            {
                sQuery = "SELECT * FROM RTM_User_List WITH (NOLOCK) where UL_System_User_Name=@user and UL_User_Status =1 ORDER BY UL_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            return dt;

        }

        public DataTable LoadEmpForEstimate(string _access, string _teamId, string _user)
        {
            dt = new DataTable();


            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId),
               new SqlParameter("@user", _user)
            };
            if (_access == "1")
            {
                sQuery = "SELECT * FROM RTM_User_List where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else if (_access == "2")
            {
                sQuery = "select * from RTM_User_List where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else if (_access == "3")
            {
                //sQuery = "select * from RTM_User_List where UL_Team_Id =@teamId and UL_User_Status =1 ORDER BY UL_User_Name";
                sQuery = "SELECT * FROM RTM_User_List where UL_System_User_Name=@user and UL_User_Status =1 ORDER BY UL_User_Name";
            }
            else
            {
                sQuery = "SELECT * FROM RTM_User_List where UL_System_User_Name=@user and UL_User_Status =1 ORDER BY UL_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);

            return dt;

        }

        public DataSet LoadClient(string _teamId, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId)
            };

            sQuery = "select * from RTM_Client_List where CL_TeamId= @teamId and CL_Status=1 and CL_ClientName !='Personal/Sick Time' and CL_ClientName!='Public Holiday' and CL_ClientName !='Inclement Weather' and CL_ClientName!='Vacation' and CL_ClientName !='Bereavement' and CL_ClientName!='Jury Duty' and CL_ClientName!='Maternity Leave' and CL_ClientName!='Paternity Leave' and CL_ClientName!='Medical Leave' and CL_ClientName!='Comp off' order by CL_ClientName";

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;
        }

        public DataSet LoadMultiClient(string _teamId, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId)
            };

            sQuery = "select * from RTM_Client_List where CL_TeamId=@teamId and CL_Status=1 and CL_ClientName <> 'Other' order by CL_ClientName";

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;
        }

        public DataTable LoadOtherClient(string _teamId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId)
            };

            sQuery = "select * from RTM_Client_List where CL_TeamId= @teamId and CL_Status=1 and CL_ClientName = 'Other'";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }



        public DataSet LoadTask(string _teamId, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@teamId", _teamId)
            };

            sQuery = "select * from RTM_Task_List where TL_TeamId = @teamId and TL_Status =1 order by TL_Task";

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;
        }

        public DataSet LoadSubTask(string taskId, string dtName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@taskId", taskId)
            };

            sQuery = "select * from RTM_SubTask_List where STL_Task_Id= @taskId and STL_ViewStatus =1 order by STL_SubTask";

            ds = objDB.DSExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters, dtName);
            return ds;
        }

        public DataTable GetServiceCode(string _taskId, string _taskName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@taskId", _taskId),
               new SqlParameter("@taskName", _taskName)
            };

            sQuery = "select STL_ServiceCode from RTM_SubTask_List where STL_SubTask =@taskName and STL_Id =@taskId";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable checkRecords(string _clientId, string _taskId, string _subTaskId, string _teamID, string _date, string _user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@clientId", _clientId),
               new SqlParameter("@taskId", _taskId),
               new SqlParameter("@subTaskId", _subTaskId),
               new SqlParameter("@teamID", _teamID),
               new SqlParameter("@date", _date),
               new SqlParameter("@user", _user)
            };

            sQuery = "select * from RTM_Estimation where EST_ClientId=@clientId and EST_TaskId=@taskId and EST_SubTaskId=@subTaskId and EST_TeamId=@teamID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, EST_Date)))=@date and EST_UserName=@user";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable PreviousEstimates(string username, string date)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@username", username),
               new SqlParameter("@date", date)
            };

            sQuery = "select Distinct CL_ClientName,EST_ClientId, TL_Task ,EST_TaskId,  STL_SubTask, EST_SubTaskId from dbo.RTM_Estimation, dbo.RTM_Client_List, dbo.RTM_Task_List,dbo.RTM_SubTask_List  where EST_ClientId = CL_ID and EST_TaskId = TL_ID and EST_SubTaskId = STL_ID and EST_UserName = @username and EST_CreatedOn >= @date";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable DisplayEstimateRecords(string access, string date, string teamId, string user)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@date", date),
               new SqlParameter("@teamId", teamId)
            };
            if (access == "1")
            {
                sQuery = "select EST_ID, EST_UserName, CL_ClientName, TL_Task, STL_SubTask, EST_Duration,EST_Comments, CONVERT(VARCHAR(10),EST_Date,101) as EST_Date from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID left join rtm_subtask_list on EST_SubTaskId = STL_ID where EST_Date >= @date and EST_TeamId=@teamId ";
            }
            else
            {
                sQuery = "select EST_ID, EST_UserName, CL_ClientName, TL_Task, STL_SubTask, EST_Duration,EST_Comments, CONVERT(VARCHAR(10),EST_Date,101) as EST_Date from RTM_Estimation left join rtm_client_list on EST_ClientId = CL_ID left join rtm_task_list on EST_TaskId = TL_ID left join rtm_subtask_list on EST_SubTaskId = STL_ID where EST_Date >= @date and EST_UserName =@user and EST_TeamId=@teamId";
            }
            
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool DeleteEstRecord(string recId)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@recId", recId)
            };

            sQuery = "Delete from RTM_Estimation where EST_ID=@recId";

            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool InsertEstimateRecord(string teamId, string userId, string userName, string clientId, string taskId, string subTaskId, string duration, string date, string createdOn, string comments)
        {
            bool result;

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@teamId", teamId),
                 new SqlParameter("@userId", userId),
                 new SqlParameter("@userName", userName),
                 new SqlParameter("@clientId", clientId),
                 new SqlParameter("@taskId", taskId),
                 new SqlParameter("@subTaskId", subTaskId),
                 new SqlParameter("@duration", duration),
                 new SqlParameter("@date", date),
                 new SqlParameter("@createdOn", createdOn),
                 new SqlParameter("@comments", comments)
            };
            sQuery = "insert into RTM_Estimation (EST_TeamId,EST_UserId, EST_UserName, EST_ClientId, EST_TaskId, EST_SubTaskId, EST_Duration, EST_Date, EST_CreatedOn, EST_Comments) values(@teamId,@userId, @userName, @clientId, @taskId, @subTaskId, @duration, @date, @createdOn, @comments)";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public bool UpdateEstimateRecord(string id, string client, string task, string subTask, string duration, string comments)
        {
            bool result;
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@id",id),
               new SqlParameter("@client", client),
               new SqlParameter("@task", task),
               new SqlParameter("@subTask", subTask),
               new SqlParameter("@duration", duration),
               new SqlParameter("@comments", comments)
            };

            sQuery = "UPDATE RTM_Estimation SET EST_ClientId=@client, EST_TaskId=@task, EST_SubTaskId=@subTask, EST_Duration=@duration, EST_Comments=@comments where EST_ID=@id";

            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            return result;
        }

        public DataTable GetTeams(string location)
        {
            dt = new DataTable();

            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@location", location)
           };

            sQuery = "SELECT T_ID, T_TeamName, T_Location, T_Manager, CASE WHEN T_Active = 1 THEN 'yes' ELSE 'no' END as T_Active From RTM_Team_List WITH (NOLOCK) where T_Location = @location order by T_TeamName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable CheckManager(string manager)
        {
            dt = new DataTable();

            SqlParameter[] parameters = new SqlParameter[]
           {
               new SqlParameter("@manager", manager)
           };

            sQuery = "SELECT UL_User_Name from RTM_User_List WITH (NOLOCK) WHERE UL_User_Name =@manager and UL_User_Status = 1";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public bool InsertTeam(string teamName, string location, string manager, string status, string createdBy)
        {
            bool result;
            string serverIp;
            string convertFactor;
            string autolock;
            int preference;

            switch (location)
            {
                case "IND":
                    serverIp = "10.80.20.84";
                    convertFactor = "5.5";
                    autolock = "300";
                    preference = 1;
                    break;

                case "CHN":
                    serverIp = "10.88.20.116";
                    convertFactor = "8";
                    autolock = "600";
                    preference = 2;
                    break;

                case "OTH":
                    serverIp = "dc2corpdevops.corp.tangoe.com";
                    convertFactor = "-5";
                    autolock = "1000";
                    preference = 2;
                    break;

                default:
                    serverIp = "10.80.20.84";
                    convertFactor = "5.5";
                    autolock = "300";
                    preference = 1;
                    break;
            }

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@teamName", teamName),
                new SqlParameter("@location", location),
                new SqlParameter("@manager", manager),
                new SqlParameter("@serverIp", serverIp),
                new SqlParameter("@convertFactor", convertFactor),
                new SqlParameter("@autolock", autolock),
                new SqlParameter("@preference", preference),
                new SqlParameter("@status", status),
                new SqlParameter("@createdBy", createdBy),
                new SqlParameter("@createdOn", DateTime.Now)
            };

            sQuery = "Insert into RTM_Team_List (T_TeamName, T_Location, T_Manager, T_Active, T_SERVER_IP, T_ConvertFactor, T_AutoLockInSec, T_Preference, T_Status, T_CreatedBy, T_CreatedOn) VALUES (@teamName, @location, @manager, @status, @serverIp, @convertFactor, @autolock, @preference, '0', @createdBy, @createdOn)";
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, param);
            return result;
        }

        public bool UpdateTeam(string id, string teamName, string location, string manager, string status, string createdBy)
        {
            bool result;
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@teamName", teamName),
                new SqlParameter("@location", location),
                new SqlParameter("@manager", manager),
                new SqlParameter("@status", status),
                new SqlParameter("@createdBy", createdBy),
                new SqlParameter("@createdOn", DateTime.Now)
            };

            sQuery = "UPDATE RTM_Team_List SET T_TeamName = @teamName, T_Location =@location , T_Manager=@manager, T_Active= @status, T_ModifiedBy = @createdBy, T_ModifiedOn = @createdOn WHERE T_ID = @id";

            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, param);
            return result;
        }

        public bool UpdateTeamStatus(string status, int id)
        {
            bool result;

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@status", status),
                new SqlParameter("@id", id)
            };

            sQuery = "UPDATE RTM_Team_List SET T_Active = @status WHERE T_ID = @id";

            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, param);
            return result;
        }

        public DataTable CheckTeam(string team, string location, string type)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@team", team),
                new SqlParameter("@location", location),
                new SqlParameter("@type", type)
            };

            if (type == "0")
            {
                sQuery = "Select T_TeamName from RTM_Team_List WITH (NOLOCK) WHERE T_Location = @location and T_TeamName= @team";
            }
            else
            {
                sQuery = "Select T_TeamName from RTM_Team_List WITH (NOLOCK) WHERE T_Location = @location and T_TeamName= @team and T_ID <> @type";
            }
            
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable GetTasks(string teamId)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@teamId", teamId)
            };

            sQuery = "Select TL_ID as [TaskId], T_TeamName as [TeamName], TL_Task as [TaskName], CASE WHEN TL_Status =1 THEN 'yes' ELSE 'no' END as [IsActive] from RTM_Task_List WITH (NOLOCK) left join RTM_Team_List WITH (NOLOCK) on TL_TeamId = T_ID WHERE TL_TeamId = @teamId ORDER BY TL_Task";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable CheckTask(string team, string task, string id)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {                
                new SqlParameter("@team", team),
                new SqlParameter("@task", task),
                new SqlParameter("@id", id)
            };

            if (id == "0")
            {
                sQuery = "select TL_Task from RTM_Task_List with (nolock) where TL_TeamId = @team and TL_Task = @task";
            }
            else
            {
                sQuery = "select TL_Task from RTM_Task_List with (nolock) where TL_TeamId = @team and TL_Task = @task and TL_ID <> @id";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public bool InsertTask(string team, string task, string active, string id, string createdBy)
        {
            bool result;

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@team", team),
                new SqlParameter("@task", task),
                new SqlParameter("@active", active),
                new SqlParameter("@id",  id),
                new SqlParameter("@createdBy", createdBy),
                new SqlParameter("@createdOn", DateTime.Now)
            };

            if (id == "0")
            {
                sQuery = "insert into RTM_Task_List (TL_TeamId, TL_Task, TL_CreatedBy, TL_CreatedOn, TL_Status) VALUES (@team, @task, @createdBy,@createdOn, @active) ";
            }
            else
            {
                sQuery = "UPDATE RTM_Task_List SET TL_TeamId= @team, TL_Task= @task, TL_ModifiedBy =@createdBy, TL_ModifiedOn = @createdOn, TL_Status= @active WHERE TL_ID = @id";
            }
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, param);
            return result;
        }

        public DataTable GetSubTasks(string taskId)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@taskId", taskId)
            };

            sQuery = "select STL_ID as [SubtaskId], STL_Subtask as [Subtask], STL_ServiceCode as [ServiceCode], TL_Task as [Task], CASE WHEN STL_ViewStatus = 1 then 'yes' ELSE 'no' END as [IsActive] from RTM_SubTask_List with (nolock) left join RTM_Task_List with (nolock) on STL_Task_Id = TL_ID WHERE STL_Task_Id = @taskId order by STL_Subtask";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable CheckSubTask(string task, string subtask, string id)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]
            {                
                new SqlParameter("@subtask", subtask),
                new SqlParameter("@task", task),
                new SqlParameter("@id", id)
            };

            if (id == "0")
            {
                sQuery = "select STL_SubTask from RTM_SubTask_List with (nolock) where STL_Task_Id = @task and STL_SubTask = @subtask";
            }
            else
            {
                sQuery = "select STL_SubTask from RTM_SubTask_List with (nolock) where STL_Task_Id = @task and STL_SubTask = @subtask and STL_ID <> @id";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public bool AddSubtask(string id, string taskId, string subtask, string servicecode, string status, string CreatedBy)
        {
            bool result;

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@taskId", taskId),
                new SqlParameter("@subtask", subtask),
                new SqlParameter("@servicecode", servicecode),
                new SqlParameter("@status", status),
                new SqlParameter("@CreatedBy", CreatedBy),
                new SqlParameter("@createdOn", DateTime.Now)
            };

            if (id == "0")
            {
                sQuery = "insert into RTM_SubTask_List (STL_Task_Id, STL_SubTask, STL_ServiceCode, STL_CreatedBy, STL_CreatedOn, STL_ViewStatus) VALUES (@taskId, @subtask, @servicecode, @CreatedBy, @createdOn, @status)";
            }
            else
            {
                sQuery = "UPDATE RTM_SubTask_List SET STL_Task_Id = @taskId, STL_SubTask = @subtask, STL_ServiceCode = @servicecode, STL_ModifiedBy = @CreatedBy, STL_ModifiedOn= @createdOn, STL_ViewStatus = @status WHERE STL_ID = @id";
            }
            result = objDB.ExecuteNonQuery(sQuery, CommandType.Text, param);

            return result;
        }
    }
}
