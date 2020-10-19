using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiHolder.Models;
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;

namespace MidiHolder
{
    public class MidiProcessor : IDisposable
    {
        public bool Passthrough { get; set; }

        public bool Hold
        {
            get => hold;
            set
            {
                if (!value && bufferedNoteOffs.Any())
                {
                    ReleseAllNoteOffs();
                }

                hold = value;
            }
        }

        private InputDevice activeInput;
        private OutputDevice activeOutput;
        private Feed feed;

        private Dictionary<int, ChannelMessage> currentlyPressedNotes = new Dictionary<int, ChannelMessage>();
        private Queue<ChannelMessage> bufferedNoteOffs = new Queue<ChannelMessage>();
        private bool hold;

        public MidiProcessor(Feed feed)
        {
            this.feed = feed;
        }

        public IList<MidiDeviceLookup> GetInputs()
        {
            var result = new List<MidiDeviceLookup>();
            var inputCount = InputDevice.DeviceCount;
            for (int id = 0; id < inputCount; id++)
            {
                var capabilities = InputDevice.GetDeviceCapabilities(id);
                result.Add(new MidiDeviceLookup()
                {
                    Id = id,
                    Name = capabilities.name,
                });
            }

            return result;
        }

        public IList<MidiDeviceLookup> GetOutputs()
        {
            var result = new List<MidiDeviceLookup>();
            var outputCount = OutputDeviceBase.DeviceCount;
            for (int id = 0; id < outputCount; id++)
            {
                var capabilities = OutputDeviceBase.GetDeviceCapabilities(id);
                result.Add(new MidiDeviceLookup()
                {
                    Id = id,
                    Name = capabilities.name,
                });
            }

            return result;
        }

        public void StartListen(MidiDeviceLookup from, MidiDeviceLookup to)
        {
            activeInput = new InputDevice(from.Id);
            activeInput.MessageReceived += ActiveInputOnMessageReceived;

            activeOutput = new OutputDevice(to.Id);

            activeInput.StartRecording();
        }

        public void StopListen()
        {
            activeInput.StopRecording();
            activeInput?.Dispose();
            activeOutput?.Dispose();
            currentlyPressedNotes.Clear();
            bufferedNoteOffs.Clear();
        }

        private void ActiveInputOnMessageReceived(IMidiMessage message)
        {
            if (message is ChannelMessage channelMsg)
            {
                if (channelMsg.Command == ChannelCommand.NoteOff || channelMsg.Command == ChannelCommand.NoteOn)
                {
                    HandleNoteEvent(channelMsg);
                    return;
                }
            }

            if (Passthrough)
            {
                switch (message)
                {
                    case ChannelMessage m:
                        activeOutput?.Send(m);
                        break;
                    case SysCommonMessage m:
                        activeOutput?.Send(m);
                        break;
                    case SysExMessage m:
                        activeOutput?.Send(m);
                        break;
                    case SysRealtimeMessage m:
                        activeOutput?.Send(m);
                        break;
                }
            }
        }

        private void HandleNoteEvent(ChannelMessage message)
        {
            var note = (Note)(message.Data1 % 12);
            var octave = message.Data1 / 12;

            if (!Hold && Passthrough)
            {
                activeOutput?.Send(message);
            }

            if (Hold)
            {
                HandleHold(message);
            }
        }

        private void HandleHold(ChannelMessage message)
        {
            if (message.Command == ChannelCommand.NoteOn)
            {
                if (!currentlyPressedNotes.Any())
                {
                    ReleseAllNoteOffs();
                }

                currentlyPressedNotes.Add(message.Data1, message);
                activeOutput?.Send(message);
            }

            if (message.Command == ChannelCommand.NoteOff)
            {
                if (currentlyPressedNotes.ContainsKey(message.Data1))
                {
                    currentlyPressedNotes.Remove(message.Data1);
                    bufferedNoteOffs.Enqueue(message);
                }
                else
                {
                    activeOutput?.Send(message);
                }
            }

            feed.Set(new List<string>() { string.Join(", ", bufferedNoteOffs.Select(it => it.Data1).Select(NoteToString)) });
        }

        public void ReleseAllNoteOffs()
        {
            foreach (var noteOff in bufferedNoteOffs)
            {
                activeOutput?.Send(noteOff);
            }

            bufferedNoteOffs.Clear();

            feed.Set(Array.Empty<string>().ToList());
        }

        private string NoteToString(int note)
        {
            var key = (Note)(note % 12);
            var octave = note / 12;

            return $"{key}{octave}";
        }

        public void Dispose()
        {
            activeInput?.Dispose();
            activeOutput?.Dispose();
        }
    }
}
