using NUnit.Framework;
using Tessera.Core.Messages;
using Tessera.Internal;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TesseraEffectsPeriodicApiErgonomicsTests
{
    [Test]
    public async Task TesseraEffects_Periodic_AutoReschedulesWithoutAppSelfScheduling()
    {
        var app = new PeriodicCounterApp();
        var effect = app.InitializeRuntime();

        Assert.That(effect, Is.Not.Null);

        for (var iteration = 1; iteration <= 3; iteration++)
        {
            var emitted = await effect!(CancellationToken.None);
            Assert.That(emitted, Is.TypeOf<TesseraPeriodicEffectMessage>());

            effect = app.UpdateRuntime(emitted!);
            Assert.That(effect, Is.Not.Null);
            Assert.That(app.PeriodicUpdateCount, Is.EqualTo(iteration));
        }
    }

    [Test]
    public async Task TesseraEffects_TickAndEvery_RemainSingleShotWithoutRuntimeAutoReschedule()
    {
        var app = new PassiveUpdateApp();

        var tick = TesseraEffects.Tick(TimeSpan.FromMilliseconds(1), _ => new TickPingMessage());
        var tickMessage = await tick(CancellationToken.None);
        var tickFollowUp = app.UpdateRuntime(tickMessage!);

        var every = TesseraEffects.Every(TimeSpan.FromMilliseconds(1), _ => new EveryPingMessage());
        var everyMessage = await every(CancellationToken.None);
        var everyFollowUp = app.UpdateRuntime(everyMessage!);

        Assert.That(tickFollowUp, Is.Null);
        Assert.That(everyFollowUp, Is.Null);
        Assert.That(app.TickCount, Is.EqualTo(1));
        Assert.That(app.EveryCount, Is.EqualTo(1));
    }

    [Test]
    public async Task TesseraEffects_Periodic_UpdateRuntimeAddsSinglePeriodicFollowUp()
    {
        var app = new PeriodicWithAppEffectApp();
        var initial = app.InitializeRuntime();
        Assert.That(initial, Is.Not.Null);

        var periodicEnvelope = await initial!(CancellationToken.None);
        var combinedEffect = app.UpdateRuntime(periodicEnvelope!);
        Assert.That(combinedEffect, Is.Not.Null);

        var coreCombined = TesseraEffectAdapter.ToCore(combinedEffect);
        Assert.That(coreCombined, Is.Not.Null);

        var coreEnvelope = await coreCombined!(CancellationToken.None);
        Assert.That(coreEnvelope, Is.TypeOf<BatchMsg>());

        var batch = (BatchMsg)coreEnvelope!;
        Assert.That(batch.Effects.Count, Is.EqualTo(2));

        var emittedMessages = new List<Message>();
        foreach (var effect in batch.Effects)
        {
            var produced = await effect(CancellationToken.None);
            if (produced is not null)
            {
                emittedMessages.Add(TesseraMessageAdapter.ToPublic(produced));
            }
        }

        Assert.That(emittedMessages.Count(message => message is AppFollowUpMessage), Is.EqualTo(1));
        Assert.That(emittedMessages.Count(message => message is TesseraPeriodicEffectMessage), Is.EqualTo(1));
    }

    private sealed record PeriodicPayloadMessage : Message;

    private sealed record TickPingMessage : Message;

    private sealed record EveryPingMessage : Message;

    private sealed record AppFollowUpMessage : Message;

    private sealed class PeriodicCounterApp : TesseraApp
    {
        public int PeriodicUpdateCount { get; private set; }

        public override TesseraEffect? Initialize() =>
            TesseraEffects.Periodic(TimeSpan.FromMilliseconds(1), _ => new PeriodicPayloadMessage());

        public override TesseraEffect? Update(Message message)
        {
            if (message is PeriodicPayloadMessage)
            {
                PeriodicUpdateCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From("periodic");
    }

    private sealed class PassiveUpdateApp : TesseraApp
    {
        public int TickCount { get; private set; }

        public int EveryCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is TickPingMessage)
            {
                TickCount++;
            }
            else if (message is EveryPingMessage)
            {
                EveryCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From("passive");
    }

    private sealed class PeriodicWithAppEffectApp : TesseraApp
    {
        public override TesseraEffect? Initialize() =>
            TesseraEffects.Periodic(TimeSpan.FromMilliseconds(1), _ => new PeriodicPayloadMessage());

        public override TesseraEffect? Update(Message message)
        {
            return message is PeriodicPayloadMessage
                ? TesseraEffects.Emit(new AppFollowUpMessage())
                : null;
        }

        public override Screen Build(ScreenContext context) => Screen.From("batch");
    }
}
