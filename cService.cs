using System;
using System.Threading;
using System.Threading.Tasks;
using WebAppShipping;

public class cService
{
    private CancellationTokenSource _cts;
    private Task _serviceTask;

    // Indicates whether the service is running.
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// Starts the service if it isn't already running.
    /// </summary>
    public void Start()
    {
        try
        {
            if (IsRunning)
                return;

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            _serviceTask = Task.Run(() => RunService(token), token);
            IsRunning = true;
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    /// <summary>
    /// The background routine that runs until cancellation is requested.
    /// </summary>
    private async Task RunService(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    EventLogHelper.WriteCustomEvent("Web App Shipping Serivce started");
                    // Reload settings if they were just updated
                    PrintavoApiClient.ReloadSettings();
                    // Replace this with the routine you want to run.
                    await PrintavoApiClient.ImportOrdersAsync(DateTime.Now.ToShortDateString());
                    await PrintavoApiClient.ProcessShippedOrdersAsync();
                }
                catch (Exception ex)
                {
                    // Log any error that occurs in the routine.
                    EventLogHelper.WriteErrorLog(ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested.
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }


    /// <summary>
    /// Stops the service if it is running.
    /// </summary>
    public void Stop()
    {
        try
        {
            if (!IsRunning)
                return;

            _cts.Cancel();
            try
            {
                _serviceTask.Wait();
                EventLogHelper.WriteCustomEvent("Web App Shipping Serivce Stopped");
            }
            catch (AggregateException ae)
            {
                // Handle task cancellation exceptions if needed.
                ae.Handle((ex) => ex is OperationCanceledException);
            }
            finally
            {
                _cts.Dispose();
            }
            IsRunning = false;
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }
}
