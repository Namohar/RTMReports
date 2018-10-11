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

   public class clsAuditTrail 
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        string sQuery;


        public void AddLogs(string UA_Action, string UA_Table, string UA_Action_By, DateTime UA_Action_Time)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@UA_Action",UA_Action),
               new SqlParameter("@UA_Table",UA_Table),
               new SqlParameter("@UA_Action_By",UA_Action_By),
               new SqlParameter("@UA_Action_Time",UA_Action_Time),
            };

                sQuery = "insert into dbo.RTM_Audit_Trail (UA_Action, UA_Table, UA_Action_By, UA_Action_Time) " +
                                                 "values(@UA_Action, @UA_Table, @UA_Action_By,@UA_Action_Time)";
                objDB.ExecuteNonQuery(sQuery, CommandType.Text, parameters);
            }

            catch (Exception ex)
            {

            }
        }
    }
}
