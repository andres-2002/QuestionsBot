// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Questions.Bot.Common.Models.EntityModel;
using Questions.Bot.Common.Models.Questions;
using Questions.Bot.Services.LuisAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Bot
{
    public class QuestionsBot : ActivityHandler
    {
        private readonly ILuisAIService _luisAIService;
        public QuestionsBot(ILuisAIService luisAIService)
        {
            _luisAIService = luisAIService;
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisAIService._luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
            await Intentions(turnContext, luisResult, cancellationToken);
        }

        private async Task Intentions(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Greet":
                    await IntentGreet(turnContext, luisResult, cancellationToken);
                    break;
                case "Dismiss":
                    await IntentDismiss(turnContext, luisResult, cancellationToken);
                    break;  
                case "Thank":
                    await IntentThank(turnContext, luisResult, cancellationToken);
                    break;  
                case "Questions":
                    await IntentQuestions(turnContext, luisResult, cancellationToken);
                    break;  
                case "None":
                    await IntentNone(turnContext, luisResult, cancellationToken);
                    break;
                default:
                    await IntentNone(turnContext, luisResult, cancellationToken);
                    break;
            }
        }

        private async Task IntentGreet(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Hola, que gusto saludarte.", cancellationToken: cancellationToken);
        }

        private async Task IntentDismiss(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Adiós, que tengas un feliz día.", cancellationToken: cancellationToken);
        }

        private async Task IntentThank(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Gracias a ti por escribirme.", cancellationToken: cancellationToken);
        }

        private async Task IntentQuestions(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {

            var entities = luisResult.Entities.ToObject<EntityLuis>();
            if(entities.ListQuestions?.Count > 0)
            {
                var question = entities.ListQuestions.FirstOrDefault().FirstOrDefault();
                var filteredList = GetQuestions().Where(x => x.name.ToLower() == question.ToLower()).ToList();

                await turnContext.SendActivityAsync($"Aquí tienes el detalle de la pregunta {question}");
                await Task.Delay(2000);
                await turnContext.SendActivityAsync(activity: ShowQuestions(filteredList), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync("¿Cúal de las siguientes dudas es la que deseas resolver?");
                await Task.Delay(2000);
                await turnContext.SendActivityAsync(activity: ShowQuestions(GetQuestions()), cancellationToken);
            }
        }

        private List<QuestionsModel> GetQuestions()
        {
            var list = new List<QuestionsModel>()
            {
                new QuestionsModel
                {
                    name = "¿Qué condiciones debo cumplir para ser teletrabajador?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Qué condiciones debo cumplir para ser teletrabajad@r?\n\n".ToUpper() + "1. Si estás interesad@ en dar inicio al proceso de teletrabajo debes conversar con tu líder ya que es fundamental que tanto cómo tú cómo él estén de acuerdo en comenzar el proceso.\n" + "2. Debes hacer todo el proceso de teletrabajo y acatar las recomendaciones que se te entregan para asegurar un teletrabajo exitoso.\n" + "3. Debes contar con  un espacio amplio, iluminado y ventilado donde puedas ubicar tu espacio de teletrabajo.\n" + "4. Si tienes menos de una año desempeñando tu cargo, es importante que tu curva de aprendizaje ya esté superada para que puedas desempeñar de manera autónoma tus responsabilidades.\n" + "5. Si das inicio al proceso de teletrabajo, te comprometes a finalizarlo. El proceso implica tiempo y costos para la compañía, por lo que tu compromiso es clave para el desarrollo del mismo.\n" + "6. Conoce más leyendo el perfil del teletrabajador"
                },
                new QuestionsModel
                {
                    name = "¿Cuáles son las condiciones locativas?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Cuáles son las condiciones locativas en el lugar de teletrabajo?\n\n".ToUpper() + "1. El espacio donde teletrabajarás, debe ser iluminado, amplio y ventilado, para que puedas ubicar el escritorio y la silla cómodamente.\n" + "2. Debes contar con un escritorio con las siguientes medidas: 70 cms de ancho por 100 cms de largo y entre 60 a 75 cms de alto, debe tener los bordes redondeados o recubiertos y no puede ser elaborado en vidrio.\n" + "3. La iluminación debe ser mínimo 300 luxes - máximo 750 luxes. Esto es medido en la validación virtual.\n" + "4. Los cables que se tengan en el puesto deben estar amarrados, para evitar caídas y riesgo eléctrico.\n" + "5. Contar con un extintor amarillo y botiquín de primeros auxilio que contenga: Gasa, Solución Yodada, Algodón, Guantes (2 pares), Micropore, Curas, Parches oculares, Vendaje de Tela y Vendaje Elástico.\n" + "6. Internet de 20 Megas. Validación con test de internet, no con factura.\n\n" + "Recomendaciones posturales, así debes organizar tu puesto de trabajo:\n" + "1. Altura de la silla:  debes regular la altura de manera que tus brazos formen un ángulo recto cuando estos están reposados en el escritorio.\n" + "2. Debes tener los 15 cm libres desde el borde del teclado auxiliar al borde del escritorio para apoyar los brazos.\n" + "3. El borde superior del monitor o pantalla del portátil debe estar a la altura de tus ojos.\n" + "4. La silla debe entrar completamente por lo que en la parte inferior no debes tener obstáculos,  para que tu espalda siempre esté recostada.\n" + "5. No debes tener elementos en la parte inferior del escritorio que te puedan incomodar.\n\n"
                },
                new QuestionsModel
                {
                    name = "¿Cuánto dura el proceso para ser teletrabajador?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "Etapas y duración del proceso de Teletrabajo:\n\n".ToUpper() + "1. Sensibilización y Pruebas de Competencias (1 hora y media)\n" + "2. Entrevista ( 1 hora)\n" + "3. Visita Domiciliaria grupal ( 1 hora y media)\n" + "4. Inducción (2 horas)\n" + "5. Formalización\n\n" + "El tiempo del proceso es de aproximadamente mes y medio. Recuerda que el tiempo depende aproximadamente en 60% de tu compromiso y cumplimiento con las recomendaciones."
                },
                new QuestionsModel
                {
                    name = "¿Puedo “perder” el proceso de teletrabajo?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Puedo “perder” el proceso de teletrabajo?\n\n".ToUpper() + "1. Como lo hemos mencionado, para dar inicio tanto tu líder como tú deben estar de acuerdo en iniciar el proceso. Esto es fundamental y uno de los principios de teletrabajo que establece la legislación.\n" + "2. Después de realizar las pruebas de competencia y la entrevista, se analiza si la persona cumple con las características de un teletrabajador. Si se identifica un hallazgo, se revisa con el colaborador y líder, para determinar si se continúa el proceso o se retoma más adelante cuando la organización, líder y colaborador lo consideren pertinente."
                },
                new QuestionsModel
                {
                    name = "¿Puedo cambiar de días?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Puedo cambiar de días de teletrabajo?\n\n".ToUpper() + "1. Si.  Cuando exista una razón que te impida teletrabajar los días definidos puedes solicitar el cambio de estos, para lo cual tú y tu líder deben firmar un cambio en las condiciones del otrosí.  Esto se realiza si el cambio es permanente.\n" + "2. En caso de requerir el cambio de días por alguna situación particular y no será recurrente, lo debes conversar con tu líder y contar con su aprobación."
                },
                new QuestionsModel
                {
                    name = "¿Qué pasa si cambio de lugar de teletrabajo?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Qué pasa si cambio de lugar de teletrabajo?\n\n".ToUpper() + "Si cambias de lugar de teletrabajo, se debe repetir la validación virtual para asegurarnos que se cumplen con todas las condiciones establecidas para teletrabajar. Esto aplica, si cambias de domicilio o si cambias el punto definido inicialmente dentro del mismo domicilio"
                },
                new QuestionsModel
                {
                    name = "¿Si soy teletrabajador, puedo ir a la sede?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Si soy teletrabajador, puedo ir a la sede?\n\n".ToUpper() + "Siempre eres bienvenido a la sede. Si necesitas desplazarte a la sede principal, donde un cliente o proveedor en un día de teletrabajo puedes hacerlo. Conversa con tu líder para que el conozca desde donde estarás desarrollando tus actividades."
                },
                new QuestionsModel
                {
                    name = "¿Puedo dejar de teletrabajar?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Puedo dejar de Teletrabajar?\n\n".ToUpper() + "Si. En caso de no desear continuar teletrabajando, puedes solicitarle a tu líder y al líder del proceso de teletrabajo la reversibilidad del proceso y tu retorno a la sede. Esto aplica si no fuiste contratado cómo teletrabajador. Si llevas poco tiempo cómo teletrabajar, te sugerimos que trabajes bajo esta modalidad por 6 meses y disfrutes de los beneficios de ser teletrabajador."
                },
                new QuestionsModel
                {
                    name = "¿Cómo doy inicio al proceso y conozco los avances del mismo?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿Cómo doy inicio al proceso y conozco los avances del mismo?\n\n".ToUpper() + "1. Después de participar en la sensibilización, conocer lo que implica teletrabajar y estar de acuerdo, recibirás un correo nombrado el ABC del teletrabajo donde te recordarán el paso a paso del proceso.\n" + "2. Recibirás también una citación para la entrevista y un correo de Psigma donde te asignan las pruebas a desarrollar.\n" + "3. Una vez ejecutes estas actividades, tu líder recibirá esta información y posteriormente será compartida contigo.\n" + "4. Si la modalidad de teletrabajo que vas a desarrollar (suplementario o autónomo) lo requiere, tendrás una validación virtual, la cual puede ser desarrollada por el proveedor que se pone en contacto contigo para definir el espacio o por personal de la compañía.\n" + "5. De esta revisión también recibirás por correo un informe de las recomendaciones que debes implementar y las condiciones de tu espacio."
                },
                new QuestionsModel
                {
                    name = "¿En qué consisten las pruebas de competencias?",
                    subName = "Da clic en VER RESPUESTA.",
                    imageUrl = "https://images.pexels.com/photos/356079/pexels-photo-356079.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                    information = "¿En qué consisten las pruebas de competencias?\n\n".ToUpper() + "1. Revisar la recepción de una notificación de Psigma Corporation, al correo. Cada usuario recibe un enlace, usuario y contraseña para el desarrollo de la prueba.\n" + "2. Tiene una duración de una hora.  Realizar la prueba en el espacio separado.\n" + "3. La prueba tiene un costo, en caso de no realizarla correctamente o que esta se deshabilite, se deberá comprar otra prueba.\n" + "4. Diligenciar todos los campos solicitados dentro de la prueba.\n" + "5. Desarrollarla de manera continua,  esta no puede suspenderse, por esto se reserva una hora en la agenda de los candidatos.\n" + "6. La prueba no determina respuestas correctas o incorrectas.\n" + "7. Elegir una opción dentro del cuestionario de la prueba, donde cada persona debe indicar con cuál opción se identifica  más y con cuál menos.\n" + "8. Es fundamental  que esta se desarrolle con completa honestidad."
                }
            };
            return list;
        }
        private IActivity ShowQuestions(List<QuestionsModel> listQuestions)
        {
            var optionsAttachments = new List<Attachment>();

            foreach (var item in listQuestions)
            {
                var card = new HeroCard
                {
                    Title = item.name,
                    Subtitle = item.subName,
                    Images = new List<CardImage> { new CardImage(item.imageUrl) },
                    Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Respuesta", Value = item.information, Type = ActionTypes.ImBack}
                }
                };
                optionsAttachments.Add(card.ToAttachment());
            }
            
            
            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;
        }

        private async Task IntentNone(ITurnContext<IMessageActivity> turnContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync("Lo siento, no entiendo lo que dices.", cancellationToken: cancellationToken);
        }
    }
}
