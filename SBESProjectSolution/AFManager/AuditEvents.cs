using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AFManager
{
	public enum AuditEventTypes
	{
		AddPortSuccess = 0,
		RemovePortSuccess = 1,
		AddProtocolSuccess = 2,
		RemoveProtocolSuccess = 3
	}

	class AuditEvents
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
						resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string AddPortSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AddPortSuccess.ToString());
			}
		}

		public static string RemovePortSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.RemovePortSuccess.ToString());
			}
		}

		public static string AddProtocolSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.AddProtocolSuccess.ToString());
			}
		}

		public static string RemoveProtocolSuccess
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.RemoveProtocolSuccess.ToString());
			}
		}
	}
}
