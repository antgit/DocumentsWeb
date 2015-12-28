using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class secureNotAllow : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.Url.Host.ToUpper().Contains("moedelo.in.ua"))
            HyperLink1.Visible = true;
        else
            HyperLink1.Visible = false;
    }
}