using NerdAmigo.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdAmigo.Queue.Common
{
	public class QueueIdentifierProvider : IQueueIdentifierProvider
	{
		public QueueIdentifier GetIdentifier(Type MessageType)
		{
			string qualifiedTypeName = String.Format("{0}.{1}", MessageType.Namespace, MessageType.Name);
			return new QueueIdentifier(qualifiedTypeName.Replace(".", "-"));
		}

		public QueueIdentifier GetIdentifier(IQueueMessage Message)
		{
			return GetIdentifier(Message.GetType());
		}

		public QueueIdentifier GetIdentifier<TMessage>() where TMessage : IQueueMessage
		{
			return GetIdentifier(typeof(TMessage));
		}
	}
}
