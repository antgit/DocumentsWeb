using System.Collections.Generic;
using System.Web.Security;
using BusinessObjects;

namespace DocumentsWeb.Areas.Admins.Models
{
    public static class UserData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (fieldName == "Config.Out.AllowNumberEdit")
                return "Если редактирование номера запретить номер документа будет присваиваться по правилам автонумерации документов";
            if (fieldName == "Config.Out.AllowAgenToEdit")
                return "Если редактирование разрешено в панели действий документа доступна ссылки на редактирование контрагента";
            if (fieldName == "Config.Out.AllowProductEdit")
                return "Если редактирование разрешено в панели действий документа доступна ссылки на редактирование объекта учета";
            if (fieldName == "Config.Out.AllowAgenToSearch")
                return "Если поиск разрешен в списке выбора контрагента добавляется возможность поиска используя диалог поиска корреспондентов";
            if (fieldName == "Config.Out.AllowProductSearch")
                return "Если поиск разрешен в списке выбора объекта учета добавляется возможность поиска используя диалог поиска объектов учета";
            if (fieldName == "Config.Out.AllowAgenToCreate")
                return "Если разрешено создание контрагентов в панели действий документа доступна ссылки на создание нового контрагента";
            if (fieldName == "Config.Out.AllowProductCreate")
                return "Если разрешено создание товаров в панели действий документа доступна ссылки на создание нового товара";

            if (fieldName == "Config.Out.DecimalPlacesQty")
                return "Настройка количества десятичных знаков при редактировании количества в строке детализации документа";
            if (fieldName == "Config.Out.DecimalPlacesPrice")
                return "Настройка количества десятичных знаков при редактировании цены в строке детализации документа";
            if (fieldName == "Config.Out.DecimalPlacesSumma")
                return "Настройка количества десятичных знаков при редактировании суммы в строке детализации документа";

            if (fieldName == "Config.Out.DistpalyFormatQty")
                return "Настройка формата отображения данных о количестве в табличной части документа";
            if (fieldName == "Config.Out.DisplayFormatPrice")
                return "Настройка формата отображения данных о цене в табличной части документа";
            if (fieldName == "Config.Out.DisplayFormatSumma")
                return "Настройка формата отображения данных о сумме в табличной части документа";

            if (fieldName == GlobalPropertyNames.Name)
                return "Логин пользователя является обязательным! Старайтесь задавать значение максимально кратким и понятным, после создания изменить логин невозможно!. Минимальная длина может быть ограничена текущими настроками системы! Максимальная длина - 255 символов.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "Наименование пользователя на основе данных о сотруднике";
            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание пользователя используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем, например Вы можете указать краткую характеристику или интересы! Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.Email)
                return "Основной электронный адрес пользователя, используется системой для подтверждения регистрации, востановления пароля, отправки специальных сообщений системой. Является обязательным!";
            if (fieldName == "IsActive")
                return "По умолчанию включено. При выключении пользователь блокируется и не может осуществить вход в систему.";
            if (fieldName == "IsReadOnly")
                return "По умолчанию выключено. При включении пользователь не может менять собственные данные, включая пароль.";
            
            if (fieldName == "IsAdmin")
                return "По умолчанию выключено. При включении пользователь является администратором системы.";
            if (fieldName == "CompanyId")
                return "Основная компания которой принадлежит пользователь. Является обязательным!";
            if (fieldName == GlobalPropertyNames.EmployerId)
                return "Сотрудник ассоциированный с данным пользователем. Является обязательным!";
            if (fieldName == GlobalPropertyNames.Password)
                return string.Format("Пароль является обязательным! Минимальная длина пароля может быть ограничена настройками системы (текущая минимальная длина {0} символов).", Membership.Provider.MinRequiredPasswordLength);

            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код корреспондента используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Дополнительный код поиска корреспондента: при быстром вводе в соотвествующие поля - помимо наименования анализируется данное свойство. По умолчанию код поиска формируется путем транслитерации наименования и атрибута \"Код\".";
            if (fieldName == GlobalPropertyNames.DateModified)
                return "Дата последнего изменения данных";
            if (fieldName == GlobalPropertyNames.Id)
                return "Уникальный числовой идентификатор";
            if (fieldName == GlobalPropertyNames.UserName)
                return "Имя пользователя создавшего или изменившего данные в последний раз";
            if (fieldName == GlobalPropertyNames.MyCompanyId)
                return "Идентификатор компании-владельца данных";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "Наименование компании-владельца данных";
            return string.Empty;
        }
    }
}