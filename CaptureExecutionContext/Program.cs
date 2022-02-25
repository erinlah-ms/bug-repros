

using System;
#if !(NET461_OR_GREATER || NET || NETCOREAPP)
using System.Runtime.Remoting.Messaging;
#endif
using System.Threading;
using System.Threading.Tasks;



#if NET461_OR_GREATER || NET || NETCOREAPP
var serviceContextAsyncLocal = new AsyncLocal<string>();
#else
Console.WriteLine("Note, using CallContext fallback");
#endif
Console.WriteLine("SetServiceContext(root)");
SetServiceContext("root");
Console.WriteLine();

ExecutionContext ec = default;

// Note, somewhere around netframework47 or 48, ExectionContext started flowing across Task.Run and new Thread.
// See native thread example later for evidence of this.
Console.WriteLine("Task.Run(() => {");
Task.Run(() =>
{
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
    Console.WriteLine("  StartActivity(\"activity-one\")");
    StartActivity("activity-one");
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
    Console.WriteLine("  ec = ExecutionContext.Capture();");
    ec = ExecutionContext.Capture();
}).Wait();
Console.WriteLine("})");
Console.WriteLine();

void StartActivity(string activityName)
{
    SetServiceContext(GetServiceContext() + "/" + activityName);
}

Console.WriteLine("Task.Run(() => {");
Task.Run(() =>
{
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
    Console.WriteLine("  ExecutionContext.Run(ec, _ => {");
    ExecutionContext.Run(ec, _ =>
    {
        Console.WriteLine("    GetServiceContext() => {0}", GetServiceContext());
#if !NET5_0_OR_GREATER
        // Pre net50, ExecutionContexts were single-use. Capture for the next use.
        ec = ExecutionContext.Capture();
#endif
    }, null);
    Console.WriteLine("  },null)");
}).Wait();
Console.WriteLine("})");
Console.WriteLine();

Console.WriteLine("thread = Thread.Start(() => {");
var thread = new Thread(() =>
{
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
    Console.WriteLine("  ExecutionContext.Run(ec, _ => {");
    ExecutionContext.Run(ec, _ =>
    {
        Console.WriteLine("    GetServiceContext() => {0}", GetServiceContext());
#if !NET5_0_OR_GREATER
        // Pre net50, ExecutionContexts were single-use. Capture for the next use.
        ec = ExecutionContext.Capture();
#endif
    }, null);
    Console.WriteLine("  },null)");
});
thread.Start();
thread.Join();
Console.WriteLine("})");
Console.WriteLine();

#if NET5_0_OR_GREATER
Console.WriteLine("Task.Run(() => {");
Task.Run(() =>
{
    Console.WriteLine("  StartActivity(\"activity-two\")");
    StartActivity("activity-two");
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
    Console.WriteLine("  ExecutionContext.Restore(ec)");
    // Note: ExecutionContext.Restore was introduced in net5.0
    ExecutionContext.Restore(ec);
    Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
}).Wait();
Console.WriteLine("})");
Console.WriteLine();
#endif

#if NETFRAMEWORK

unsafe
{
    Console.WriteLine("Statics.CreateThread(() => {");
    uint threadId;
    var nativeThreadHandle = Statics.CreateThread(
        null,
        0,
        () =>
        {
            Console.WriteLine("  GetServiceContext() => {0}", GetServiceContext());
            Console.WriteLine("  ExecutionContext.Run(ec, _ => {");
            ExecutionContext.Run(ec, _ =>
            {
                Console.WriteLine("    GetServiceContext() => {0}", GetServiceContext());
#if !NET5_0_OR_GREATER
                // Pre net50, ExecutionContexts were single-use. Capture for the next use.
                ec = ExecutionContext.Capture();
#endif
            }, null);
            Console.WriteLine("  },null)");
        },
        null,
        0,
        out threadId
        );
    Statics.WaitForSingleObject(nativeThreadHandle, Statics.INFINITE);
    Console.WriteLine("})");
    Console.WriteLine();
}
#endif

void SetServiceContext(string value)
{
#if NET461_OR_GREATER || NET || NETCOREAPP
    serviceContextAsyncLocal.Value = value;
#else
    CallContext.LogicalSetData("ServiceContext", value);
#endif
}

string GetServiceContext()
{
#if NET461_OR_GREATER || NET || NETCOREAPP
    return serviceContextAsyncLocal.Value;
#else
    return (string)CallContext.LogicalGetData("ServiceContext");
#endif
}
