using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using DevExpress.Web.ASPxNavBar;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;

namespace DocumentsWeb.Code
{
    /// <summary>
    /// Класс для работы со связанными документами
    /// </summary>
    public class LinkedDocumentsHelper
    {
        /// <summary>
        /// Создание ссылок на документы, которые могут быть созданы из текущего
        /// </summary>
        /// <param name="group"></param>
        /// <param name="Model"></param>
        /// <param name="page"></param>
        public static void CreateChildDocuments(MVCxNavBarGroup group, DocumentModel Model, WebViewPage page)
        {
            if(Model.Id==0)
                return;

            List<BusinessObjects.Documents.DocChain> coll = BusinessObjects.Documents.DocChain.Get(WADataProvider.WA, Model.TemplateId);
            if(coll.Count>0)
                group.Items.Add(itemLinkHr => itemLinkHr.SetTemplateContent(s => page.ViewContext.Writer.Write("<hr/>")));
            
            foreach (var chain in coll)
            {
                bool haveCreate = WADataProvider.FolderElementRightView.IsAllow(BusinessObjects.Security.Right.DOCCREATE, WADataProvider.WA.GetFolderByCodeFind(chain.CodeFind).Id);
                if (!haveCreate)
                    continue;

                group.Items.Add(item =>
                {
                    item.Text = chain.Name;
                    item.ToolTip = chain.Memo;
                    item.Image.Url = page.Url.Content("~/Images/ACTIVITY_SEQUENCE_X16.png");
                    item.Target = "_blank";
                    item.NavigateUrl = page.Url.Action("ChildDocumentCreate", new { Controller = page.Url.RequestContext.RouteData.Values["controller"], id = Model.Id, chainId = chain.Id, codeValue = chain.Code, });
                });
            }
        }

        /// <summary>
        /// Созданее группы, отображающей документы, являющиеся подчиненными или родительскими к данному
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="Model"></param>
        /// <param name="ViewContext"></param>
        /// <param name="Request"></param>
        public static void CreateLinkedDocumentsGroup(NavBarSettings settings, DocumentModel Model, ViewContext ViewContext, HttpRequestBase Request, WebViewPage page)
        {
            if (Model.Id == 0)
                return;

            settings.Groups.Add(group =>
            {
                group.Text = "Связанные документы";
                group.Name = "grpLinkedDocuments";
                group.ItemLinkMode = GroupItemLinkMode.TextAndImage;
                group.ItemStyle.Paddings.PaddingTop = System.Web.UI.WebControls.Unit.Pixel(2);
                group.ItemStyle.Paddings.PaddingBottom = System.Web.UI.WebControls.Unit.Pixel(2);
                group.HeaderImage.Url = page.Url.Content("~/Images/link_x16.png");
                // TODO: убрать после обновления
                group.ContentStyle.Font.Underline = true;
                group.Expanded = true;
                //group.ClientVisible = (Model.Id != 0);
                
                group.Items.Add(itemLinkChaild => itemLinkChaild.SetTemplateContent(s => ViewContext.Writer.Write("<b>Подчиненные</b>")));
                if (Model.GetChainsDocuments().Count == 0)
                    group.Items.Add(item =>
                        {
                            item.SetTemplateContent(s => ViewContext.Writer.Write("Нет подчиненных"));
                        });
                else
                    foreach (var doc in Model.GetChainsDocuments())
                    {
                        group.Items.Add(docItem =>
                        {
                            docItem.Text = doc.NameFull;
                            docItem.NavigateUrl = doc.GetEditUrl(Request.RequestContext);
                            docItem.Target = "_blank";
                        });
                    }
                group.Items.Add(itemLinkParentld => itemLinkParentld.SetTemplateContent(s => ViewContext.Writer.Write("<hr/><b>Главные</b>")));
                if (Model.GetChainsParentDocuments().Count == 0)
                    group.Items.Add(item =>
                        {
                            item.SetTemplateContent(s => ViewContext.Writer.Write("Нет глваных"));
                        });
                else
                    foreach (var doc in Model.GetChainsParentDocuments())
                    {
                        group.Items.Add(docItem =>
                        {
                            docItem.Text = doc.NameFull;
                            docItem.NavigateUrl = doc.GetEditUrl(Request.RequestContext);
                            docItem.Target = "_blank";
                        });
                    }
            });
        }
    }
}