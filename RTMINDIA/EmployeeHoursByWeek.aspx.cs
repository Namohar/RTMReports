using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using BAL;
using DAL;
using iTextSharp.text;
using NReco.PivotData;
using NReco.PivotData.Output;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.html.simpleparser;


public partial class EmployeeHoursByWeek : System.Web.UI.Page
{
    DataTable dt = new DataTable();    
    DataTable dtResult = new DataTable();
    DataTable dtTeam = new DataTable();
    DataTable dtUsers = new DataTable();
    DateTime temp;
    List lstDate = new List();
    clsADM objADM = new clsADM();
    
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
            LoadLocation();
        }
    }

    private void LoadLocation()
    {
        clsUserLogin objLogin = new clsUserLogin();
        dt = objLogin.getLocationsforMSPReport();
        if (dt.Rows.Count > 0)
        {
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "T_Location";
            ddlLocation.DataValueField = "T_Location";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, "All");
            ddlLocation.SelectedIndex = 0;
        }
    }
  
    protected void btnExport_Click(object sender, EventArgs e)
    {
       // ExportReport();
        ExportReporttoPDF();
    }

    private void BuildTeamTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Team");
        dtResult.Columns.Add("Date");
        dtResult.Columns.Add("Avg");
    }

    private void BuildEmpTable()
    {
        dtResult = new DataTable();
        dtResult.Columns.Add("Team");
        dtResult.Columns.Add("User");
        dtResult.Columns.Add("Date");
        dtResult.Columns.Add("Avg");
    }

    private void ExportReporttoPDF()
    {
        if (DateTime.TryParse(txtFrom.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "from date", "alert('Please select valid from date')", true);
            return;
        }

        if (DateTime.TryParse(txtTo.Text, out temp))
        { }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('Please select valid to date')", true);
            return;
        }

        if (Convert.ToDateTime(txtFrom.Text) > Convert.ToDateTime(txtTo.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "to date", "alert('To date should be greated than from date')", true);
            return;
        }

        int weeks = Convert.ToInt32((Convert.ToDateTime(txtTo.Text) - Convert.ToDateTime(txtFrom.Text)).TotalDays / 7);
        string from = Convert.ToDateTime(txtFrom.Text).ToString("MM/dd/yyyy");
        for (int i = 0; i <= weeks - 1; i++)
        {

            string to = Convert.ToDateTime(from).AddDays(6).ToString("MM/dd/yyyy");

            if (Convert.ToDateTime(to) > Convert.ToDateTime(txtTo.Text))
            {
                break;
            }
            lstDate.Add(from + "-" + to);
            from = Convert.ToDateTime(to).AddDays(1).ToString("MM/dd/yyyy");
            
        }

        DataRow dr;
        BuildTeamTable();
        
        //double count = 0;
        double average=0;

        //dtTeam = objADM.LoadTeams(ddlLocation.SelectedItem.Text);
        //if (dtTeam.Rows.Count > 0)
        //{
        //    foreach (DataRow drTeam in dtTeam.Rows)
        //    { 
        //        dt = objADM.GetUserCount(drTeam["T_ID"].ToString());
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (!string.IsNullOrEmpty(dt.Rows[0]["Count"].ToString()))
        //            {
        //                count = Convert.ToDouble(dt.Rows[0]["Count"]);
        //            }
        //        }

        //        foreach (var item in lstDate.Chunks)
        //        {
        //            dr = dtResult.NewRow();
        //            dr["Team"] = drTeam["T_TeamName"];
        //            dr["Date"] = item.ToString().Split('-')[0];

        //            dt = objADM.GetTotalWorkHours("team", item.ToString().Split('-')[0], item.ToString().Split('-')[1], drTeam["T_ID"].ToString());

        //            if (dt.Rows.Count > 0)
        //            {
        //                if (dt.Rows[0]["Total"].ToString() == "0")
        //                {
        //                    continue;
        //                }
        //                if (count > 0)
        //                {
        //                    average = (Convert.ToDouble(dt.Rows[0]["Total"])) / count;
        //                }
        //            }
                    

        //            dr["Avg"] = Math.Round(average, 2, MidpointRounding.AwayFromZero);
        //            if (average > 0)
        //            {
        //                dtResult.Rows.Add(dr);
        //            }
                    
        //            average = 0;                    
        //        }

        //        count = 0;

        //    }
        //}

        foreach (var item in lstDate.Chunks)
        {

            dt = objADM.GetTeamTotal(item.ToString().Split('-')[0], item.ToString().Split('-')[1], ddlLocation.SelectedItem.Text);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();
                    if (string.IsNullOrEmpty(drRow["Team1"].ToString()))
                    {
                        dr["Team"] = drRow["Team2"];
                    }
                    else
                    {
                        dr["Team"] = drRow["Team1"];
                    }
                    dr["Date"] = item.ToString().Split('-')[0];

                    dtUsers = objADM.GetUserCount(dr["Team"].ToString());
                    if (dtUsers.Rows.Count > 0)
                    {
                        average = Convert.ToDouble(drRow["Total"]) / Convert.ToDouble(dtUsers.Rows[0]["Count"].ToString());
                        dr["Avg"] = Math.Round(average, 2, MidpointRounding.AwayFromZero);
                    }
                    average = 0;
                    dtResult.Rows.Add(dr);
                }
            }
        }

        //if (dtResult.Rows.Count > 0)
        //{
        //    DataView dv = dtResult.DefaultView;
        //    dv.Sort = "[Date] asc, [Team] asc";
        //    dtResult = dv.ToTable();
            
        //}

        var ordersPvtData = new PivotData(new[] { "Team", "Date" },
           new SumAggregatorFactory("Avg"));
        ordersPvtData.ProcessData(new DataTableReader(dtResult));

        var pvtTbl = new PivotTable(
                    new[] { "Team" }, //rows
                    new[] { "Date" }, // columns
                    ordersPvtData,

                    new CustomSortKeyComparer(new[] {  NaturalSortKeyComparer.Instance }),
                  new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance })  
                );

        var pvtDataTableWr = new PivotTableDataTableWriter("PivotTable");
        DataTable res = pvtDataTableWr.Write(pvtTbl);
        res.Columns[0].ColumnName = "Team";

        ExportToPDF(res);
        
    }
    
    private DataTable GetWeeklyHoursByEmployee()
    {
        DataRow dr;
        BuildEmpTable();



        //dtTeam = objADM.LoadUsers(ddlLocation.SelectedItem.Text);
        //if (dtTeam.Rows.Count > 0)
        //{
        //    foreach (DataRow drTeam in dtTeam.Rows)
        //    {
        //        foreach (var item in lstDate.Chunks)
        //        {
        //            dr = dtResult.NewRow();
        //            dr["Team"] = drTeam["T_TeamName"];
        //            dr["User"] = drTeam["UL_User_Name"].ToString().Trim();
        //            dr["Date"] = item.ToString().Split('-')[0];

        //            dt = objADM.GetTotalWorkHours("user", item.ToString().Split('-')[0], item.ToString().Split('-')[1], drTeam["UL_User_Name"].ToString());

        //            if (dt.Rows.Count > 0)
        //            {
        //                if (dt.Rows[0]["Total"].ToString() == "0")
        //                {
        //                    continue;
        //                }

        //                dr["Avg"] = Math.Round(Convert.ToDouble(dt.Rows[0]["Total"].ToString()), 2, MidpointRounding.AwayFromZero);
        //            }                    
                    
        //            dtResult.Rows.Add(dr);
                    
        //        }

                
        //    }
        //}

        foreach (var item in lstDate.Chunks)
        {
            dt = objADM.GetUserTotal(item.ToString().Split('-')[0], item.ToString().Split('-')[1], ddlLocation.SelectedItem.Text);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    dr = dtResult.NewRow();
                    if (string.IsNullOrEmpty(drRow["Team1"].ToString()))
                    {
                        dr["Team"] = drRow["Team2"];
                    }
                    else
                    {
                        dr["Team"] = drRow["Team1"];
                    }
                    if (string.IsNullOrEmpty(drRow["user1"].ToString()))
                    {
                        dr["User"] = drRow["user2"].ToString().Trim();
                    }
                    else
                    {
                        dr["User"] = drRow["user1"].ToString().Trim();
                    }
                    
                    dr["Date"] = item.ToString().Split('-')[0];
                    dr["Avg"] = Math.Round(Convert.ToDecimal(drRow["Total"]), 2, MidpointRounding.AwayFromZero);

                    dtResult.Rows.Add(dr);
                }
            }
        }

        //if (dtResult.Rows.Count > 0)
        //{
        //    DataView dv = dtResult.DefaultView;
        //    dv.Sort = "[Date] asc, [Team] asc";
        //    dtResult = dv.ToTable();

        //}

        var ordersPvtData = new PivotData(new[] { "Team", "User", "Date" },
           new SumAggregatorFactory("Avg"));
        ordersPvtData.ProcessData(new DataTableReader(dtResult));

        var pvtTbl = new PivotTable(
                    new[] { "Team", "User" }, //rows
                    new[] { "Date" }, // columns
                    ordersPvtData,
                    new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance }),
                    new CustomSortKeyComparer(new[] { NaturalSortKeyComparer.Instance }) 
                );

        var pvtDataTableWr = new PivotTableDataTableWriter("PivotTable");
        DataTable res = pvtDataTableWr.Write(pvtTbl);
        res.Columns[0].ColumnName = "Team";
        res.Columns[1].ColumnName = "User";

        return res;
    }

    protected void ExportToPDF(DataTable dt)
    {

        
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment;filename=EmpHoursByWeek.pdf");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
       
        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();

        //Report Header
        BaseFont bfntHead = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        Font fntHead = new Font(bfntHead, 16, 1, Color.BLACK);
        Font fntHead2 = new Font(bfntHead, 12, 1, Color.BLACK);
        Paragraph prgHeading = new Paragraph();
        prgHeading.Alignment = Element.ALIGN_CENTER;
        prgHeading.Add(new Chunk("Employee Hours By Week", fntHead));
        prgHeading.Add(new Chunk(Environment.NewLine+"From " + txtFrom.Text + " to " + txtTo.Text, fntHead2));
        pdfDoc.Add(prgHeading);

        //Report Paragraph
        pdfDoc.Add(new Chunk("\n", fntHead));
        Font fntparagraph = new Font(bfntHead, 10, 1, Color.BLACK);
        Paragraph prgParagraph1 = new Paragraph();
        prgParagraph1.Alignment = Element.ALIGN_LEFT;
        prgParagraph1.Add(new Chunk("Average Weekly Employee Work Hours by Team", fntparagraph));
        //pdfDoc.Add(new Chunk("\n", fntHead));
        pdfDoc.Add(prgParagraph1);
        pdfDoc.Add(new Chunk("\n"));
        //Average Work hours Table
        Font fnt = FontFactory.GetFont(FontFactory.COURIER, 10);
        
        PdfPTable table = new PdfPTable(dt.Columns.Count);
        PdfPRow row = null;
        //float[] widths = new float[] { 4f, 4f, 4f, 4f };
        //table.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });
        //table.SetWidths(widths);

        table.WidthPercentage = 100;
        int iCol = 0;
        string colname = "";

        PdfPCell cell = new PdfPCell(new Phrase("Hours"));

        cell.Colspan = dt.Columns.Count;

        foreach (DataColumn col in dt.Columns)
        {

            table.AddCell(new Phrase(col.ColumnName, fnt));
        }

        foreach (DataRow r in dt.Rows)
        {
            if (dt.Rows.Count > 0)
            {
                int cnt = r.ItemArray.Count();
                int j = 1;
                foreach (var item in r.ItemArray)
                {

                    PdfPCell cell1 = new PdfPCell(new Phrase(item.ToString(), fnt));
                    if (j < cnt)
                    {
                        decimal n;
                        bool isNumeric = decimal.TryParse(item.ToString(), out n);
                        if (isNumeric)
                        {
                            if (r.ItemArray.Contains("Totals") == false)
                            {
                                if (Convert.ToDecimal(item.ToString()) >= 45 && Convert.ToDecimal(item.ToString()) < 50)
                                {
                                    cell1.BackgroundColor = Color.GREEN;
                                }
                                else if (Convert.ToDecimal(item.ToString()) >= 50 && Convert.ToDecimal(item.ToString()) < 55)
                                {
                                    cell1.BackgroundColor = Color.YELLOW;
                                }
                                else if (Convert.ToDecimal(item.ToString()) >= 55)
                                {
                                    cell1.BackgroundColor = Color.RED;
                                }
                            }
                        }
                    }
                   // pdfTable.AddCell(cell);
                    table.AddCell(cell1);
                    j = j + 1;
                }
                //table.AddCell(new Phrase(r[0].ToString(), fnt));
                //table.AddCell(new Phrase(r[1].ToString(), fnt));
                //table.AddCell(new Phrase(r[2].ToString(), fnt));
                //table.AddCell(new Phrase(r[3].ToString(), fnt));
            }
        } 
        
        pdfDoc.Add(table);

        //WeeklyEmployeeHours
        pdfDoc.Add(new Chunk("\n", fntHead));
        Paragraph prgParagraph2 = new Paragraph();
        prgParagraph2.Alignment = Element.ALIGN_LEFT;
        prgParagraph2.Add(new Chunk("Actual Weekly Employee Work Hours by Team", fntparagraph));
        
        pdfDoc.Add(prgParagraph2);
        pdfDoc.Add(new Chunk("\n", fntHead));
        dt = new DataTable();

        dt = GetWeeklyHoursByEmployee();

        PdfPTable table1 = new PdfPTable(dt.Columns.Count);
        row = null;
       //float[] widths1 = new float[] { 4f, 4f, 4f, 4f, 4f };
      // table1.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });
       // table1.SetWidths(widths1);

        table1.WidthPercentage = 100;
        iCol = 0;
        colname = "";

        PdfPCell cell2 = new PdfPCell(new Phrase("EmpHours"));

        cell2.Colspan = dt.Columns.Count;

        foreach (DataColumn col in dt.Columns)
        {

            table1.AddCell(new Phrase(col.ColumnName, fnt));
        }

        foreach (DataRow r in dt.Rows)
        {
           
            if (dt.Rows.Count > 0)
            {
                int cnt = r.ItemArray.Count();
                int j = 1;
                foreach (var item in r.ItemArray)
                {
                    PdfPCell cell1 = new PdfPCell(new Phrase(item.ToString(), fnt));
                    if (j < cnt)
                    {
                        decimal n;
                        bool isNumeric = decimal.TryParse(item.ToString(), out n);
                        if (isNumeric)
                        {
                            if (r.ItemArray.Contains("Totals") == false)
                            {
                                if (Convert.ToDecimal(item.ToString()) >= 45 && Convert.ToDecimal(item.ToString()) < 50)
                                {
                                    cell1.BackgroundColor = Color.GREEN;
                                }
                                else if (Convert.ToDecimal(item.ToString()) >= 50 && Convert.ToDecimal(item.ToString()) < 55)
                                {
                                    cell1.BackgroundColor = Color.YELLOW;
                                }
                                else if (Convert.ToDecimal(item.ToString()) >= 55)
                                {
                                    cell1.BackgroundColor = Color.RED;
                                }
                            }
                        }
                    }
                    
                    table1.AddCell(cell1);
                    j= j+1;
                    //table1.AddCell(new Phrase(item.ToString(), fnt));
                }
                //table1.AddCell(new Phrase(r[1].ToString(), fnt));
                //table1.AddCell(new Phrase(r[2].ToString(), fnt));
                //table1.AddCell(new Phrase(r[3].ToString(), fnt));
                //table1.AddCell(new Phrase(r[4].ToString(), fnt));
            }
        }

        pdfDoc.Add(table1);
        
        //htmlparser.Parse(sr);
        pdfDoc.Close();
        Response.Write(pdfDoc);
        Response.End();
    }

    

    public void ExportDSToPdf(DataTable dt)
    {
        Document document = new Document();
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("Datatable.pdf", FileMode.Create));
        document.Open();

        //Report Header
        BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        Font fntHead = new Font(bfntHead, 16, 1, Color.GRAY);
        Font fntHead2 = new Font(bfntHead, 12, 1, Color.GRAY);
        Paragraph prgHeading = new Paragraph();
        prgHeading.Alignment = Element.ALIGN_CENTER;
        prgHeading.Add(new Chunk("Employee Hours By Week", fntHead));
        prgHeading.Add(new Chunk("From "+txtFrom.Text +" to "+txtTo.Text, fntHead2));
        document.Add(prgHeading);

        iTextSharp.text.Font fnt = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

        PdfPTable table = new PdfPTable(dt.Columns.Count);
        PdfPRow row = null;
        float[] widths = new float[] { 4f, 4f, 4f, 4f };

        table.SetWidths(widths);

        table.WidthPercentage = 100;
        int iCol = 0;
        string colname = "";
        PdfPCell cell = new PdfPCell(new Phrase("Products"));

        cell.Colspan = dt.Columns.Count;

        foreach (DataColumn col in dt.Columns)
        {

            table.AddCell(new Phrase(col.ColumnName, fnt));
        }

        foreach (DataRow r in dt.Rows)
        {
            if (dt.Rows.Count > 0)
            {
                table.AddCell(new Phrase(r[0].ToString(), fnt));
                table.AddCell(new Phrase(r[1].ToString(), fnt));
                table.AddCell(new Phrase(r[2].ToString(), fnt));
                table.AddCell(new Phrase(r[3].ToString(), fnt));
            }
        } document.Add(table);
        document.Close();
    }


}