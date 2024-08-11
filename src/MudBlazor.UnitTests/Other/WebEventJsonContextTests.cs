// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;
using ErrorEventArgs = Microsoft.AspNetCore.Components.Web.ErrorEventArgs;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class WebEventJsonContextTests
    {
        public class TestEventArgs : EventArgs
        {
            public int Id { get; set; }
        }

        [Test]
        public void TestEventArgsDeserializationFail()
        {
            var evenType = typeof(TestEventArgs);
            var eventArgs = new TestEventArgs
            {
                Id = 15
            };

            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var action = () =>
            {
                JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            };
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ChangeEventArgsDeserialization()
        {
            var evenType = typeof(ChangeEventArgs);
            var eventArgs = new ChangeEventArgs
            {
                Value = "string"
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            // Special case for ChangeEventArgs because its value type can be one of
            // several types, and System.Text.Json doesn't pick types dynamically
            var @event = DeserializeChangeEventArgs(eventData, new WebEventJsonContext());
            @event?.Value.Should().Be(@event.Value);
        }

        [Test]
        public void DragEventArgsDeserialization()
        {
            var evenType = typeof(DragEventArgs);
            var eventArgs = new DragEventArgs
            {
                DataTransfer = new DataTransfer()
                {
                    DropEffect = "Effect",
                    Items = new DataTransferItem[]
                    {
                        new()
                        {
                            Type = "TestType"
                        }
                    }
                },
                Type = "TestType",
                ClientX = 10,
                ClientY = 20,
                AltKey = true,
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (DragEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.ClientX.Should().Be(eventArgs.ClientX);
            @event?.ClientY.Should().Be(eventArgs.ClientY);
            @event?.AltKey.Should().Be(eventArgs.AltKey);
            @event?.DataTransfer.DropEffect.Should().Be("Effect");
            @event?.DataTransfer.Items.Should().HaveCount(1);
        }

        [Test]
        public void KeyboardEventArgsDeserialization()
        {
            var evenType = typeof(KeyboardEventArgs);
            var eventArgs = new KeyboardEventArgs
            {
                Type = "TestType",
                Key = "A"
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (KeyboardEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.Key.Should().Be(eventArgs.Key);
        }

        [Test]
        public void PointerEventArgsDeserialization()
        {
            var evenType = typeof(PointerEventArgs);
            var eventArgs = new PointerEventArgs
            {
                Type = "TestType",
                Height = 100,
                IsPrimary = true
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (PointerEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.Height.Should().Be(eventArgs.Height);
            @event?.IsPrimary.Should().Be(eventArgs.IsPrimary);
        }

        [Test]
        public void ProgressEventArgsDeserialization()
        {
            var evenType = typeof(ProgressEventArgs);
            var eventArgs = new ProgressEventArgs
            {
                Type = "TestType",
                Loaded = 100,
                LengthComputable = true
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (ProgressEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.Loaded.Should().Be(eventArgs.Loaded);
            @event?.LengthComputable.Should().Be(eventArgs.LengthComputable);
        }

        [Test]
        public void TouchEventArgsDeserialization()
        {
            var evenType = typeof(TouchEventArgs);
            var eventArgs = new TouchEventArgs
            {
                Type = "TestType",
                TargetTouches = new[]
                {
                    new TouchPoint()
                    {
                        ClientX = 0
                    },
                    new TouchPoint()
                    {
                        ClientX = 10
                    }
                }
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (TouchEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.TargetTouches.Should().HaveCount(2);
        }

        [Test]
        public void WheelEventArgsDeserialization()
        {
            var evenType = typeof(WheelEventArgs);
            var eventArgs = new WheelEventArgs
            {
                Type = "TestType",
                ShiftKey = true
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (WheelEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.ShiftKey.Should().Be(eventArgs.ShiftKey);
        }

        [Test]
        public void FocusEventArgsDeserialization()
        {
            var evenType = typeof(FocusEventArgs);
            var eventArgs = new FocusEventArgs
            {
                Type = "TestType"
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (FocusEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
        }

        [Test]
        public void ErrorEventArgsDeserialization()
        {
            var evenType = typeof(ErrorEventArgs);
            var eventArgs = new ErrorEventArgs
            {
                Type = "TestType",
                Colno = 10,
                Message = "SomeMessage"
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (ErrorEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.Colno.Should().Be(eventArgs.Colno);
            @event?.Message.Should().Be(eventArgs.Message);
        }

        [Test]
        public void MouseEventArgsDeserialization()
        {
            var evenType = typeof(MouseEventArgs);
            var eventArgs = new MouseEventArgs
            {
                Type = "TestType",
                ClientX = 10,
                ClientY = 20,
                AltKey = true,
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (MouseEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
            @event?.ClientX.Should().Be(eventArgs.ClientX);
            @event?.ClientY.Should().Be(eventArgs.ClientY);
            @event?.AltKey.Should().Be(eventArgs.AltKey);
        }

        [Test]
        public void ClipboardEventArgsDeserialization()
        {
            var evenType = typeof(ClipboardEventArgs);
            var eventArgs = new ClipboardEventArgs
            {
                Type = "TestType"
            };
            var eventData = JsonSerializer.Serialize(eventArgs, evenType);
            var @event = (ClipboardEventArgs)JsonSerializer.Deserialize(eventData, evenType, new WebEventJsonContext());
            @event?.Type.Should().Be(eventArgs.Type);
        }

        private static ChangeEventArgs DeserializeChangeEventArgs(string eventArgsJson, WebEventJsonContext jsonSerializerContext)
        {
            var changeArgs = JsonSerializer.Deserialize(eventArgsJson, jsonSerializerContext.ChangeEventArgs);
            var jsonElement = (JsonElement)changeArgs?.Value!;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Null:
                    changeArgs.Value = null;
                    break;
                case JsonValueKind.String:
                    changeArgs.Value = jsonElement.GetString();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    changeArgs.Value = jsonElement.GetBoolean();
                    break;
                default:
                    throw new ArgumentException($"Unsupported {nameof(ChangeEventArgs)} value {jsonElement}.");
            }
            return changeArgs;
        }
    }
}
