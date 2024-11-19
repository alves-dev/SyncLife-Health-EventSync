﻿using Orchestrator;
using Broker.MQTT;
using Broker.Rabbit;

class Program
{
    static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var events = new Events();
        new OrchestratorService(events);

        var rabbitConnection = new RabbitMqConnection();

        MqttClient mqttClient = new MqttClient();
        await mqttClient.Initialize();

        //Ver como deixar essa linha acima

        var mqttService = new MqttClientService(mqttClient.GetClient(), events);
        try
        {




            // Inicializa a conexão de forma assíncrona
            await rabbitConnection.InitializeAsync();

            // Configura o serviço para consumir mensagens
            var rabbitService = new RabbitMqService(rabbitConnection);

            // Inicia o consumo das mensagens
            rabbitService.StartListening();
            rabbitService.PublicMenssage("cjgfchkghgkghkv");




            /*TODO:
            - as classes services vão receber uma instancia de Events
            - Então eles vão lancar um evento indicando o recebimento
            - As serviçes tb vão assinar eventos (para envio)
            - vou ter 4 eventos que indicam o recebimento dos dois brokers e para o envio deles
            - A OrchestratorService tb vai receber uma instancia de Events para poder se inscrever e lançar depois de tratados


            */

            //MQTT
            mqttService.PublicMenssage("dillicia");




            // RUN
            Console.WriteLine("Pressione [Enter] para encerrar...");
            Console.ReadLine();
        }
        finally
        {
            // Libera os recursos de forma assíncrona
            await rabbitConnection.DisposeAsync();
            await mqttService.DisposeAsync();
        }
    }
}