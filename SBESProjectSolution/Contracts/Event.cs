using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public enum CriticallityLevel {[EnumMember] GREEN_ALERT, [EnumMember] YELLOW_ALERT, [EnumMember] RED_ALERT }
    [DataContract]
    public enum MessageState {[EnumMember] OPEN, [EnumMember] CLOSE }


    [DataContract]
    public class Event
    {
        CriticallityLevel criticallity;
        DateTime timestamp;
        Consumer source;
        string message;
        MessageState state;

        public Event( CriticallityLevel criticallity, DateTime timestamp, Consumer source, string message, MessageState state)
        {
            this.criticallity = criticallity;
            this.timestamp = timestamp;
            this.source = source;
            this.message = message;
            this.state = state;
        }

        [DataMember]
        public CriticallityLevel Criticallity 
        {
            get
            {
                return this.criticallity;
            }
            set
            {
                this.criticallity = value;
            } 
        }

        [DataMember]
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
            set
            {
                this.timestamp = value;
            }
        }

        [DataMember]
        public Consumer Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        [DataMember]
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        [DataMember]
        public MessageState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        public void Update(MessageState ms)
        {

            this.State = ms;
        }

        public override string ToString()
        {
            string s = "\n";
            if (Source != null)
                s = Source.ToString();
            return string.Format("\tCriticallity: {0}\n\tTimestamp: {1}\n\tSource: {2}\tMessage: {3}\n\tState: {4}",
                criticallity.ToString(), timestamp.ToString(), s, message, state.ToString());
        }
        /*~Event() {
            Event.key--;
        }*/
    }
}
