using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[DataContract]
	public class Consumer
	{
		string username = string.Empty;
		string id = string.Empty;

		public Consumer(string username, string id)
		{
			this.username = username;
			this.id = id;
		}

		[DataMember]
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		[DataMember]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

        public override string ToString()
        {
            return string.Format("username - {0}, id - {1}\n", Username, Id); ;
        }
    }
}
