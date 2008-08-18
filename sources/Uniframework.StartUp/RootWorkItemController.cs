﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI.Common;

using Uniframework.Client;

namespace Uniframework.StartUp
{
    public class RootWorkItemController : WorkItemController
    {
        public override void OnRunStarted()
        {
            WorkItem.Activated += new EventHandler(WorkItem_Activated);
            base.OnRunStarted();
            WorkItem.Activate();
        }

        private void WorkItem_Activated(object sender, EventArgs e)
        {
            Program.Logger.Debug("启动离线服务的连接管理线程");
            Program.SetInitialState("启动离线服务……");
            Program.IncreaseProgressBar(10);
            ServiceRepository.Instance.Start();
            Program.CloseLoginForm();
        }
    }
}
