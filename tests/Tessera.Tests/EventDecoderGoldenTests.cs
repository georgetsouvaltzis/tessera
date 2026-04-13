using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Tests;

internal static class EventDecoderGoldenTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Decoder_ArrowAndNavigationKeys_Parse",
            ArrowAndNavigationSequences_ParseExpectedKeys);
        yield return new TestCase("Decoder_EnhancedKeyboardSequences_Parse",
            EnhancedKeyboardSequences_ParseExpectedKeys);
        yield return new TestCase("Decoder_ControlByteKeys_Parse", ControlByteKeys_ParseExpectedKeys);
        yield return new TestCase("Decoder_AltControlSequences_Parse", AltControlSequences_ParseExpectedKeys);
        yield return new TestCase("Decoder_PasteBoundaryMarkers_Parse", PasteBoundaryMarkers_ParseExpectedMessages);
        yield return new TestCase("Decoder_FocusMarkers_Parse", FocusMarkers_ParseExpectedMessages);
        yield return new TestCase("Decoder_WindowResizeSequence_Parses", WindowResizeSequence_ParsesExpectedSize);
        yield return new TestCase("Decoder_ModeReportSequence_Parses", ModeReportSequence_ParsesExpectedMessage);
        yield return new TestCase("Decoder_MouseSequences_Parse", MouseSequences_ParseExpectedMessages);
        yield return new TestCase("Decoder_MouseTopLeftCoordinates_Parse",
            MouseTopLeftCoordinates_ParseExpectedZeroBasedValues);
        yield return new TestCase("Decoder_MouseExtendedSequences_Parse", MouseExtendedSequences_ParseExpectedMessages);
        yield return new TestCase("Decoder_OscSequences_ParseKnownCapabilityMessages",
            OscSequences_ParseKnownCapabilityMessages);
        yield return new TestCase("Decoder_OscClipboardSequence_IgnoresTrailingSegments",
            OscClipboardSequence_IgnoresTrailingSegments);
        yield return new TestCase("Decoder_DcsCapabilityResponse_Parses", DcsCapabilityResponse_Parses);
        yield return new TestCase("Decoder_DcsCapabilityResponse_InvalidHexFallsBackToRawText",
            DcsCapabilityResponse_InvalidHexFallsBackToRawText);
        yield return new TestCase("Decoder_KeyboardEnhancementReport_Parses", KeyboardEnhancementReport_Parses);
        yield return new TestCase("Decoder_PartialCsi_RequestsMoreDataUntilTimeout",
            PartialCsi_RequestsMoreDataUntilTimeout);
        yield return new TestCase("Decoder_Utf8Rune_ParsesWithoutReplacement", Utf8Rune_ParsesWithoutReplacement);
        yield return new TestCase("Decoder_Utf8Partial_RequestsMoreData", Utf8Partial_RequestsMoreData);
        yield return new TestCase("Decoder_UnknownSequence_ProducesUnknownMessage",
            UnknownSequence_ProducesUnknownMessage);
    }

    private static Task ArrowAndNavigationSequences_ParseExpectedKeys()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var up = Decode(decoder, "\e[A");
        var ctrlUp = Decode(decoder, "\e[1;5A");
        var ctrlDelete = Decode(decoder, "\e[3;5~");
        var pageUp = Decode(decoder, "\e[5~");
        var pageDown = Decode(decoder, "\e[6~");
        var home = Decode(decoder, "\eOH");
        var end = Decode(decoder, "\eOF");
        var backTab = Decode(decoder, "\e[Z");
        var f1 = Decode(decoder, "\eOP");
        var f4 = Decode(decoder, "\eOS");
        var f5 = Decode(decoder, "\e[15~");
        var ctrlF5 = Decode(decoder, "\e[15;5~");
        var f12 = Decode(decoder, "\e[24~");
        var altK = Decode(decoder, "\ek");

        // Assert
        AssertKey(up, KeyCode.Up);
        AssertKey(ctrlUp, KeyCode.Up, KeyModifiers.Ctrl);
        AssertKey(ctrlDelete, KeyCode.Delete, KeyModifiers.Ctrl);
        AssertKey(pageUp, KeyCode.PageUp);
        AssertKey(pageDown, KeyCode.PageDown);
        AssertKey(home, KeyCode.Home);
        AssertKey(end, KeyCode.End);
        AssertKey(backTab, KeyCode.Tab, KeyModifiers.Shift);
        AssertKey(f1, KeyCode.F1);
        AssertKey(f4, KeyCode.F4);
        AssertKey(f5, KeyCode.F5);
        AssertKey(ctrlF5, KeyCode.F5, KeyModifiers.Ctrl);
        AssertKey(f12, KeyCode.F12);
        AssertKey(altK, KeyCode.Character, KeyModifiers.Alt, "k");
        return Task.CompletedTask;
    }

    private static Task EnhancedKeyboardSequences_ParseExpectedKeys()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var modifyOtherCtrlShiftA = Decode(decoder, "\e[27;6;97~");
        var modifyOtherAltBracket = Decode(decoder, "\e[27;3;91~");
        var csiUCtrlK = Decode(decoder, "\e[107;5u");
        var csiUShiftTab = Decode(decoder, "\e[9;2u");
        var csiUEscape = Decode(decoder, "\e[27;1u");
        var csiURepeatK = Decode(decoder, "\e[107;5;2u");
        var csiUReleaseK = Decode(decoder, "\e[107;5;3u");
        var csiUReleaseKColon = Decode(decoder, "\e[107;5:3u");

        // Assert
        AssertKey(modifyOtherCtrlShiftA, KeyCode.Character, KeyModifiers.Shift | KeyModifiers.Ctrl, "a");
        AssertKey(modifyOtherAltBracket, KeyCode.Character, KeyModifiers.Alt, "[");
        AssertKey(csiUCtrlK, KeyCode.Character, KeyModifiers.Ctrl, "k");
        AssertKey(csiUShiftTab, KeyCode.Tab, KeyModifiers.Shift);
        AssertKey(csiUEscape, KeyCode.Escape);
        AssertKey(csiURepeatK, KeyCode.Character, KeyModifiers.Ctrl, "k", true);
        AssertKeyRelease(csiUReleaseK, KeyCode.Character, KeyModifiers.Ctrl, "k");
        AssertKeyRelease(csiUReleaseKColon, KeyCode.Character, KeyModifiers.Ctrl, "k");
        return Task.CompletedTask;
    }

    private static Task ControlByteKeys_ParseExpectedKeys()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var ctrlA = decoder.Decode([0x01], false);
        var ctrlK = decoder.Decode([0x0B], false);
        var ctrlBracket = decoder.Decode([0x1D], false);

        // Assert
        AssertKey(ctrlA, KeyCode.Character, KeyModifiers.Ctrl, "a");
        AssertKey(ctrlK, KeyCode.Character, KeyModifiers.Ctrl, "k");
        AssertKey(ctrlBracket, KeyCode.Character, KeyModifiers.Ctrl, "]");
        return Task.CompletedTask;
    }

    private static Task AltControlSequences_ParseExpectedKeys()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altBackspaceDel = decoder.Decode([0x1B, 0x7F], false);
        var altBackspaceCtrlH = decoder.Decode([0x1B, 0x08], false);
        var altCtrlA = decoder.Decode([0x1B, 0x01], false);
        var altTab = decoder.Decode([0x1B, 0x09], false);
        var altEscapePending = decoder.Decode([0x1B, 0x1B], false);
        var altEscape = decoder.Decode([0x1B, 0x1B], true);
        var altUp = decoder.Decode([0x1B, 0x1B, (byte)'[', (byte)'A'], false);

        // Assert
        AssertKey(altBackspaceDel, KeyCode.Backspace, KeyModifiers.Alt);
        AssertKey(altBackspaceCtrlH, KeyCode.Backspace, KeyModifiers.Alt);
        AssertKey(altCtrlA, KeyCode.Character, KeyModifiers.Alt | KeyModifiers.Ctrl, "a");
        AssertKey(altTab, KeyCode.Tab, KeyModifiers.Alt);
        TestAssert.True(altEscapePending.NeedMoreData,
            "Double escape should wait for timeout before resolving to alt+escape.");
        AssertConsumed(altEscapePending, 0);
        AssertKey(altEscape, KeyCode.Escape, KeyModifiers.Alt);
        AssertKey(altUp, KeyCode.Up, KeyModifiers.Alt);
        return Task.CompletedTask;
    }

    private static Task PasteBoundaryMarkers_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var pasteStart = Decode(decoder, "\e[200~");
        var pasteEnd = Decode(decoder, "\e[201~");

        // Assert
        AssertMessageType<PasteStartMsg>(pasteStart);
        AssertMessageType<PasteEndMsg>(pasteEnd);
        return Task.CompletedTask;
    }

    private static Task FocusMarkers_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var focusIn = Decode(decoder, "\e[I");
        var focusOut = Decode(decoder, "\e[O");

        // Assert
        AssertMessageType<FocusInMsg>(focusIn);
        AssertMessageType<FocusOutMsg>(focusOut);
        return Task.CompletedTask;
    }

    private static Task WindowResizeSequence_ParsesExpectedSize()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var resize = Decode(decoder, "\e[8;24;80t");

        // Assert
        AssertConsumed(resize, 10);
        if (resize.Message is not WindowSizeMsg { Width: 80, Height: 24 })
        {
            throw new InvalidOperationException("Expected WindowSizeMsg(80,24).");
        }

        return Task.CompletedTask;
    }

    private static Task ModeReportSequence_ParsesExpectedMessage()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var syncEnabled = Decode(decoder, "\e[?2026;1$y");
        var mouseUnsupported = Decode(decoder, "\e[?1006;0$y");

        // Assert
        AssertModeReport(syncEnabled, 2026, ModeReportState.Set);
        AssertModeReport(mouseUnsupported, 1006, ModeReportState.Unsupported);
        return Task.CompletedTask;
    }

    private static Task MouseSequences_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var sgrPress = Decode(decoder, "\e[<0;11;7M");
        var sgrRelease = Decode(decoder, "\e[<0;11;7m");
        var sgrMotion = Decode(decoder, "\e[<35;11;7M");
        var sgrWheel = Decode(decoder, "\e[<65;11;7M");
        var sgrCtrlClick = Decode(decoder, "\e[<16;11;7M");

        var x10Bytes = new byte[] { 0x1B, (byte)'[', (byte)'M', (byte)' ', (byte)'+', (byte)'&' };
        var x10Press = decoder.Decode(x10Bytes, false);

        // Assert
        AssertMouse<MouseClickMsg>(sgrPress, MouseEventType.Press, MouseButton.Left, 10, 6, KeyModifiers.None);
        AssertMouse<MouseReleaseMsg>(sgrRelease, MouseEventType.Release, MouseButton.Left, 10, 6, KeyModifiers.None);
        AssertMouse<MouseMotionMsg>(sgrMotion, MouseEventType.Motion, MouseButton.None, 10, 6, KeyModifiers.None);
        AssertMouse<MouseWheelMsg>(sgrWheel, MouseEventType.Wheel, MouseButton.WheelDown, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(sgrCtrlClick, MouseEventType.Press, MouseButton.Left, 10, 6, KeyModifiers.Ctrl);
        AssertMouse<MouseClickMsg>(x10Press, MouseEventType.Press, MouseButton.Left, 10, 5, KeyModifiers.None);

        return Task.CompletedTask;
    }

    private static Task MouseTopLeftCoordinates_ParseExpectedZeroBasedValues()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var sgrTopLeft = Decode(decoder, "\e[<0;1;1M");
        var x10TopLeft = decoder.Decode([0x1B, (byte)'[', (byte)'M', (byte)' ', (byte)'!', (byte)'!'],
            false);

        // Assert
        AssertMouse<MouseClickMsg>(sgrTopLeft, MouseEventType.Press, MouseButton.Left, 0, 0, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(x10TopLeft, MouseEventType.Press, MouseButton.Left, 0, 0, KeyModifiers.None);
        return Task.CompletedTask;
    }

    private static Task MouseExtendedSequences_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var wheelLeft = Decode(decoder, "\e[<66;11;7M");
        var wheelRight = Decode(decoder, "\e[<67;11;7M");
        var backward = Decode(decoder, "\e[<128;11;7M");
        var forward = Decode(decoder, "\e[<129;11;7M");
        var button10 = Decode(decoder, "\e[<130;11;7M");
        var button11 = Decode(decoder, "\e[<131;11;7M");
        var button12 = Decode(decoder, "\e[<132;11;7M");
        var button15 = Decode(decoder, "\e[<135;11;7M");
        var dragBackward = Decode(decoder, "\e[<160;11;7M");
        var dragButton12 = Decode(decoder, "\e[<164;11;7M");
        var shiftAltRight = Decode(decoder, "\e[<14;11;7M");
        var allModsWheelDown = Decode(decoder, "\e[<93;11;7M");
        var motionReportedAsRelease = Decode(decoder, "\e[<35;11;7m");

        var x10ReleaseBytes = new byte[] { 0x1B, (byte)'[', (byte)'M', (byte)'#', (byte)'2', (byte)'(' };
        var x10Release = decoder.Decode(x10ReleaseBytes, false);
        var x10WheelRightBytes = new byte[] { 0x1B, (byte)'[', (byte)'M', 99, (byte)'2', (byte)'(' };
        var x10WheelRight = decoder.Decode(x10WheelRightBytes, false);
        var x10BackwardDragBytes = new byte[] { 0x1B, (byte)'[', (byte)'M', 192, (byte)'2', (byte)'(' };
        var x10BackwardDrag = decoder.Decode(x10BackwardDragBytes, false);
        var x10Button12Bytes = new byte[] { 0x1B, (byte)'[', (byte)'M', 164, (byte)'2', (byte)'(' };
        var x10Button12 = decoder.Decode(x10Button12Bytes, false);
        var x10ShiftAltRightBytes = new byte[] { 0x1B, (byte)'[', (byte)'M', 46, (byte)'2', (byte)'(' };
        var x10ShiftAltRight = decoder.Decode(x10ShiftAltRightBytes, false);

        // Assert
        AssertMouse<MouseWheelMsg>(wheelLeft, MouseEventType.Wheel, MouseButton.WheelLeft, 10, 6, KeyModifiers.None);
        AssertMouse<MouseWheelMsg>(wheelRight, MouseEventType.Wheel, MouseButton.WheelRight, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(backward, MouseEventType.Press, MouseButton.Backward, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(forward, MouseEventType.Press, MouseButton.Forward, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(button10, MouseEventType.Press, MouseButton.Button10, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(button11, MouseEventType.Press, MouseButton.Button11, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(button12, MouseEventType.Press, MouseButton.Button12, 10, 6, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(button15, MouseEventType.Press, MouseButton.Button15, 10, 6, KeyModifiers.None);
        AssertMouse<MouseMotionMsg>(dragBackward, MouseEventType.Motion, MouseButton.Backward, 10, 6,
            KeyModifiers.None);
        AssertMouse<MouseMotionMsg>(dragButton12, MouseEventType.Motion, MouseButton.Button12, 10, 6,
            KeyModifiers.None);
        AssertMouse<MouseClickMsg>(shiftAltRight, MouseEventType.Press, MouseButton.Right, 10, 6,
            KeyModifiers.Shift | KeyModifiers.Alt);
        AssertMouse<MouseWheelMsg>(allModsWheelDown, MouseEventType.Wheel, MouseButton.WheelDown, 10, 6,
            KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Ctrl);
        AssertMouse<MouseMotionMsg>(motionReportedAsRelease, MouseEventType.Motion, MouseButton.None, 10, 6,
            KeyModifiers.None);
        AssertMouse<MouseReleaseMsg>(x10Release, MouseEventType.Release, MouseButton.None, 17, 7, KeyModifiers.None);
        AssertMouse<MouseWheelMsg>(x10WheelRight, MouseEventType.Wheel, MouseButton.WheelRight, 17, 7,
            KeyModifiers.None);
        AssertMouse<MouseMotionMsg>(x10BackwardDrag, MouseEventType.Motion, MouseButton.Backward, 17, 7,
            KeyModifiers.None);
        AssertMouse<MouseClickMsg>(x10Button12, MouseEventType.Press, MouseButton.Button12, 17, 7, KeyModifiers.None);
        AssertMouse<MouseClickMsg>(x10ShiftAltRight, MouseEventType.Press, MouseButton.Right, 17, 7,
            KeyModifiers.Shift | KeyModifiers.Alt);
        return Task.CompletedTask;
    }

    private static Task OscSequences_ParseKnownCapabilityMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var clipboard = decoder.Decode(
            [
                0x1B, (byte)']', (byte)'5', (byte)'2', (byte)';', (byte)'c', (byte)';', (byte)'a', (byte)'G', (byte)'V',
                (byte)'s', (byte)'b', (byte)'G', (byte)'8', (byte)'=', 0x07
            ],
            false);

        var foreground = decoder.Decode(
            [
                0x1B, (byte)']', (byte)'1', (byte)'0', (byte)';', (byte)'r', (byte)'g', (byte)'b', (byte)':', (byte)'a',
                (byte)'a', (byte)'/', (byte)'b', (byte)'b', (byte)'/', (byte)'c', (byte)'c', 0x1B, (byte)'\\'
            ],
            false);

        var background = decoder.Decode(
            [
                0x1B, (byte)']', (byte)'1', (byte)'1', (byte)';', (byte)'#', (byte)'1', (byte)'1', (byte)'2', (byte)'2',
                (byte)'3', (byte)'3', 0x07
            ],
            false);

        // Assert
        AssertConsumed(clipboard, 16);
        if (clipboard.Message is not ClipboardMsg { Content: "hello", Selection: 'c' })
        {
            throw new InvalidOperationException("Expected ClipboardMsg(content=hello, selection=c).");
        }

        AssertConsumed(foreground, 19);
        if (foreground.Message is not ForegroundColorMsg { Color: "#AABBCC" })
        {
            throw new InvalidOperationException("Expected ForegroundColorMsg(#AABBCC).");
        }

        AssertConsumed(background, 13);
        if (background.Message is not BackgroundColorMsg { Color: "#112233" })
        {
            throw new InvalidOperationException("Expected BackgroundColorMsg(#112233).");
        }

        return Task.CompletedTask;
    }

    private static Task DcsCapabilityResponse_Parses()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var result = decoder.Decode(
            [
                0x1B, (byte)'P', (byte)'1', (byte)'+', (byte)'r', (byte)'5', (byte)'4', (byte)'6', (byte)'3', (byte)'=',
                (byte)'3', (byte)'1', 0x1B, (byte)'\\'
            ],
            false);

        // Assert
        AssertConsumed(result, 14);
        if (result.Message is not CapabilityMsg { Name: "Tc", Value: "1" })
        {
            throw new InvalidOperationException("Expected CapabilityMsg(Tc=1).");
        }

        return Task.CompletedTask;
    }

    private static Task OscClipboardSequence_IgnoresTrailingSegments()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var result = decoder.Decode(
            [
                0x1B, (byte)']', (byte)'5', (byte)'2', (byte)';', (byte)'c', (byte)';', (byte)'a', (byte)'G', (byte)'V',
                (byte)'s', (byte)'b', (byte)'G', (byte)'8', (byte)'=', (byte)';', (byte)'i', (byte)'g', (byte)'n',
                (byte)'o', (byte)'r', (byte)'e', (byte)'d', 0x07
            ],
            false);

        // Assert
        AssertConsumed(result, 24);
        if (result.Message is not ClipboardMsg { Content: "hello", Selection: 'c' })
        {
            throw new InvalidOperationException("Expected ClipboardMsg(content=hello, selection=c).");
        }

        return Task.CompletedTask;
    }

    private static Task DcsCapabilityResponse_InvalidHexFallsBackToRawText()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var result = decoder.Decode(
            [
                0x1B, (byte)'P', (byte)'1', (byte)'+', (byte)'r', (byte)'Z', (byte)'Z', (byte)'=', (byte)'3', (byte)'1',
                0x1B, (byte)'\\'
            ],
            false);

        // Assert
        AssertConsumed(result, 12);
        if (result.Message is not CapabilityMsg { Name: "ZZ", Value: "1" })
        {
            throw new InvalidOperationException("Expected CapabilityMsg(ZZ=1) for invalid name hex fallback.");
        }

        return Task.CompletedTask;
    }

    private static Task KeyboardEnhancementReport_Parses()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var result = Decode(decoder, "\e[?3u");

        // Assert
        if (result.Message is not KeyboardEnhancementsMsg keyboard || keyboard.Flags != 3)
        {
            throw new InvalidOperationException("Expected KeyboardEnhancementsMsg flags=3.");
        }

        return Task.CompletedTask;
    }

    private static Task PartialCsi_RequestsMoreDataUntilTimeout()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var partial = Decode(decoder, "\e[1;");
        var timedOutPartial = Decode(decoder, "\e[1;", true);

        // Assert
        TestAssert.True(partial.NeedMoreData, "Partial CSI should request more data.");
        AssertConsumed(partial, 0);
        AssertConsumed(timedOutPartial, 1);
        AssertKey(timedOutPartial, KeyCode.Escape);
        return Task.CompletedTask;
    }

    private static Task UnknownSequence_ProducesUnknownMessage()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var unknown = Decode(decoder, "\e[999~");

        // Assert
        AssertMessageType<UnknownInputMsg>(unknown);
        return Task.CompletedTask;
    }

    private static Task Utf8Rune_ParsesWithoutReplacement()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var result = Decode(decoder, "გ");

        // Assert
        AssertKey(result, KeyCode.Character, KeyModifiers.None, "გ");
        return Task.CompletedTask;
    }

    private static Task Utf8Partial_RequestsMoreData()
    {
        // Arrange
        var decoder = new EventDecoder();
        var full = "გ"u8.ToArray();

        // Act
        var partial = decoder.Decode(full.AsSpan(0, 2), false);
        var complete = decoder.Decode(full, false);

        // Assert
        TestAssert.True(partial.NeedMoreData, "Partial UTF-8 rune should request more data.");
        AssertConsumed(partial, 0);
        AssertKey(complete, KeyCode.Character, KeyModifiers.None, "გ");
        return Task.CompletedTask;
    }

    private static DecodeResult Decode(EventDecoder decoder, string sequence, bool timeoutExpired = false)
    {
        return decoder.Decode(Encoding.UTF8.GetBytes(sequence), timeoutExpired);
    }

    private static void AssertMessageType<T>(DecodeResult result)
        where T : class, IMessage
    {
        if (result.Message is not T)
        {
            throw new InvalidOperationException(
                $"Expected {typeof(T).Name} but got {result.Message?.GetType().Name ?? "null"}.");
        }
    }

    private static void AssertConsumed(DecodeResult result, int expected)
    {
        if (result.Consumed != expected)
        {
            throw new InvalidOperationException($"Expected consumed={expected} but got {result.Consumed}.");
        }
    }

    private static void AssertKey(
        DecodeResult result,
        KeyCode keyCode,
        KeyModifiers modifiers = KeyModifiers.None,
        string text = "",
        bool isRepeat = false)
    {
        if (result.Message is not KeyPressMsg key)
        {
            throw new InvalidOperationException(
                $"Expected KeyPressMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != keyCode
            || key.Modifiers != modifiers
            || !string.Equals(key.Text, text, StringComparison.Ordinal)
            || key.IsRepeat != isRepeat)
        {
            throw new InvalidOperationException(
                $"Expected key(code={keyCode}, modifiers={modifiers}, text=\"{text}\", repeat={isRepeat}) " +
                $"but got key(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\", repeat={key.IsRepeat}).");
        }
    }

    private static void AssertKeyRelease(
        DecodeResult result,
        KeyCode keyCode,
        KeyModifiers modifiers = KeyModifiers.None,
        string text = "")
    {
        if (result.Message is not KeyReleaseMsg key)
        {
            throw new InvalidOperationException(
                $"Expected KeyReleaseMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != keyCode || key.Modifiers != modifiers ||
            !string.Equals(key.Text, text, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected release(code={keyCode}, modifiers={modifiers}, text=\"{text}\") " +
                $"but got release(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\").");
        }
    }

    private static void AssertMouse<TMouse>(
        DecodeResult result,
        MouseEventType eventType,
        MouseButton button,
        int x,
        int y,
        KeyModifiers modifiers)
        where TMouse : MouseMsg
    {
        if (result.Message is not TMouse mouse)
        {
            throw new InvalidOperationException(
                $"Expected {typeof(TMouse).Name} but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (mouse.EventType != eventType
            || mouse.Button != button
            || mouse.X != x
            || mouse.Y != y
            || mouse.Modifiers != modifiers)
        {
            throw new InvalidOperationException(
                $"Expected mouse(type={eventType}, button={button}, x={x}, y={y}, modifiers={modifiers}) " +
                $"but got mouse(type={mouse.EventType}, button={mouse.Button}, x={mouse.X}, y={mouse.Y}, modifiers={mouse.Modifiers}).");
        }
    }

    private static void AssertModeReport(DecodeResult result, int mode, ModeReportState state)
    {
        if (result.Message is not ModeReportMsg report)
        {
            throw new InvalidOperationException(
                $"Expected ModeReportMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (report.Mode != mode || report.State != state)
        {
            throw new InvalidOperationException(
                $"Expected mode-report(mode={mode}, state={state}) but got mode-report(mode={report.Mode}, state={report.State}).");
        }
    }
}
