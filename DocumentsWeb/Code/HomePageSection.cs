using System.Linq;
using System.Collections.Generic;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Code
{
    /// <summary>
    /// Раздел Домашней страницы
    /// </summary>
    public class HomePageSection
    {
        /// <summary>Имя контроллера документа</summary>
        public string ControllerName;
        /// <summary>Имя контроллера списка документов</summary>
        public string ViewListControllerName;
        /// <summary>Имя документа</summary>
        public string DocumentName;
        /// <summary>Подсказка для перехода в список документов</summary>
        public string ViewListToolTip;
        /// <summary>Название списка документов</summary>
        public string ViewListName;
        /// <summary>Код папки для проверки пап доступа. Оставьте пустым если проверка не требуется</summary>
        public string FolderCode=string.Empty;
        /// <summary>Отображать в панеле видимость</summary>
        public bool ShowInView = true;

        /// <summary>Список разделов для домашней страницы продаж</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionSales(bool nds)
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleOut",
                                                         ViewListControllerName =  nds ? "ViewListOutNds": "ViewListOut",
                                                         DocumentName = "Расход товара",
                                                         ViewListToolTip = "Перейти в список всех расходных накладных",
                                                         ViewListName = "Расходы",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_NDS: Folder.CODE_FIND_SALES_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleOrderOut",
                                                         ViewListControllerName =  nds ? "ViewListOrderOutNds": "ViewListOrderOut",
                                                         DocumentName = "Заказ поставщику",
                                                         ViewListToolTip = "Перейти в список всех заказов поставщику",
                                                         ViewListName = "Заказы поставщику",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_ORDER_NDS: Folder.CODE_FIND_SALES_ORDER_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleAccountOut",
                                                         ViewListControllerName =  nds ? "ViewListAccountOutNds": "ViewListAccountOut",
                                                         DocumentName = "Исходящий счет",
                                                         ViewListToolTip = "Перейти в список всех исходящих счетов",
                                                         ViewListName = "Исходящие счета",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_ACCOUNT_NDS: Folder.CODE_FIND_SALES_ACCOUNT_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleAssortOut",
                                                         ViewListControllerName =  nds ? "ViewListAssortOutNds": "ViewListAssortOut",
                                                         DocumentName = "Ассорт. лист исходящий",
                                                         ViewListToolTip =
                                                             "Перейти в список всех входящих ассортиментных листов",
                                                         ViewListName = "Ассортиментные листы",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_ASSORT_NDS: Folder.CODE_FIND_SALES_ASSORT_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleReturnOut",
                                                         ViewListControllerName =  nds ? "ViewListReturnOutNds": "ViewListReturnOut",
                                                         DocumentName = "Возврат товара поставщику",
                                                         ViewListToolTip =
                                                             "Перейти в список всех ввозвратов товара поставщику",
                                                         ViewListName = "Возвраты поставщику",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_RETURN_NDS: Folder.CODE_FIND_SALES_RETURN_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleActOut",
                                                         ViewListControllerName =  nds ? "ViewListActOutNds": "ViewListActOut",
                                                         DocumentName = "Акт списания",
                                                         ViewListToolTip = "Перейти в список всех актов списания",
                                                         ViewListName = "Акты списания",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_OUT_ACT_NDS: Folder.CODE_FIND_SALES_ACT_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleIn",
                                                         ViewListControllerName =  nds ? "ViewListInNds": "ViewListIn",
                                                         DocumentName = "Приход товара",
                                                         ViewListToolTip = "Перейти в список всех приходных накладных",
                                                         ViewListName = "Приходные накладные",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_NDS: Folder.CODE_FIND_SALES_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleOrderIn",
                                                         ViewListControllerName =  nds ? "ViewListOrderInNds": "ViewListOrderIn",
                                                         DocumentName = "Заказ покупателя",
                                                         ViewListToolTip = "Перейти в список всех заказов покупатей",
                                                         ViewListName = "Заказы покупателя",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_ORDER_NDS: Folder.CODE_FIND_SALES_ORDER_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleAccountIn",
                                                         ViewListControllerName =  nds ? "ViewListAccountInNds": "ViewListAccountIn",
                                                         DocumentName = "Входящий счет",
                                                         ViewListToolTip = "Перейти в список всех входящих счетов",
                                                         ViewListName = "Входящие счета",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_ACCOUNT_NDS: Folder.CODE_FIND_SALES_ACCOUNT_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleAssortIn",
                                                         ViewListControllerName =  nds ? "ViewListAssortInNds": "ViewListAssortIn",
                                                         DocumentName = "Ассорт. лист входящий",
                                                         ViewListToolTip =
                                                             "Перейти в список всех входящих ассортиментных листов",
                                                         ViewListName = "Входящие ассорт. листы",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_ASSORT_NDS: Folder.CODE_FIND_SALES_ASSORT_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleReturnIn",
                                                         ViewListControllerName =  nds ? "ViewListReturnInNds": "ViewListReturnIn",
                                                         DocumentName = "Возврат от покупателя",
                                                         ViewListToolTip =
                                                             "Перейти в список всех ввозвратов от покупателя",
                                                         ViewListName = "Возвраты покупателю",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_RETURN_NDS: Folder.CODE_FIND_SALES_RETURN_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleActIn",
                                                         ViewListControllerName =  nds ? "ViewListActInNds": "ViewListActIn",
                                                         DocumentName = "Акт прихода",
                                                         ViewListToolTip = "Перейти в список всех актов прихода",
                                                         ViewListName = "Акты прихода",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_IN_ACT_NDS : Folder.CODE_FIND_SALES_ACT_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleMove",
                                                         ViewListControllerName =  nds ? "ViewListMoveNds": "ViewListMove",
                                                         DocumentName = "Перемещение",
                                                         ViewListToolTip = "Перейти в список всех перемещений",
                                                         ViewListName = "Перемещения",
                                                         FolderCode = nds ? Folder.CODE_FIND_SALES_MOVE_NDS: Folder.CODE_FIND_SALES_MOVE
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "SaleInventory",
                                                         ViewListControllerName =  nds ? "ViewListInventoryNds": "ViewListInventory",
                                                         DocumentName = "Инвентаризация",
                                                         ViewListToolTip = "Перейти в список всех инвентаризаций",
                                                         ViewListName = "Инвентаризации",
                                                         FolderCode = Folder.CODE_FIND_SALES_INVENTORY
                                                     }
                                             };
            return coll.Where(s => WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(s.FolderCode).Id)).ToList();
        }

        /// <summary>Список разделов для домашней страницы услуг</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionServices(bool nds)
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceActIn",
                                                         ViewListControllerName =  nds ? "ViewListActInNds": "ViewListActIn",
                                                         DocumentName = "Акт выполненых работ входящий",
                                                         ViewListToolTip = "Перейти в список всех входящих актов",
                                                         ViewListName = "Входящие акты",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_IN_NDS : Folder.CODE_FIND_SERVICE_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceAccountIn",
                                                         ViewListControllerName =  nds ? "ViewListAccountInNds": "ViewListAccountIn",
                                                         DocumentName = "Входящий счет",
                                                         ViewListToolTip = "Перейти в список всех входящих счетов",
                                                         ViewListName = "Входящие счета",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_IN_NDS : Folder.CODE_FIND_SERVICE_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceOrderIn",
                                                         ViewListControllerName =  nds ? "ViewListOrderInNds": "ViewListOrderIn",
                                                         DocumentName = "Заказ услуг входящий",
                                                         ViewListToolTip = "Перейти в список всех входящих заказов",
                                                         ViewListName = "Входящие заказы",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_IN_ORDER_NDS : Folder.CODE_FIND_SERVICE_IN_ORDER
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceActOut",
                                                         ViewListControllerName =  nds ? "ViewListActOutNds": "ViewListActOut",
                                                         DocumentName = "Акт выполненых работ исходящий",
                                                         ViewListToolTip = "Перейти в список всех исходящих актов",
                                                         ViewListName = "Исходящие акты",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_OUT_NDS : Folder.CODE_FIND_SERVICE_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceAccountOut",
                                                         ViewListControllerName =  nds ? "ViewListAccountOutNds": "ViewListAccountOut",
                                                         DocumentName = "Исходящий счет",
                                                         ViewListToolTip = "Перейти в список всех исходящих счетов",
                                                         ViewListName = "Исходящие счета",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_OUT_ACCOUNT_NDS : Folder.CODE_FIND_SERVICE_OUT_ACCOUNT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "ServiceOrderOut",
                                                         ViewListControllerName =  nds ? "ViewListOrderOutNds": "ViewListOrderOut",
                                                         DocumentName = "Заказ услуг поставщику",
                                                         ViewListToolTip = "Перейти в список всех исходящих заказов",
                                                         ViewListName = "Исходящие заказы",
                                                         FolderCode = nds ? Folder.CODE_FIND_SERVICE_OUT_ORDER_NDS : Folder.CODE_FIND_SERVICE_OUT_ORDER
                                                     }
                                             };
            return coll.Where(s => WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(s.FolderCode).Id)).ToList();
        }

        /// <summary>Список разделов для домашней страницы финансов</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionFinances(bool nds)
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "FinanceIn",
                                                         ViewListControllerName =  nds ? "ViewListFinanceInNds": "ViewListFinanceIn",
                                                         DocumentName = "Входящая оплата",
                                                         ViewListToolTip = "Перейти в список всех входящих оплат",
                                                         ViewListName = "Входящие платежи",
                                                         FolderCode = nds ? Folder.CODE_FIND_FINANCE_IN_NDS : Folder.CODE_FIND_FINANCE_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "FinanceOut",
                                                         ViewListControllerName =  nds ? "ViewListFinanceOutNds": "ViewListFinanceOut",
                                                         DocumentName = "Исходящая оплата",
                                                         ViewListToolTip = "Перейти в список всех исходящих оплат",
                                                         ViewListName = "Исходящие платежи",
                                                         FolderCode = nds ? Folder.CODE_FIND_FINANCE_OUT_NDS : Folder.CODE_FIND_FINANCE_OUT
                                                     }
                                             };
            return coll.Where(s => WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(s.FolderCode).Id)).ToList();
        }

        /// <summary>Список разделов для домашней страницы налоговых документов</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionTaxes()
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "TaxOut",
                                                         ViewListControllerName = "ViewListTaxOut",
                                                         DocumentName = "Налоговая накладная",
                                                         ViewListToolTip = "Перейти в список всех исходящих налоговых накладных",
                                                         ViewListName = "Исходящие налоговые",
                                                         FolderCode = Folder.CODE_FIND_TAX_OUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "CorOut",
                                                         ViewListControllerName = "ViewListCorOut",
                                                         DocumentName = "Корректировачная налоговая",
                                                         ViewListToolTip = "Перейти в список всех исходящих корректировочных накладных",
                                                         ViewListName = "Исходящие корректировочные",
                                                         FolderCode = Folder.CODE_FIND_TAX_COROUT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "TaxIn",
                                                         ViewListControllerName = "ViewListTaxIn",
                                                         DocumentName = "Входящая налоговая",
                                                         ViewListToolTip = "Перейти в список всех входящих налоговых накладныхт",
                                                         ViewListName = "Входящие налоговые",
                                                         FolderCode = Folder.CODE_FIND_TAX_IN
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "CorIn",
                                                         ViewListControllerName = "ViewListCorIn",
                                                         DocumentName = "Входящая корректировочная",
                                                         ViewListToolTip = "Перейти в список всех входящих корректировочные налоговые",
                                                         ViewListName = "Входящие корректировочные",
                                                         FolderCode = Folder.CODE_FIND_TAX_CORIN
                                                     }
                                             };
            return coll.Where(s => WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(s.FolderCode).Id)).ToList();
        }

        /// <summary>Список разделов для домашней страницы договоров</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionContracts()
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Contract",
                                                         ViewListControllerName = "ViewListContract",
                                                         DocumentName = "Договор",
                                                         ViewListToolTip = "Перейти в список всех договоров",
                                                         ViewListName = "Договора",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_CONTRACT
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Revision",
                                                         ViewListControllerName = "ViewListRevision",
                                                         DocumentName = "Акт ревизии",
                                                         ViewListToolTip = "Перейти в список всех исходящих актов ревизий",
                                                         ViewListName = "Акты ревизий",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_REVISION
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Verification",
                                                         ViewListControllerName = "ViewListVerification",
                                                         DocumentName = "Акт сверки",
                                                         ViewListToolTip = "Перейти в список всех актов сверок",
                                                         ViewListName = "Акты сверок",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_VERIFICATION
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "AccountingComputers",
                                                         ViewListControllerName = "ViewListAccountingComputers",
                                                         DocumentName = "Учет компьютеров",
                                                         ViewListToolTip = "Перейти в список документов учета компьютеров",
                                                         ViewListName = "Учет компьютеров",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_COMPUTER
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "AccountingPrinters",
                                                         ViewListControllerName = "ViewListAccountingPrinters",
                                                         DocumentName = "Учет принтеров",
                                                         ViewListToolTip ="Перейти в список документов учета принтеров",
                                                         ViewListName = "Учет принтеров",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_PRINTER
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "OfficialNote",
                                                         ViewListControllerName = "ViewListOfficialNote",
                                                         DocumentName = "Служебная записка",
                                                         ViewListToolTip = "Перейти в список служебных записок",
                                                         ViewListName = "Служебные записки",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_OFFICIALNOTE
                                                     },
                                                
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Revision",
                                                         ViewListControllerName = "ViewListRevision",
                                                         DocumentName = "Договор аренды оборудования",
                                                         ViewListToolTip = "Перейти в список всех договоров аренды оборудования",
                                                         ViewListName = "Договора аренды оборудования",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_REVISION,
                                                         ShowInView=false
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Revision",
                                                         ViewListControllerName = "ViewListRevision",
                                                         DocumentName = "Акт передачи клиенту TXO",
                                                         ViewListToolTip = "Перейти в список всех актов передачи клиенту TXO",
                                                         ViewListName = "Акты передачи клиенту TXO",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_REVISION,
                                                         ShowInView=false
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Revision",
                                                         ViewListControllerName = "ViewListRevision",
                                                         DocumentName = "Акт выполнения ремонта ТХО",
                                                         ViewListToolTip = "Перейти в список всех актов выполнения ремонта ТХО",
                                                         ViewListName = "Акты выполнения ремонта ТХО",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_REVISION,
                                                         ShowInView=false
                                                     },
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Revision",
                                                         ViewListControllerName = "ViewListRevision",
                                                         DocumentName = "Учет анрендного оборудования",
                                                         ViewListToolTip = "Перейти в список учета анрендного оборудования",
                                                         ViewListName = "Учет анрендного оборудования",
                                                         FolderCode=Folder.CODE_FIND_CONTRACTS_REVISION,
                                                         ShowInView=false
                                                     }
                                             };
            //return coll;
            return coll.Where(s => WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(s.FolderCode).Id)).ToList();
        }

        /// <summary>Список разделов для домашней страницы договоров</summary>
        /// <returns></returns>
        public static List<HomePageSection> GetCollectionMarketing()
        {
            List<HomePageSection> coll = new List<HomePageSection>
                                             {
                                                 new HomePageSection
                                                     {
                                                         ControllerName = "Mktg",
                                                         ViewListControllerName = "ViewListMrak",
                                                         DocumentName = "Анкета клиента",
                                                         ViewListToolTip = "Перейти в список всех анкетных данных",
                                                         ViewListName = "Анкеты клиентов",
                                                         FolderCode = Folder.CODE_FIND_MKTG_MRAK
                                                     }
                                             };
            return coll;
        }
    }
}