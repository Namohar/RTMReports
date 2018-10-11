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
    public class clsTSheetReports
    {
        SqlDBHelper objDB = new SqlDBHelper();
        DataTable dt = new DataTable();
        string sQuery;

        public DataTable getResultClient(string qryWhereCond, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };

            sQuery = "SELECT dbo.RTM_Client_List.CL_ClientName as [Client Name] , SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) AS Hours " + 
                     " FROM            dbo.RTM_Records INNER JOIN " +
                     "    dbo.RTM_Client_List ON dbo.RTM_Records.R_Client = dbo.RTM_Client_List.CL_ID " +
                     " WHERE  " +
                     qryWhereCond +
                      " AND  (dbo.RTM_Client_List.CL_ClientName <> 'Other' AND dbo.RTM_Client_List.CL_ClientName <> 'Internal') " +
                     " AND (dbo.RTM_Records.R_Start_Date_Time >= @from and dbo.RTM_Records.R_Start_Date_Time <= @to) " +
                     " AND dbo.RTM_Records.R_Duration <> 'HH:MM:SS' " +
                     " GROUP BY dbo.RTM_Client_List.CL_ClientName " +
                     " ORDER BY dbo.RTM_Client_List.CL_ClientName";
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            return dt;
        }

        public DataTable getResultTeams(string qryWhereCond, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };

            DataTable dtRecords = new DataTable();
            DataTable dtLogActions = new DataTable();
            DataTable dtLeaveDetails = new DataTable();
            DataTable dtFilled = new DataTable();
            dtFilled.Clear();
            dtFilled.Columns.Add("T_TeamName", typeof(string));
            dtFilled.Columns.Add("Hours", typeof(decimal));

            sQuery = "SELECT        TOP (100) PERCENT dbo.RTM_Team_List.T_TeamName,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [hours] " + 
                      " FROM            dbo.RTM_Team_List INNER JOIN " +
                      "   dbo.RTM_Records ON dbo.RTM_Team_List.T_ID = dbo.RTM_Records.R_TeamId " +
                     " WHERE  " +
                     qryWhereCond +
                     " AND (dbo.RTM_Records.R_Start_Date_Time >= @from and dbo.RTM_Records.R_Start_Date_Time <= @to) " +
                     " AND dbo.RTM_Records.R_Duration <> 'HH:MM:SS' " +
                     " GROUP BY dbo.RTM_Team_List.T_TeamName " +
                     " ORDER BY dbo.RTM_Team_List.T_TeamName";
            dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
           // dtRecords = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);



            SqlParameter[] parameters1 = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };

            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Records.R_TeamId", "dbo.RTM_Log_Actions.LA_TeamId");
            sQuery = "SELECT        TOP (100) PERCENT dbo.RTM_Team_List.T_TeamName,SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as [hours] " + 
                      " FROM            dbo.RTM_Log_Actions INNER JOIN  " +
                      "   dbo.RTM_Team_List ON dbo.RTM_Log_Actions.LA_TeamId = dbo.RTM_Team_List.T_ID " +
                              " WHERE  " +
                          qryWhereCond +
                     " AND (dbo.RTM_Log_Actions.LA_Start_Date_Time >=  @from and dbo.RTM_Log_Actions.LA_Start_Date_Time <= @to) " +
                     " GROUP BY dbo.RTM_Team_List.T_TeamName " +
                     " ORDER BY dbo.RTM_Team_List.T_TeamName";
            //dtLogActions = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLogActions = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1);


            SqlParameter[] parameters2 = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };

            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Log_Actions.LA_TeamId", "dbo.RTM_Team_List.T_ID");
            sQuery = " SELECT        TOP (100) PERCENT dbo.RTM_Team_List.T_TeamName, sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as Decimal(10, 2)) end) AS Hours " +
                    " FROM            dbo.RTM_LeaveDetails INNER JOIN " +
                    " dbo.RTM_User_List ON dbo.RTM_LeaveDetails.LD_UserName = dbo.RTM_User_List.UL_User_Name INNER JOIN " +
                    " dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID " +
                       " WHERE  " +
                   qryWhereCond +
              " AND (dbo.RTM_LeaveDetails.LD_Date >=  @from and dbo.RTM_LeaveDetails.LD_Date <=  @to) " +
              " GROUP BY dbo.RTM_Team_List.T_TeamName " +
              " ORDER BY dbo.RTM_Team_List.T_TeamName";
         //   dtLeaveDetails = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLeaveDetails = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);


            dtFilled.Rows.Clear();

            foreach (DataRow dr1 in dtRecords.Rows)
            {
                dtFilled.Rows.Add(dr1["T_TeamName"], dr1["Hours"]);
            }

            foreach (DataRow dr2 in dtLogActions.Rows)
            {
                dtFilled.Rows.Add(dr2["T_TeamName"], dr2["Hours"]);
            }

            foreach (DataRow dr3 in dtLeaveDetails.Rows)
            {
                dtFilled.Rows.Add(dr3["T_TeamName"], dr3["Hours"]);
            }

            DataTable dtNew = new DataTable();
            DataTable dtUnique = new DataTable();

            dtUnique.Clear();

            dtUnique = dtFilled.DefaultView.ToTable(true, "T_TeamName");

            decimal sum = 0;

            dtNew.Clear();
            dtNew.Columns.Add("Group", typeof(string));
            dtNew.Columns.Add("Hours", typeof(decimal));

            foreach (DataRow dr4 in dtUnique.Rows)
            {
                foreach (DataRow dr5 in dtFilled.Rows)
                {
                    if (dr4["T_TeamName"].ToString().ToLower() == dr5["T_TeamName"].ToString().ToLower())
                    {
                        if ((dr5["Hours"].ToString().Trim()) != "")
                        {
                            sum = sum + Convert.ToDecimal(dr5["Hours"]);
                        }

                     
                    }
                }
                dtNew.Rows.Add(dr4["T_TeamName"], sum);
                sum = 0;
            }
            return dtNew;
        }

        public DataTable getResultDate(string qryWhereCond, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };


            DataTable dtRecords = new DataTable();
            DataTable dtLogActions = new DataTable();
            DataTable dtLeaveDetails = new DataTable();
            DataTable dtFilled = new DataTable();
            dtFilled.Clear();
            dtFilled.Columns.Add("Date", typeof(DateTime));
            dtFilled.Columns.Add("Hours", typeof(decimal));

            sQuery = "SELECT        TOP (100) PERCENT cast(dbo.RTM_Records.R_Start_Date_Time as date) as Date, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [hours] " + 
                     " FROM            dbo.RTM_Records INNER JOIN " +
                     "   dbo.RTM_Team_List ON dbo.RTM_Records.R_TeamId = dbo.RTM_Team_List.T_ID " +
                     " WHERE  " +
                     qryWhereCond +
                     " AND (dbo.RTM_Records.R_Start_Date_Time >= @from and dbo.RTM_Records.R_Start_Date_Time <= @to) " +
                     " AND dbo.RTM_Records.R_Duration <> 'HH:MM:SS' " +
                     " GROUP BY cast(dbo.RTM_Records.R_Start_Date_Time as date) " +
                     " ORDER BY cast(dbo.RTM_Records.R_Start_Date_Time as date)";
          //  dtRecords = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);


            SqlParameter[] parameters1 = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };
            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Records.R_TeamId", "dbo.RTM_Log_Actions.LA_TeamId");
            sQuery = "SELECT        TOP (100) PERCENT CAST(dbo.RTM_Log_Actions.LA_Start_Date_Time AS date) AS Date, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as [hours] " + 
                       " FROM  dbo.RTM_Log_Actions INNER JOIN " +
                       "  dbo.RTM_Team_List ON dbo.RTM_Log_Actions.LA_TeamId = dbo.RTM_Team_List.T_ID " +
                              " WHERE  " +
                          qryWhereCond +
                     " AND (dbo.RTM_Log_Actions.LA_Start_Date_Time >= @from and dbo.RTM_Log_Actions.LA_Start_Date_Time <= @to) " +
                     " GROUP BY CAST(dbo.RTM_Log_Actions.LA_Start_Date_Time AS date) ";
                     //" ORDER BY dbo.RTM_Team_List.T_TeamName";
         //   dtLogActions = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLogActions = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1);


            SqlParameter[] parameters2 = new SqlParameter[]
            {
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };
            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Log_Actions.LA_TeamId", "dbo.RTM_Team_List.T_ID");
            sQuery = "SELECT       CAST( dbo.RTM_LeaveDetails.LD_Date AS date) AS Date, sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as Decimal) end) AS Hours " +
                     " FROM        dbo.RTM_LeaveDetails INNER JOIN " +
                      "   dbo.RTM_User_List ON dbo.RTM_LeaveDetails.LD_UserName = dbo.RTM_User_List.UL_User_Name INNER JOIN " +
                      "   dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID " +
                       " WHERE  " +
                   qryWhereCond +
              " AND (dbo.RTM_LeaveDetails.LD_Date >= @from and dbo.RTM_LeaveDetails.LD_Date <= @to) " +
              " group by  CAST( dbo.RTM_LeaveDetails.LD_Date AS date)";
              //" ORDER BY dbo.RTM_Team_List.T_TeamName";
           // dtLeaveDetails = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLeaveDetails = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            dtFilled.Rows.Clear();

            foreach (DataRow dr1 in dtRecords.Rows)
            {
                dtFilled.Rows.Add(dr1["Date"], dr1["Hours"]);
            }

            foreach (DataRow dr2 in dtLogActions.Rows)
            {
                dtFilled.Rows.Add(dr2["Date"], dr2["Hours"]);
            }

            foreach (DataRow dr3 in dtLeaveDetails.Rows)
            {
                dtFilled.Rows.Add(dr3["Date"], dr3["Hours"]);
            }

            DataTable dtNew = new DataTable();
            DataTable dtUnique = new DataTable();

            dtUnique.Clear();

            dtUnique = dtFilled.DefaultView.ToTable(true, "Date");

            decimal sum = 0;

            dtNew.Clear();
            dtNew.Columns.Add("Date", typeof(DateTime));
            dtNew.Columns.Add("Hours", typeof(decimal));

            foreach (DataRow dr4 in dtUnique.Rows)
            {
                foreach (DataRow dr5 in dtFilled.Rows)
                {
                    if (dr4["Date"].ToString().ToLower() == dr5["Date"].ToString().ToLower())
                    {
                        sum = sum + Convert.ToDecimal(dr5["Hours"]);
                    }
                }
                dtNew.Rows.Add(String.Format("{0:MM/dd/yyyy}",dr4["Date"]), sum);
 
                sum = 0;
            }
            return dtNew;

        }


        public DataTable getResultEmployee(string qryWhereCond, string fromDate, string toDate)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               //new SqlParameter("@user", user),
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };


            DataTable dtRecords = new DataTable();
            DataTable dtLogActions = new DataTable();
            DataTable dtLeaveDetails = new DataTable();
            DataTable dtFilled = new DataTable();
            dtFilled.Clear();
            dtFilled.Columns.Add("UL_User_Name", typeof(string));
            dtFilled.Columns.Add("UL_EmailId", typeof(string));
            dtFilled.Columns.Add("UL_Payroll_Id", typeof(string));
            dtFilled.Columns.Add("UL_Employee_Id", typeof(string));
            dtFilled.Columns.Add("Salaried", typeof(string));
            dtFilled.Columns.Add("Hours", typeof(string));

            sQuery = "SELECT TOP (100) PERCENT dbo.RTM_User_List.UL_User_Name as [Full Name], dbo.RTM_User_List.UL_EmailId as [Email], dbo.RTM_User_List.UL_Payroll_Id as [Payroll_Id]," +
					 "	 dbo.RTM_User_List.UL_Employee_Id as [Number], " +
                     "    case when UL_Hourly is Null then 1 end AS Salaried, " +
                     "	  SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as [hours] " +
                     " FROM            dbo.RTM_User_List INNER JOIN " +
                     "   dbo.RTM_Records ON dbo.RTM_User_List.UL_User_Name = dbo.RTM_Records.R_User_Name INNER JOIN " +
                     "   dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID" +
                     " WHERE  " +
                     qryWhereCond +
                     " AND (dbo.RTM_Records.R_Start_Date_Time >= @from and dbo.RTM_Records.R_Start_Date_Time <= @to) " +
                     " AND dbo.RTM_Records.R_Duration <> 'HH:MM:SS' " +
                     " GROUP BY dbo.RTM_User_List.UL_User_Name, dbo.RTM_User_List.UL_EmailId,dbo.RTM_User_List.UL_Payroll_Id , dbo.RTM_User_List.UL_Employee_Id,UL_Hourly";
           // dtRecords = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);


            SqlParameter[] parameters1 = new SqlParameter[]
            {
               //new SqlParameter("@user", user),
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };

            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Records.R_TeamId", "dbo.RTM_Log_Actions.LA_TeamId");
            sQuery = "SELECT TOP (100) PERCENT dbo.RTM_User_List.UL_User_Name as [Full Name], dbo.RTM_User_List.UL_EmailId as [Email], dbo.RTM_User_List.UL_Payroll_Id as [Payroll_Id]," +
                     "	 dbo.RTM_User_List.UL_Employee_Id as [Number], " +
                     "    case when UL_Hourly is Null then 1 end AS Salaried, " +
                     "	  SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as [hours] " +
                     " FROM            dbo.RTM_User_List INNER JOIN " +
                     "   dbo.RTM_Log_Actions ON dbo.RTM_User_List.UL_User_Name = dbo.RTM_Log_Actions.LA_User_Name INNER JOIN " +
                     "   dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID " +
                              " WHERE  " +
                          qryWhereCond +
                     " AND (dbo.RTM_Log_Actions.LA_Start_Date_Time >= @from and dbo.RTM_Log_Actions.LA_Start_Date_Time <= @to) " +
                     " GROUP BY dbo.RTM_User_List.UL_User_Name, dbo.RTM_User_List.UL_EmailId,dbo.RTM_User_List.UL_Payroll_Id , dbo.RTM_User_List.UL_Employee_Id,UL_Hourly";
            //" ORDER BY dbo.RTM_Team_List.T_TeamName";
           //dtLogActions = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLogActions = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1);


            SqlParameter[] parameters2 = new SqlParameter[]
            {
               //new SqlParameter("@user", user),
               new SqlParameter("@from", fromDate),
               new SqlParameter("@to", toDate)
            };
            qryWhereCond = qryWhereCond.Replace("dbo.RTM_Log_Actions.LA_TeamId", "dbo.RTM_Team_List.T_ID");
            sQuery = "SELECT TOP (100) PERCENT dbo.RTM_User_List.UL_User_Name as [Full Name], dbo.RTM_User_List.UL_EmailId as [Email], dbo.RTM_User_List.UL_Payroll_Id as [Payroll_Id]," +
                     "	 dbo.RTM_User_List.UL_Employee_Id as [Number], " +
                     "    case when UL_Hourly is Null then 1 end AS Salaried, " +
                     "	  sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as Decimal) end) AS Hours " +
                     " FROM            dbo.RTM_LeaveDetails INNER JOIN " +
                     "    dbo.RTM_User_List ON dbo.RTM_LeaveDetails.LD_UserName = dbo.RTM_User_List.UL_User_Name INNER JOIN " +
                      "    dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID " +
                       " WHERE  " +
                   qryWhereCond +
              " AND (dbo.RTM_LeaveDetails.LD_Date >= @from and dbo.RTM_LeaveDetails.LD_Date <= @to) " +
                   " GROUP BY dbo.RTM_User_List.UL_User_Name, dbo.RTM_User_List.UL_EmailId,dbo.RTM_User_List.UL_Payroll_Id, dbo.RTM_User_List.UL_Employee_Id,UL_Hourly";
            //" ORDER BY dbo.RTM_Team_List.T_TeamName";
            //dtLeaveDetails = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            dtLeaveDetails = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);

            dtFilled.Rows.Clear();

            foreach (DataRow dr1 in dtRecords.Rows)
            {
                dtFilled.Rows.Add(dr1["Full Name"], dr1["Email"], dr1["Payroll_Id"], dr1["Number"], dr1["Salaried"], dr1["Hours"]);
            }

            foreach (DataRow dr2 in dtLogActions.Rows)
            {
                dtFilled.Rows.Add(dr2["Full Name"], dr2["Email"], dr2["Payroll_Id"], dr2["Number"], dr2["Salaried"], dr2["Hours"]);
            }

            foreach (DataRow dr3 in dtLeaveDetails.Rows)
            {
                dtFilled.Rows.Add(dr3["Full Name"], dr3["Email"], dr3["Payroll_Id"], dr3["Number"], dr3["Salaried"], dr3["Hours"]);
            }

            DataTable dtNew = new DataTable();
            DataTable dtUnique = new DataTable();

            dtUnique.Clear();

            dtUnique = dtFilled.DefaultView.ToTable(true, "UL_User_Name", "UL_EmailId", "UL_Employee_Id", "Salaried");

            dtUnique.Columns.Add("Payroll Id");
            dtUnique.Columns["Payroll Id"].SetOrdinal(2);

            decimal sum = 0;

            dtNew.Clear();
            dtNew.Columns.Add("Full Name", typeof(string));
            dtNew.Columns.Add("Email", typeof(string));
            dtNew.Columns.Add("Payroll Id", typeof(string));
            dtNew.Columns.Add("Number", typeof(string));
            dtNew.Columns.Add("Salaried", typeof(string));
            dtNew.Columns.Add("Exempt", typeof(string));
            dtNew.Columns.Add("Hours", typeof(string));


            foreach (DataRow dr4 in dtUnique.Rows)
            {
                foreach (DataRow dr5 in dtFilled.Rows)
                {
                     if (dr4["UL_User_Name"].ToString().ToLower() == dr5["UL_User_Name"].ToString().ToLower())
                    {
                        if ((dr5["Hours"].ToString().Trim()) != "")
                        {
                        sum = sum + Convert.ToDecimal(dr5["Hours"]);
                        }
                    }
                }

                if (dr4["Salaried"].ToString() == "0")
                {
                    dtNew.Rows.Add(dr4["UL_User_Name"], dr4["UL_EmailId"], dr4["Payroll Id"], dr4["UL_Employee_Id"], dr4["Salaried"], "0", sum);
                }
                else
                {
                    dtNew.Rows.Add(dr4["UL_User_Name"], dr4["UL_EmailId"], dr4["Payroll Id"], dr4["UL_Employee_Id"], dr4["Salaried"], "1", sum);
                }

                sum = 0;
            }
            return dtNew;
        }

        public DataTable getResultTangoeApprovals(string qryWhereCond, string toDate)
        {
            DataTable dtRecords = new DataTable();
            DataTable dtLogActions = new DataTable();
            DataTable dtLeaveDetails = new DataTable();
            DataTable dtSubmitted = new DataTable();
            DataTable dtApproved = new DataTable();
            DataTable dtFilled = new DataTable();
            DataTable dtResult = new DataTable();
            dtFilled.Clear();
            dtSubmitted.Clear();
            dtFilled.Columns.Add("Full Name", typeof(string));
            dtFilled.Columns.Add("Date of Joining", typeof(string));
            dtFilled.Columns.Add("Payroll Id", typeof(string));
            dtFilled.Columns.Add("Group", typeof(string));
            dtFilled.Columns.Add("Submitted-through", typeof(string));
            dtFilled.Columns.Add("Approved-through", typeof(string));

            dtSubmitted.Columns.Add("Date", typeof(DateTime));
            dtApproved.Columns.Add("Date", typeof(DateTime));

            sQuery = "SELECT        dbo.RTM_User_List.UL_User_Name, dbo.RTM_User_List.UL_DOJ, dbo.RTM_Team_List.T_TeamName,dbo.RTM_User_List.UL_Payroll_Id as [Payroll_Id] " +
                     " FROM            dbo.RTM_User_List INNER JOIN " +
                     "    dbo.RTM_Team_List ON dbo.RTM_User_List.UL_Team_Id = dbo.RTM_Team_List.T_ID " +
         " WHERE  " +
         qryWhereCond;
            dtResult = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);

            foreach (DataRow row in dtResult.Rows)
            {
                DataTable dt = new DataTable();
                SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
               new SqlParameter("@to", toDate)
            };

                dtSubmitted.Rows.Clear();
                dtApproved.Rows.Clear();

                // For submitted date
                sQuery = "SELECT Max(dbo.RTM_Records.R_SubmittedOn) as R_Start_Date_Time from dbo.RTM_Records where R_User_Name = @user " +
                           " AND " +
                           " dbo.RTM_Records.R_SubmittedOn <= @to and R_Submit = '1'";
                    // dtRecords = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
                dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);


            //    SqlParameter[] parameters1 = new SqlParameter[]
            //{
            //   new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
            //   new SqlParameter("@to", toDate)
            //};
            //         sQuery = "SELECT Max(dbo.RTM_Log_Actions.LA_SubmittedOn) as LA_Start_Date_Time from dbo.RTM_Log_Actions where LA_User_Name = @user " +
            //                 " AND " +
            //                 " dbo.RTM_Log_Actions.LA_SubmittedOn <= @to and LA_Submit = '1'";
            //        // dtLogActions = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            //         dtLogActions = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters1);

            //         SqlParameter[] parameters2 = new SqlParameter[]
            //{
            //   new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
            //   new SqlParameter("@to", toDate)
            //};
            //         sQuery = "SELECT Max(dbo.RTM_LeaveDetails.LD_SubmittedOn) as LD_Date  from dbo.RTM_LeaveDetails where LD_UserName = @user " +
            //            " AND " +
            //            " dbo.RTM_LeaveDetails.LD_SubmittedOn <= @to and LD_Submit = '1'";
            //         //dtLeaveDetails = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
            //         dtLeaveDetails = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters2);


                     foreach (DataRow row1 in dtRecords.Rows)
                     {
                         dtSubmitted.Rows.Add(row1["R_Start_Date_Time"]);
                     }

                     //foreach (DataRow row2 in dtLogActions.Rows)
                     //{
                     //    dtSubmitted.Rows.Add(row2["LA_Start_Date_Time"]);
                     //}

                     //foreach (DataRow row3 in dtLeaveDetails.Rows)
                     //{
                     //    dtSubmitted.Rows.Add(row3["LD_Date"]);
                     //}

                     object maxDateSubmitted = dtSubmitted.Compute("MAX(Date)", null);

                // End submitted date


                // For Approval date

                     SqlParameter[] parameters3 = new SqlParameter[]
                           {
                                 new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
                                 new SqlParameter("@to", toDate)
                           };
                     sQuery = "SELECT Max(dbo.RTM_Records.R_ApprovedOn) as R_Approve_Date from dbo.RTM_Records where R_User_Name = @user " +
                             " AND " +
                             " dbo.RTM_Records.R_ApprovedOn <= @to and R_Approve = '1'";
                     dtRecords = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters3);


                   //  SqlParameter[] parameters4 = new SqlParameter[]
                   //        {
                   //              new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
                   //              new SqlParameter("@to", toDate)
                   //        };
                   //  sQuery = "SELECT Max(dbo.RTM_Log_Actions.LA_ApprovedOn) as LA_Approve_Date from dbo.RTM_Log_Actions where LA_User_Name = @user " +
                   //          " AND " +
                   //          " dbo.RTM_Log_Actions.LA_ApprovedOn <= @to and LA_Approve = '1'";
                   ////  dtLogActions = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
                   //  dtLogActions = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters4);


                   //  SqlParameter[] parameters5 = new SqlParameter[]
                   //        {
                   //              new SqlParameter("@user", row["UL_User_Name"].ToString().Trim()),
                   //              new SqlParameter("@to", toDate)
                   //        };
                   //  sQuery = "SELECT Max(dbo.RTM_LeaveDetails.LD_ApprovedOn) as LD_Approve_Date from dbo.RTM_LeaveDetails where LD_UserName = @user " +
                   //     " AND " +
                   //     " dbo.RTM_LeaveDetails.LD_ApprovedOn <= @to and LD_Approve = '1'";
                   ////  dtLeaveDetails = objDB.ExecuteSelectCommand(sQuery, CommandType.Text);
                   //  dtLeaveDetails = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters5);

                     foreach (DataRow row1 in dtRecords.Rows)
                     {
                         dtApproved.Rows.Add(row1["R_Approve_Date"]);
                     }

                     //foreach (DataRow row2 in dtLogActions.Rows)
                     //{
                     //    dtApproved.Rows.Add(row2["LA_Approve_Date"]);
                     //}

                     //foreach (DataRow row3 in dtLeaveDetails.Rows)
                     //{
                     //    dtApproved.Rows.Add(row3["LD_Approve_Date"]);
                     //}

                     object maxDateApproved = dtApproved.Compute("MAX(Date)", null);

                if  (maxDateSubmitted.ToString() != "" &  maxDateApproved.ToString() != "")
                {
                    dtFilled.Rows.Add(row["UL_User_Name"], row["UL_DOJ"], row["Payroll_Id"], row["T_TeamName"], maxDateSubmitted.ToString(), maxDateApproved.ToString());
                }
            }

            // End Approval date

            return dtFilled;

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
            if (_access == "1")
            {
                sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_Active = 1 order by T_TeamName";
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
                sQuery = "select * from RTM_Team_List, RTM_AccessPermissions where T_ID = AP_TID and AP_UID =@uid and AP_Status =1 and T_Active =1 order by T_TeamName ";
                //sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_Manager =@manager and T_Active = 1 order by T_TeamName";
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
                         "select  T_ID, T_TeamName from CTE, RTM_Team_List Where CTE.UL_Team_Id = T_ID GROUP BY T_ID, T_TeamName order by T_TeamName";
                //sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_ID = @teamId and T_Active = 1 order by T_TeamName";
            }
            else if (_access == "4")
            {
                sQuery = "SELECT * From RTM_Team_List where T_Location = @location and T_ID = @teamId and T_Active = 1 order by T_TeamName";
            }
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public int GetWeekNumber(DateTime date)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            return weekNum;
        }

        public DataTable GetHourlyTimeCardRecords(string type, string user, string from, string to, string location, string _access, string _uid)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location),
               new SqlParameter("@uid", _uid)
            };
            if (type == "adm")
            {
                if (user == "All")
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
                else
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and T_Location=@user and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
            }
            else if (type == "user")
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            else if (type == "team")
            {
                if (user == "All")
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID left join RTM_AccessPermissions on R_TeamId = AP_TID where AP_UID=@uid and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
                else
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where R_TeamId=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
            }
            else
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, STL_SubTask  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and T_Location=@location and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, STL_SubTask Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetHourlyTimeCardRecordsGreaterThan24(string type, string user, string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            
            if (type == "user")
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode  , SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and CAST(SUBSTRING(R_Duration,0,charindex(':',ASCII(LEFT(R_Duration,1)))) as Int) >= 24  GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            else if (type == "team")
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where R_TeamId=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' and CAST(SUBSTRING(R_Duration,0,charindex(':',ASCII(LEFT(R_Duration,1)))) as Int) >= 24  GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            else
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], R_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode  ,SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records left join RTM_Client_List on R_Client = CL_ID left join RTM_SubTask_List on R_SubTask = STL_ID left join RTM_Task_List on R_Task = TL_ID left join RTM_Team_List on R_TeamId = T_ID where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and T_Location=@location and R_Duration != 'HH:MM:SS' and CAST(SUBSTRING(R_Duration,0,charindex(':',ASCII(LEFT(R_Duration,1)))) as Int) >= 24  GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,R_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetHourlyTimeCardLogs(string type, string user, string from, string to, string location, string _access, string _uid)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location),
               new SqlParameter("@uid", _uid)
            };
            if (type == "adm")
            {
                if (user == "All")
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK) left join dbo.RTM_Team_List WITH (NOLOCK) on LA_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on LA_User_Name = UL_User_Name where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
                else
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK), dbo.RTM_Team_List WITH (NOLOCK), RTM_User_List WITH (NOLOCK) where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and T_Location=@user and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
            }
           else if (type == "user")
            {
                sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK), dbo.RTM_Team_List WITH (NOLOCK), RTM_User_List WITH (NOLOCK) where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_User_Name =@user group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
            }
            else if (type == "team")
            {
                if (user == "All")
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK), dbo.RTM_Team_List WITH (NOLOCK), RTM_User_List WITH (NOLOCK), RTM_AccessPermissions WITH (NOLOCK) where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and LA_TeamId = AP_TID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and AP_UID  =@uid group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
                else
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK), dbo.RTM_Team_List WITH (NOLOCK), RTM_User_List WITH (NOLOCK) where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_TeamId  =@user group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
            }
            else
            {
                sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions WITH (NOLOCK), dbo.RTM_Team_List WITH (NOLOCK), RTM_User_List WITH (NOLOCK) where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and T_Location=@location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101) order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetHourlyTimeCardLeaves(string type, string user, string from, string to, string location)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location)
            };
            if (type == "user")
            {
                sQuery = "select DISTINCT CONVERT(VARCHAR(12), LD_Date, 101) as [Date], LD_UserName, UL_Employee_Id, T_TeamName, L_JobCode,L_ServiceCode, LD_Duration  from RTM_LeaveDetails, RTM_User_List,RTM_Team_List, RTM_Leave where LD_UserName= UL_User_Name and UL_Team_Id= T_ID and LD_LID = L_ID and LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to";
            }
            else if (type == "team")
            {
                sQuery = "select DISTINCT CONVERT(VARCHAR(12), LD_Date, 101) as [Date], LD_UserName, UL_Employee_Id, T_TeamName, L_JobCode,L_ServiceCode, LD_Duration  from RTM_LeaveDetails, RTM_User_List,RTM_Team_List, RTM_Leave where LD_UserName= UL_User_Name and UL_Team_Id= T_ID and LD_LID = L_ID and UL_Team_Id =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to";
            }
            else
            {
                sQuery = "select DISTINCT CONVERT(VARCHAR(12), LD_Date, 101) as [Date], LD_UserName, UL_Employee_Id, T_TeamName, L_JobCode,L_ServiceCode, LD_Duration  from RTM_LeaveDetails, RTM_User_List,RTM_Team_List, RTM_Leave where LD_UserName= UL_User_Name and UL_Team_Id= T_ID and LD_LID = L_ID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to and T_Location=@location";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetUsers(string type, string location, string team)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@team", team),
               new SqlParameter("@location", location)
            };

            if (type == "team")
            {
                sQuery = "select UL_Employee_Id, UL_User_Name, T_TeamName from RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and UL_Team_Id = @team and UL_User_Status =1";
            }
            else if (type == "All")
            {
                sQuery = "select UL_Employee_Id, UL_User_Name, T_TeamName from RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and UL_User_Status =1 and T_Location = @location";
            }
            else
            {
                sQuery = "select UL_Employee_Id, UL_User_Name, T_TeamName from RTM_User_List, RTM_Team_List where UL_Team_Id = T_ID and UL_User_Name = @team and UL_User_Status =1";
            }
            
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalWorkHours(string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            //sQuery = "SELECT Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600) as [hours], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60) as [minutes], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(R_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Records where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'";

            sQuery = "SELECT SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS'";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalLogHours(string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            sQuery = "select Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_User_Name =@user";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetTotalLeaveHours(string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            sQuery = "select L_JobCode, sum(case when LD_Duration IS null then 8 else CAST(LD_Duration as decimal) end) as LD_Duration  from RTM_LeaveDetails, RTM_User_List,RTM_Team_List, RTM_Leave where LD_UserName= UL_User_Name and UL_Team_Id= T_ID and LD_LID = L_ID and LD_UserName =@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to group by L_JobCode";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetApprovalStatus(string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            sQuery = "select R_Approve from RTM_Records where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to group by R_Approve";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable GetLeaveApprovalStatus(string user, string from, string to)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to)
            };

            sQuery = "select LD_Approve from RTM_LeaveDetails where LD_UserName=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LD_Date))) BETWEEN @from and @to group by LD_Approve";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            return dt;
        }

        public DataTable getResultTsheetMSPReport(string Team, string fromDate, string toDate)
        {
            if (Team == "All")
            {
                sQuery = "Select ProjectPLDepartment,ClientName,cf_CustomerCode,ProjectCodeName,CompleteProjectServiceCodeDescription,EmployeePLDepartment,EmployeeName,EmployeeNumber,CONVERT(decimal(16,2), CAST([Hours] AS FLOAT)) as Hours,CONVERT(VARCHAR(12),TimeDate, 101) as TimeDate,Period,PeriodDateRange from RTM_TSheet_Data where TimeDate between '" + fromDate + "' and '" + toDate + "' order by EmployeeName";
            }
            else
            {
                sQuery = "Select ProjectPLDepartment,ClientName,cf_CustomerCode,ProjectCodeName,CompleteProjectServiceCodeDescription,EmployeePLDepartment,EmployeeName,EmployeeNumber,CONVERT(decimal(16,2), CAST([Hours] AS FLOAT)) as Hours,CONVERT(VARCHAR(12),TimeDate, 101) as TimeDate,Period,PeriodDateRange from RTM_TSheet_Data where EmployeePLDepartment = '" + Team + "' and TimeDate between '" + fromDate + "' and '" + toDate + "' order by EmployeeName";
            }
           
            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }


        public DataTable getResultTeamTsheetMSPReport()
        {
            sQuery = "Select distinct EmployeePLDepartment from RTM_TSheet_Data order by EmployeePLDepartment";

            dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }


        public DataTable GetExpandedHourRecords(string type, string user, string from, string to, string location, string _access, string _uid)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location),
               new SqlParameter("@uid", _uid)
            };
            if (type == "adm")
            {
                if (user == "All")
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
                else
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and T_Location=@user and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
            }
            else if (type == "user")
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where R_User_Name=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            else if (type == "team")
            {
                if (user == "All")
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_AccessPermissions WITH (NOLOCK) on R_TeamId = AP_TID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where AP_UID=@uid and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task,  STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
                else
                {
                    sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where R_TeamId=@user and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
                }
            }
            else
            {
                sQuery = "SELECT CONVERT(VARCHAR(12), R_TimeDate, 101) as [Date], R_User_Name as [User], UL_Employee_Id ,T_TeamName, CL_ClientName as [Client], CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as INT)) as [hours], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as INT)) as [minutes], SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as INT)) as seconds from RTM_Records WITH (NOLOCK) left join RTM_Client_List WITH (NOLOCK) on R_Client = CL_ID left join RTM_SubTask_List WITH (NOLOCK) on R_SubTask = STL_ID left join RTM_Task_List WITH (NOLOCK) on R_Task = TL_ID left join RTM_Team_List WITH (NOLOCK) on R_TeamId = T_ID left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) BETWEEN @from and @to and T_Location=@location and R_Duration != 'HH:MM:SS' GROUP BY CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name,UL_Employee_Id,T_TeamName, CL_ClientName,CL_Product, CL_Code, STL_ServiceCode, TL_Task, TL_Task_New, STL_SubTask, UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail Order By CONVERT(VARCHAR(12), R_TimeDate, 101), R_User_Name";
            }
            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable GetExpandedHourLogs(string type, string user, string from, string to, string location, string _access, string _uid)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@user", user),
               new SqlParameter("@from", from),
               new SqlParameter("@to", to),
               new SqlParameter("@location", location),
               new SqlParameter("@uid", _uid)
            };
            if (type == "adm")
            {
                if (user == "All")
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason , UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
                else
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason,  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and T_Location=@user and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
            }
            else if (type == "user")
            {
                sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason,  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_User_Name =@user group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
            }
            else if (type == "team")
            {
                if (user == "All")
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason,  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List, RTM_AccessPermissions where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and LA_TeamId = AP_TID and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and AP_UID  =@uid group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
                else
                {
                    sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason,  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) and LA_TeamId  =@user group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
                }
            }
            else
            {
                sQuery = "select CONVERT(VARCHAR(12), LA_TimeDate, 101) as [date],  LA_User_Name, UL_Employee_Id, T_TeamName,  LA_Reason,  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail, Convert(Varchar, sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600) as [hour], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60) as [minute], Convert(Varchar,(sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))-(((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/3600)*3600)-60*((sum(datediff(second,'00:00:00',REPLACE(LA_Duration,'-', '')))/60)%60)))%60) as seconds from RTM_Log_Actions, dbo.RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) between @from and @to and T_Location=@location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) group by T_TeamName, LA_User_Name,UL_Employee_Id, LA_Reason, CONVERT(VARCHAR(12), LA_TimeDate, 101),  UL_Hourly, UL_DOJ, UL_Job_Title, UL_RepMgrId, UL_RepMgrEmail order BY CONVERT(VARCHAR(12), LA_TimeDate, 101), LA_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, parameters);
            //dt = objDB.DBExecDataTable(sQuery);
            return dt;
        }

        public DataTable NonComplianceReport(string team, string location)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@team", team),
                new SqlParameter("@location", location)
            };

            if (team == "All")
            {
                if (location == "ADM")
                {
                    sQuery = "select UL_Employee_ID as [Employee Id], R_User_Name as [Employee Name], T_TeamName as [Team Name], UL_RepMgrEmail as [Reporting Manager email Id], UL_RepMgrId,  max(R_TimeDate) as [Last Submitted Date], UL_Director from RTM_Records WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name left join RTM_Team_List WITH (NOLOCK) on UL_Team_Id = T_ID where R_Submit =1 and R_SubmittedOn is not null and (UL_Hourly = 0 or UL_Hourly is null) and (UL_Exclude is null or UL_Exclude =0) and UL_Job_Title Not in ('Dir, Imp Shared Services', 'Dir, Program Management', 'Dir, Service Delivery', 'Dir, Strategic Consulting', 'Director, Audit', 'Director, CA & CB', 'Director, Content Management', 'Director, Engagement', 'Director, Financial Ops', 'Director, Implementations', 'Director, Operations', 'EVP, Operations', 'Sr Dir, Imp Onboarding', 'Vice President, Engagement', 'VP, Service Delivery', 'VP, Shared Services') and UL_User_Status=1 group by R_User_Name, UL_Employee_ID,T_TeamName, UL_RepMgrEmail, UL_RepMgrId, UL_Director order by T_TeamName,R_User_Name ";
                }
                else
                {
                    sQuery = "select UL_Employee_ID as [Employee Id], R_User_Name as [Employee Name], T_TeamName as [Team Name], UL_RepMgrEmail as [Reporting Manager email Id], UL_RepMgrId,  max(R_TimeDate) as [Last Submitted Date], UL_Director from RTM_Records WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name left join RTM_Team_List WITH (NOLOCK) on UL_Team_Id = T_ID where R_Submit =1 and R_SubmittedOn is not null and (UL_Hourly = 0 or UL_Hourly is null) and (UL_Exclude is null or UL_Exclude =0) and UL_Job_Title Not in ('Dir, Imp Shared Services', 'Dir, Program Management', 'Dir, Service Delivery', 'Dir, Strategic Consulting', 'Director, Audit', 'Director, CA & CB', 'Director, Content Management', 'Director, Engagement', 'Director, Financial Ops', 'Director, Implementations', 'Director, Operations', 'EVP, Operations', 'Sr Dir, Imp Onboarding', 'Vice President, Engagement', 'VP, Service Delivery', 'VP, Shared Services') and T_Location =@location and UL_User_Status=1 group by R_User_Name, UL_Employee_ID,T_TeamName, UL_RepMgrEmail, UL_RepMgrId, UL_Director order by T_TeamName,R_User_Name ";
                }
            }
            else
            {
                sQuery = "select UL_Employee_ID as [Employee Id], R_User_Name as [Employee Name], T_TeamName as [Team Name], UL_RepMgrEmail as [Reporting Manager email Id], UL_RepMgrId,  max(R_TimeDate) as [Last Submitted Date], UL_Director from RTM_Records WITH (NOLOCK) left join RTM_User_List WITH (NOLOCK) on R_User_Name = UL_User_Name left join RTM_Team_List WITH (NOLOCK) on UL_Team_Id = T_ID where R_Submit =1 and R_SubmittedOn is not null and (UL_Hourly = 0 or UL_Hourly is null) and (UL_Exclude is null or UL_Exclude =0) and UL_Job_Title Not in ('Dir, Imp Shared Services', 'Dir, Program Management', 'Dir, Service Delivery', 'Dir, Strategic Consulting', 'Director, Audit', 'Director, CA & CB', 'Director, Content Management', 'Director, Engagement', 'Director, Financial Ops', 'Director, Implementations', 'Director, Operations', 'EVP, Operations', 'Sr Dir, Imp Onboarding', 'Vice President, Engagement', 'VP, Service Delivery', 'VP, Shared Services') and UL_Team_Id=@team and UL_User_Status=1 group by R_User_Name, UL_Employee_ID,T_TeamName, UL_RepMgrEmail, UL_RepMgrId, UL_Director order by T_TeamName,R_User_Name";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable getDirector(string user)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@user", user)
            };

            sQuery = ";WITH CTE_Traverse_hierarchy "+
                        "AS "+
                        "( "+
                          "SELECT UL_Employee_Id,UL_User_Name,UL_EmailId,UL_RepMgrId,UL_Job_Title,UL_RepMgrEmail,0 Lvl FROM RTM_User_List "+
                           "Where UL_User_Name=@user " +
                           "UNION ALL  "+
                           "SELECT E.UL_Employee_Id,E.UL_User_Name,E.UL_EmailId,E.UL_RepMgrId,E.UL_Job_Title,E.UL_RepMgrEmail,Lvl+1 Lvl FROM RTM_User_List E "+
                           "JOIN CTE_Traverse_hierarchy Parent on Parent.UL_RepMgrId=E.UL_Employee_Id "+
                        ") "+
                        "Select UL_EmailId from CTE_Traverse_hierarchy where (UL_Job_Title Like '%Dir%' or UL_Job_Title Like '%Director%')";

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable getDirectorFromMasterTable(string empId)
        {
            dt = new DataTable();
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@empId", empId)
            };

            sQuery = ";WITH CTE_Traverse_hierarchy "+
                        "AS "+
                        "( "+
                          " SELECT MUL_EmployeeId,MUL_EmailId,MUL_ManagerId,MUL_Job_Title,MUL_ManagerEmail_Id,0 Lvl FROM RTM_Master_UserList Where MUL_EmailId != MUL_ManagerEmail_Id and MUL_EmployeeId=@empId " +
                           "UNION ALL "+
                           "SELECT E.MUL_EmployeeId,E.MUL_EmailId,E.MUL_ManagerId,E.MUL_Job_Title,E.MUL_ManagerEmail_Id,Lvl+1 Lvl FROM RTM_Master_UserList E "+
                           "JOIN CTE_Traverse_hierarchy Parent on Parent.MUL_ManagerId=E.MUL_EmployeeId WHERE E.MUL_EmailId != E.MUL_ManagerEmail_Id "+
                        ") "+
                        "Select * from CTE_Traverse_hierarchy"; 

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }

        public DataTable GetWeeklyHoursGreaterThan40(string from, string to, string team, string location)
        {
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@from", from),
                new SqlParameter("@to", to),
                new SqlParameter("@team", team),
                new SqlParameter("@location", location)
            };

            if (team == "All")
            {
                if (location == "ADM")
                {
                    sQuery = " SELECT A.team as [Team1], A.username as [user1], A.UL_Employee_ID, A.UL_Job_Title, A.UL_RepMgrEmail, A.UL_Director, ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total] " +
                             "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_Job_Title, UL_RepMgrEmail, UL_Director, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List, RTM_User_List where R_TeamId = T_ID and R_User_Name= UL_User_Name and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to Group by T_TeamName, R_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director  ) A  " +
                             "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director) B on A.username = B.username " +
                             "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID,A.UL_Job_Title, A.UL_RepMgrEmail, A.UL_Director HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) >40 order by A.team, A.username ";
                }
                else
                {
                    sQuery = " SELECT A.team as [Team1], A.username as [user1], A.UL_Employee_ID, A.UL_Job_Title, A.UL_RepMgrEmail, A.UL_Director, ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total] " +
                                 "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID, UL_Job_Title, UL_RepMgrEmail, UL_Director, SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records, RTM_Team_List, RTM_User_List where R_TeamId = T_ID and R_User_Name= UL_User_Name and R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and T_Location=@location Group by T_TeamName, R_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director  ) A  " +
                                 "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail,UL_Director, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions , RTM_Team_List, RTM_User_List where LA_TeamId = T_ID and LA_User_Name = UL_User_Name and LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and T_Location=@location and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director) B on A.username = B.username " +
                                 "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID,A.UL_Job_Title, A.UL_RepMgrEmail, A.UL_Director HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) >40 order by A.team, A.username ";
                }
            }
            else
            {
                sQuery = " SELECT A.team as [Team1], A.username as [user1], A.UL_Employee_ID, A.UL_Job_Title, A.UL_RepMgrEmail,A.UL_Director, ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)),2) as [Total] " +
                             "FROM ( select T_TeamName as [team], R_User_Name as [username], UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail,UL_Director,  SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(R_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Records left join RTM_Team_List on R_TeamId = T_ID left join RTM_User_List on R_User_Name= UL_User_Name where R_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, R_TimeDate))) Between @from and @to and R_TeamId=@team Group by T_TeamName, R_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director  ) A  " +
                             "Left join ( select T_TeamName as [team], LA_User_Name as [username], UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director, SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),3) as float)) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),2) as float))/60) + (SUM(CAST(PARSENAME(REPLACE(LA_Duration,':','.'),1) as float))/3600) as Sum_Col1 FROM  RTM_Log_Actions left join RTM_Team_List on LA_TeamId = T_ID left join RTM_User_List on LA_User_Name = UL_User_Name where LA_Duration !='HH:MM:SS' and CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, LA_TimeDate))) Between @from and @to and LA_TeamId=@team and (LA_Reason = 'Non-Task' or LA_Reason = 'Conference-Call' or LA_Reason = 'Conf-Call' or LA_Reason='Meeting' or LA_Reason= 'Meetings' ) Group by T_TeamName, LA_User_Name, UL_Employee_ID,UL_Job_Title, UL_RepMgrEmail, UL_Director) B on A.username = B.username " +
                             "Group by A.team, B.team,A.username, B.username,A.UL_Employee_ID,A.UL_Job_Title, A.UL_RepMgrEmail, A.UL_Director HAVING ROUND(SUM(COALESCE(A.Sum_Col1,0) + COALESCE(B.Sum_Col1,0)), 2) >40 order by A.team, A.username ";
            }

            dt = objDB.ExecuteParamerizedSelectCommand(sQuery, CommandType.Text, param);
            return dt;
        }
    }
}
