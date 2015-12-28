using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Agents.Models
{
    public class WorkerChainModel
    {
        /// <summary>
        /// Модель корреспондента
        /// </summary>
        public ClientModel Agent { get; set; }

        private ClientChainState _agentState;
        /// <summary>
        /// Состояние корреспондента
        /// </summary>
        public ClientChainState AgentState
        {
            get
            {
                return _agentState;
            }
            set
            {
                switch (value)
                {
                    case ClientChainState.Worker:
                        AgentStateName = "Сотрудник";
                        break;
                    case ClientChainState.Trader:
                        AgentStateName = "Торговый агент";
                        break;
                    case ClientChainState.Dissmised:
                        AgentStateName = "Уволенный";
                        break;
                }
                _agentState = value;
            }
        }

        /// <summary>
        /// Наименование состояния корреспондента
        /// </summary>
        public string AgentStateName { get; set; }

        /// <summary>
        /// Состояние
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }

        public WorkerChainModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}