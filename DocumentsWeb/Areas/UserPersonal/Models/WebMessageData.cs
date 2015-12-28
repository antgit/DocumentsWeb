using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Models
{
    public static class WebMessageData
    {
        /// <summary>
        /// Список всех сообщений для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetAllMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).ToList();
        }

        /// <summary>
        /// Список всех входящих сообщений за сегодня для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetTodayMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f=>f.IsIncomminMessage && f.IsTodayMessage && f.StateId!=State.STATEDELETED).ToList();
        }

        /// <summary>
        /// Список всех входящих сообщений за весь период для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetAllIncommingMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f => f.IsIncomminMessage && f.IsSend && f.StateId != State.STATEDELETED).ToList();
        }

        /// <summary>
        /// Список всех исходящих сообщений за весь период для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetAllOutcommingMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f => f.IsOutcomminMessage && f.IsSend && f.StateId != State.STATEDELETED).ToList();
        }

        /// <summary>
        /// Список всех исходящих сообщений за весь период для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetDraftMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f => f.IsDraft && f.StateId != State.STATEDELETED).ToList();
        }
        /// <summary>
        /// Список всех собственных сообщений за весь период для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetSelfMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id && f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f => f.IsSelfMessage && f.StateId != State.STATEDELETED).ToList();
        }
        /// <summary>
        /// Список всех удаленных сообщений за весь период для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetTrashMessages(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => WebMessageModel.ConvertToModel(s)).Where(f => f.StateId == State.STATEDELETED).ToList();
        }
    }
}