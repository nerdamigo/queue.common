using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdAmigo.Abstractions;

namespace NerdAmigo.Queue.Common
{
	public class QueueMessageProcessor<TMessage> where TMessage : IQueueMessage
	{
		private IQueueClient<TMessage> Client;
		private ILogger Logger;
		public QueueMessageProcessor(IQueueClient<TMessage> Client, ILogger Logger)
		{
			this.Client = Client;
			this.Logger = Logger;
		}

		public IQueueMessageWorkerActivator QueueMessageWorkerActivator { private get; set; }

		public void Begin(System.Threading.CancellationToken cancellationToken)
		{
			if(QueueMessageWorkerActivator == null)
			{
				throw new Exception("QueueMessageWorkerActivator must be assigned before any work can be performed!");
			}

			Task messageProcessingTask = Task.Factory.StartNew(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					Task<TMessage> nextMessageTask = Client.Receive(cancellationToken);

					try
					{
						nextMessageTask.Wait(cancellationToken);
					}
					catch (OperationCanceledException) { }

					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}

					TMessage nextMessage = nextMessageTask.Result;

					Logger.Log(new LogEntry(LogEventSeverity.Debug, string.Format("New Message Received: {0}", nextMessage.ToString())));

					IQueueMessageWorker<TMessage> worker = QueueMessageWorkerActivator.GetWorker<TMessage>();
					Exception processingException = null;

					try
					{
						worker.Execute(nextMessage);
					}
					catch(Exception ex)
					{
						processingException = ex;
					}

					if (processingException == null)
					{
						Client.Delete(nextMessage, cancellationToken);
					}
					else
					{
						Logger.Log(new LogEntry(LogEventSeverity.Error, "Exception during message processing, message not deleted", processingException));
					}
				}

			}, cancellationToken);
		}
	}
}
