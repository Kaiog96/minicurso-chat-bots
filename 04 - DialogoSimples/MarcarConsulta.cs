using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using System;
using System.Linq;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using static Microsoft.Bot.Builder.Prompts.DateTimeResult;

namespace ExemploDialogo
{
    public class MarcarConsulta : IBot
    {

        DialogSet dialogos;
        string nomeCliente;
        string tipoPizza;
        DateTime dataHora;
        string tamanho;


        public MarcarConsulta()
        {
            dialogos = new DialogSet();

            dialogos.Add("PedirPizaa", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                    await dc.Prompt("capturaTexto","Qual o seu nome ?");
                },
                async (dc, args, next) =>
                {
                    nomeCliente = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto",$"{nomeCliente}, qual pizza deja ?");
                },
                async (dc, args, next) =>
                {
                    tipoPizza = ((TextResult)args).Value;
                    await dc.Prompt("capturaTexto", $" {tipoPizza}, qual tamanho?");
                },
                async (dc, args, next) =>
                {
                    tamanho = ((TextResult)args).Value;

                    await dc.Context.SendActivity($"Ok. Sua pizza {tipoPizza} de {tamanho} estará pronta em 20 minutos");
                    await dc.End();
                },
            });

            dialogos.Add("capturaDataHora", new Microsoft.Bot.Builder.Dialogs.DateTimePrompt(Culture.Portuguese));
            dialogos.Add("capturaTexto", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }

        public async Task OnTurn(ITurnContext context)
        {

            Dictionary<string, object> state = ConversationState<Dictionary<string, object>>.Get(context);
            DialogContext dc = dialogos.CreateContext(context, state);
            await dc.Continue();

            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (!context.Responded)
                {
                    await dc.Begin("PedirPizaa");
                }
            }
        }
    }
}
