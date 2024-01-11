using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AFManager
{
	public enum AuditCCEventTypes
	{
		InvalidProtocol = 0,
		InvalidPort = 1
	}

	class AuditCCEvents
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
						resourceManager = new ResourceManager(typeof(AuditCCEventFile).ToString(), Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string InvalidPort
		{
			get
			{
				return ResourceMgr.GetString(AuditCCEventTypes.InvalidPort.ToString());
			}
		}

		public static string InvalidProtocol
		{
			get
			{
				return ResourceMgr.GetString(AuditCCEventTypes.InvalidProtocol.ToString());
			}
		}
	}
}
