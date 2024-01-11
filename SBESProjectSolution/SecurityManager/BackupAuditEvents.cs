using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public enum BackupAuditEventTypes
    {

        EventSuccess = 0
    }
    class BackupAuditEvents
    {
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
						resourceManager = new ResourceManager(typeof(BackupAuditEventFIle).ToString(), Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string EventSuccess
		{
			get
			{
				return ResourceMgr.GetString(BackupAuditEventTypes.EventSuccess.ToString());
			}
		}
	}
}
