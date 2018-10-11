using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Site : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Context.Session != null)
        {
            if (Session.IsNewSession)
            {
                HttpCookie newSessionIdCookie = Request.Cookies["ASP.NET_SessionId"];
                if (newSessionIdCookie != null)
                {
                    string newSessionIdCookieValue = newSessionIdCookie.Value;
                    if (newSessionIdCookieValue != string.Empty)
                    {
                        if (!Page.IsCallback)
                        {
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            Response.RedirectLocation = Page.ResolveUrl("~/Default.aspx");
                        }
                    }
                }
            }
        }        
        if (!IsPostBack)
        {
            string usr = Session["username"].ToString();
            lblUser.Text = Session["username"].ToString().Replace('.', ' ');
            if (Session["access"].ToString() == "1")
            {
                lblOnline.Text = "Total Active Users:" + Application["TotalOnlineUsers"].ToString();
            }
           // lblTime.Text = DateTime.Now.AddHours(Convert.ToDouble(timezone.Text)).ToShortTimeString();
            if (Session["Location"].ToString() == "IND")
            {
                IN_MenuAcess();
            }
            else if (Session["Location"].ToString() == "CHN")
            {
                CN_MenuAccess();
            }
            else if (Session["Location"].ToString() == "ADM")
            {
                ADM_MenuAccess();
            }
            else if (Session["Location"].ToString() != "IND" || Session["Location"].ToString() != "CHN" || Session["Location"].ToString() == "ADM")
            {
                FRO_MenuAccess();
            }
        }
    }

    private void IN_MenuAcess()
    {
        if (Session["access"].ToString() == "1")
        {
            log.Visible = true;
            //Strike rate
            strike.Visible = true;
            cmp.Visible = true;
            ems.Visible = true;
            ipv.Visible = true;
            psl.Visible = true;
            orders.Visible = true;
            //Reports
            DelayEarly.Visible = true;
            internalmetrics.Visible = true;
            expandedHours.Visible = true;
            //Team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;
            est.Visible = true;
            estRep.Visible = true;
            estComp.Visible = true;
            ipvDet.Visible = true;
            qcDet.Visible = true;
            //cmpDet.Visible = true;
            orderDet.Visible = true;
            odHourlyTask.Visible = true;
            newClient.Visible = true;
            implRealTimeStatus.Visible = true;
            taskDet.Visible = true;
            //Settings
            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;
            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            details.Visible = true;
            TSheetMSPHoursReport.Visible = true;
            approve.Visible = true;

            nonCompliance.Visible = true;

            //EMSDB
            //EMSDB.Visible = true;
            //EMSReports.Visible = true;
            //EMSIP.Visible = true;
            //EMSQC.Visible = true;
            employeeList.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
            strikeratedetails.Visible = true;

            weeklyHoursTracking.Visible = true;
        }
        else if (Session["access"].ToString() == "2")
        {
            log.Visible = true;
            //Strike rate
            strike.Visible = true;
            cmp.Visible = true;
            ems.Visible = true;
            ipv.Visible = true;
            psl.Visible = true;
            ipvDet.Visible = true;
            qcDet.Visible = true;
            //cmpDet.Visible = true;
            orders.Visible = true;
            orderDet.Visible = true;
            //Reports
            DelayEarly.Visible = true;
            expandedHours.Visible = true;
            //internalmetrics.Visible = true;
            //Team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;
            taskDet.Visible = true;
            //Settings
            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;

            est.Visible = true;
            estRep.Visible = true;
            estComp.Visible = true;

            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;

            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            clockIn.Visible = false;
            details.Visible = true;

            TSheetMSPHoursReport.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;

            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
            strikeratedetails.Visible = true;

            weeklyHoursTracking.Visible = true;
        }
        else if (Session["access"].ToString() == "3")
        {
            log.Visible = true;           
            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;

            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;

            expandedHours.Visible = true;
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;
            taskDet.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            clockIn.Visible = false;

            est.Visible = true;
            estRep.Visible = true;
            estComp.Visible = true;
            details.Visible = true;

            TSheetMSPHoursReport.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;

            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
            strikeratedetails.Visible = true;
            if (Session["team"].ToString() == "29")
            {
                strike.Visible = true;
                cmp.Visible = true;
                ems.Visible = true;
                ipv.Visible = true;
                psl.Visible = true;
                ipvDet.Visible = true;
                cmpDet.Visible = true;
                strikeratedetails.Visible = false;
            }

            if (Session["team"].ToString() == "9")
            {
                qcDet.Visible = true;
                strikeratedetails.Visible = false;
            }

            if (Session["team"].ToString() == "1")
            {
                strike.Visible = true;
                orders.Visible = true;
                orderDet.Visible = true;
                strikeratedetails.Visible = false;
            }

            weeklyHoursTracking.Visible = true;
        }
        else if (Session["access"].ToString() == "4")
        {           
            update.Visible = true;
            comments.Visible = true;  
            //team
            addRec.Visible = true;
            editRec.Visible = true;
           // addLog.Visible = true;
            editLog.Visible = true;
            dailyAdd.Visible = false;

            Account.Visible = true;

            clockIn.Visible = false;

            est.Visible = true;
            estRep.Visible = true;
            estComp.Visible = true;
            details.Visible = true;

            tsheetReports.Visible = false;

            if (Session["team"].ToString() == "29")
            {
                strike.Visible = true;
                cmp.Visible = true;
                ems.Visible = true;
                ipv.Visible = true;
                psl.Visible = true;
                //ipvDet.Visible = true;
                //cmpDet.Visible = true;
            }

            if (Session["team"].ToString() == "1")
            {
                strike.Visible = true;
                orders.Visible = true;
               // orderDet.Visible = true;
            }
        }

        //if (Session["EMSTeam"].ToString() == "IP")
        //{
        //    EMSDB.Visible = true;
        //    if (Session["access"].ToString() == "4")
        //    {
        //        EMSIP.Visible = true;
        //        EMSAdmin.Visible = false;
        //        EMSReports.Visible = false;
        //    }
        //    else
        //    {
        //        EMSIP.Visible = true;
        //        EMSAdmin.Visible = true;
        //        EMSReports.Visible = true;
        //    }

        //}
        //if (Session["EMSTeam"].ToString() == "QC")
        //{
        //    EMSDB.Visible = true;
        //    if (Session["access"].ToString() == "4")
        //    {
        //        EMSQC.Visible = true;
        //        EMSAdmin.Visible = false;
        //        EMSReports.Visible = false;
        //    }
        //    else
        //    {
        //        EMSQC.Visible = true;
        //        EMSAdmin.Visible = true;
        //        EMSReports.Visible = true;
        //    }

        //}
    }

    private void CN_MenuAccess()
    {
        if (Session["access"].ToString() == "1")
        {
            internalmetrics.Visible = true;   
           //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;


            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

              HourlyTimeCardReport.Visible = true;
              MSPHourly.Visible = true;
              payrollSummary.Visible = true;
              TangoeApprovals.Visible = true;
             
              approve.Visible = true;
              details.Visible = true;

              TSheetMSPHoursReport.Visible = true;

              employeeList.Visible = true;

              expandedHours.Visible = true;
              nonCompliance.Visible = true;
              weekhoursgreaterthan40.Visible = true;
              summaryClientHoursByEmployee.Visible = true;
              SummaryClientHoursbyRoles.Visible = true;

              strike.Visible = true;
              orders.Visible = true;
              strikeratedetails.Visible = true;
        }
        else if (Session["access"].ToString() == "2")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;

            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            clockIn.Visible = false;
            details.Visible = true;

            TSheetMSPHoursReport.Visible = true;

            expandedHours.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
            strike.Visible = true;
            orders.Visible = true;
            strikeratedetails.Visible = true;
        }
        else if (Session["access"].ToString() == "3")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;

            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;

            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            clockIn.Visible = false;
            details.Visible = true;

            TSheetMSPHoursReport.Visible = true;

            expandedHours.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;

            if (Session["team"].ToString() == "37")
            {
                strike.Visible = true;
                orders.Visible = true;
                strikeratedetails.Visible = true;
            }
           
        }
        else if (Session["access"].ToString() == "4")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;

            Account.Visible = true;

            clockIn.Visible = false;
            details.Visible = true;
            tsheetReports.Visible = false;
        }
    }

    private void FRO_MenuAccess()
    {
        if (Session["access"].ToString() == "1")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;
            //Settings
            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            //ManageUsers.Visible = true;
            approve.Visible = true;

            TSheetMSPHoursReport.Visible = true;

            employeeList.Visible = true;

            expandedHours.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
        }
        else if (Session["access"].ToString() == "2")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;

            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;
            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            TSheetMSPHoursReport.Visible = true;

            expandedHours.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
        }
        else if (Session["access"].ToString() == "3")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;

            update.Visible = true;
            schedule.Visible = true;
            comments.Visible = true;
            Account.Visible = true;
            ManageUsers.Visible = true;
            manageUsersHRIS.Visible = true;

            HoursbyFullJobCode.Visible = true;
            HoursbyTeam.Visible = true;

            HoursbyDate.Visible = true;
            HoursByEmployee.Visible = true;

            HourlyTimeCardReport.Visible = true;
            MSPHourly.Visible = true;
            payrollSummary.Visible = true;
            TangoeApprovals.Visible = true;
            approve.Visible = true;

            TSheetMSPHoursReport.Visible = true;

            expandedHours.Visible = true;
            nonCompliance.Visible = true;
            weekhoursgreaterthan40.Visible = true;
            summaryClientHoursByEmployee.Visible = true;
            SummaryClientHoursbyRoles.Visible = true;
        }
        else if (Session["access"].ToString() == "4")
        {
            //team
            addRec.Visible = true;
            editRec.Visible = true;
            //addLog.Visible = true;
            editLog.Visible = true;
            Account.Visible = true;
            tsheetReports.Visible = false;
        }
    }

    private void ADM_MenuAccess()
    {        
        myReports.Visible = false;
        tsheetReports.Visible = false;
        userdata.Visible = false;
        ADMReports.Visible = true;

        Account.Visible = true;
        manageTasks.Visible = true;
        manageTeam.Visible = true;

        employeeList.Visible = true;
        manageUsersHRIS.Visible = true;
        ManageUsers.Visible = true;
    }
}
