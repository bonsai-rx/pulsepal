using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace Bonsai.PulsePal
{
    internal static class PulsePalManager
    {
        static readonly Dictionary<string, Tuple<PulsePalDevice, RefCountDisposable>> openConnections = new();
        static readonly object openConnectionsLock = new();

        internal static PulsePalDisposable ReserveConnection(string deviceName)
        {
            return ReserveConnection(deviceName, PulsePalConfiguration.Default);
        }

        internal static PulsePalDisposable ReserveConnection(string deviceName, PulsePalConfiguration pulsePalConfiguration)
        {
            var connection = default(Tuple<PulsePalDevice, RefCountDisposable>);
            lock (openConnectionsLock)
            {
                if (string.IsNullOrEmpty(deviceName))
                {
                    if (!string.IsNullOrEmpty(pulsePalConfiguration.PortName)) deviceName = pulsePalConfiguration.PortName;
                    else if (openConnections.Count == 1) connection = openConnections.Values.Single();
                    else throw new ArgumentException("An alias or serial port name must be specified.", nameof(deviceName));
                }

                if (connection == null && !openConnections.TryGetValue(deviceName, out connection))
                {
                    var serialPortName = pulsePalConfiguration.PortName;
                    if (string.IsNullOrEmpty(serialPortName)) serialPortName = deviceName;

                    var pulsePal = new PulsePalDevice(serialPortName);
                    try
                    {
                        pulsePal.Open();
                        pulsePal.SetClientId(nameof(Bonsai));
                        pulsePalConfiguration.Configure(pulsePal);
                        var dispose = Disposable.Create(() =>
                        {
                            pulsePal.Close();
                            openConnections.Remove(deviceName);
                        });

                        var refCount = new RefCountDisposable(dispose);
                        connection = Tuple.Create(pulsePal, refCount);
                        openConnections.Add(deviceName, connection);
                        return new PulsePalDisposable(pulsePal, refCount);
                    }
                    catch
                    {
                        pulsePal.Close();
                        throw;
                    }
                }
            }

            return new PulsePalDisposable(connection.Item1, connection.Item2.GetDisposable());
        }
    }
}
