using NerdAmigo.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NerdAmigo.Queue.Common
{
	public class QueueClientLogDecorator<TMessage> : IQueueClient<TMessage> where TMessage : IQueueMessage
	{
		private IQueueClient<TMessage> Decorated;
		private ILogger Logger;
		public QueueClientLogDecorator(IQueueClient<TMessage> Decorated, ILogger Logger)
		{
			this.Decorated = Decorated;
			this.Logger = Logger;
		}

		public async Task<TMessage> Enqueue(TMessage Message, CancellationToken CancellationToken)
		{
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Message Queueing")));
			var result = await this.Decorated.Enqueue(Message, CancellationToken);
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Message Queueing Complete")));
			return result;
		}

		public async Task<TMessage> Receive(CancellationToken CancellationToken)
		{
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Waiting for next message")));
			var result = await this.Decorated.Receive(CancellationToken);
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Received new message")));
			return result;
		}


		public async Task Delete(TMessage Message, CancellationToken CancellationToken)
		{
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Deleting message")));
			await this.Decorated.Delete(Message, CancellationToken);
			Logger.Log(new LogEntry(LogEventSeverity.Debug, String.Format("Message deleted")));
		}
	}
}
