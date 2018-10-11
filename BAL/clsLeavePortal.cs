using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace BAL
{
    public class clsLeavePortal
    {
        DataTable dt = new DataTable();
        public DataTable GetLeaves(string empId, string from, string to)
        {
            dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.AppSettings["conLeaveData"].ToString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("select employee_id, from_date, to_date, number_of_days, leave_status, leave_type from employee_leave_details_view where employee_id = '" + empId + "'  and (leave_type = 1 or leave_type = 3 or leave_type = 4) order by from_date desc LIMIT 10"))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}
