﻿using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleAssortOutController : SaleController
    {
        public SaleAssortOutController()
        {
            Name = "WEBТАЛИNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_OUT_ASSORT_NDS;
            DocumentKindId = DocumentSales.KINDID_ASSORTOUT;
        }
        public override ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 2;
            ViewData["onlyUsers"] = false;
            ViewData["OnlySupplyer"] = false;
            return PartialView("ClientsFinderPartial");
        }
    }
}
