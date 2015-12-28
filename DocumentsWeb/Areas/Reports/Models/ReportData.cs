using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Reports.Models
{
    public static class ReportData
    {
        public static List<Library> LinkedReports(Library selfLibrary)
        {
            // Ќе указана или неправильно указана библиотека, см свойство Name контролерра
            if (selfLibrary == null)
                return new List<Library>();
            List<Library> coll = Chain<Library>.GetChainSourceList(selfLibrary, WADataProvider.WA.ReportFormChainId(), State.STATEACTIVE).Where(s => s.StateId == State.STATEACTIVE).ToList();

            return
                coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)
                && WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, s.Id)
                && WADataProvider.LibrariesElementRightView.IsAllow(Right.UIREPORTBUILD, s.Id)).ToList();
        }
    }
}