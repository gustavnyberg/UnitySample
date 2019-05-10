﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using Unity;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Unity";
        var connectionString = "";
        #region ContainerConfiguration

        var endpointConfiguration = new EndpointConfiguration("Samples.Unity");
        var container = new UnityContainer();
        container.RegisterInstance(new MyService());
        endpointConfiguration.UseContainer<UnityBuilder>(
            customizations: customizations =>
            {
                customizations.UseExistingContainer(container);
            });

        #endregion

        endpointConfiguration.UsePersistence<LearningPersistence>();
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(connectionString);
        transport.UseForwardingTopology();
        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        var myMessage = new MyMessage();
        await endpointInstance.SendLocal(myMessage)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}