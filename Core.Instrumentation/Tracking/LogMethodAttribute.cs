namespace Core.Instrumentation.Tracking
{
	using System;
	using System.Reflection;
	using PostSharp.Aspects;
	using PostSharp.Extensibility;

	[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)] // Inheritance = MulticastInheritance.Multicast
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	[Serializable]
	public sealed class LogMethodAttribute : OnMethodBoundaryAspect
	{
		// These fields are initialized at runtime. They do not need to be serialized.
		[NonSerialized]
		private string methodName;
		[NonSerialized]
		private string className;

		public override void RuntimeInitialize(MethodBase method)
		{
			this.className = method.DeclaringType.FullName;
			this.methodName = method.DeclaringType.FullName + "." + method.Name;
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			//Arguments arg = args.Arguments;
			//IMessageBase reqMessage = arg != null ? arg.FirstOrDefault() as IMessageBase : null;
			//IMessageBody reqMessageBody = reqMessage as IMessageBody ?? null;

			//string message = string.Format("Entering {0}", methodName);
			//string transaction = null;
			//string source = null;

			//if (reqMessage != null && reqMessageBody != null)
			//{
			//    transaction = reqMessage.GetMessageType();
			//    source = reqMessageBody.TransactionOriginatorId;
			//    message = message + " - " + transaction + " - " + source.ToUpper();
			//    Logger.LogDebug(source, transaction, message);
			//}
			//else
			//{
			//    Logger.LogDebug(className, string.Empty, message);
			//}
			//args.MethodExecutionTag = Utility.GlobalStopwatch.ElapsedTicks;
		}

		public override void OnExit(MethodExecutionArgs args)
		{
			//Arguments arg = args.Arguments;
			//decimal milliseconds = Utility.TicksDiffInMs((long)args.MethodExecutionTag);
			//IMessageBase reqMessage = arg != null ? arg.FirstOrDefault() as IMessageBase : null;
			//IMessageBody reqMessageBody = reqMessage as IMessageBody ?? null;
			//string transaction = null;
			//string source = null;
			//string message = this.methodName;

			//if (reqMessage != null && reqMessageBody != null)
			//{
			//    transaction = reqMessage.GetMessageType();
			//    source = reqMessageBody.TransactionOriginatorId;
			//    message = message + " - " + transaction + " - " + source.ToUpper();
			//    Logger.LogInfo(source, transaction, string.Format("Exiting {0} - {1} ms elapsed", message, milliseconds.ToString("#.##")));
			//}
			//else
			//{
			//    Logger.LogInfo(className, "Exiting " + message, string.Format("Exiting {0} - {1} ms elapsed", message, milliseconds.ToString("#.##")));
			//}

		}

	}
}
