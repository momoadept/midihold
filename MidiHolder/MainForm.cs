using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MidiHolder.Models;
using Color = System.Drawing.Color;

namespace MidiHolder
{
    public partial class MainForm : Form
    {
        private IList<MidiDeviceLookup> inputs;
        private IList<MidiDeviceLookup> outputs;
        private MidiProcessor midi;

        private bool passthroughOn;
        private bool holdOn;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            btnHold.Enabled = passthroughOn;

            midi = new MidiProcessor(new Feed(list =>
            {
                txtDebug.Lines = list.ToArray();
                txtDebug.SelectionStart = txtDebug.TextLength;
                txtDebug.ScrollToCaret();
            }));
            inputs = midi.GetInputs();
            outputs = midi.GetOutputs();

            lstInputs.Items.Clear();
            lstInputs.Items.AddRange(inputs.ToArray());

            lstOutputs.Items.Clear();
            lstOutputs.Items.AddRange(outputs.ToArray());

            btnPassthrough.Click += (sender, args) => TogglePassthrough();
            btnHold.Click += (sender, args) => ToggleHold();
            btnRelease.Click += (sender, args) => midi?.ReleseAllNoteOffs();
        }

        private void TogglePassthrough()
        {
            if (holdOn)
            {
                return;
            }

            if (passthroughOn)
            {
                DisablePassthrough();
            }
            else
            {
                EnablePassthrough();
            }

            btnHold.Enabled = passthroughOn;
            lstInputs.Enabled = lstOutputs.Enabled = !passthroughOn;
        }

        private void DisablePassthrough()
        {
            passthroughOn = false;
            midi.Passthrough = false;
            midi.StopListen();
            btnPassthrough.ResetBackColor();
        }

        private void EnablePassthrough()
        {
            var selectedInput = lstInputs.SelectedItem as MidiDeviceLookup;
            var selectedOutput = lstOutputs.SelectedItem as MidiDeviceLookup;
            if (selectedOutput == null || selectedInput == null)
            {
                return;
            }

            passthroughOn = true;
            midi.Passthrough = true;
            midi.StartListen(selectedInput, selectedOutput);
            btnPassthrough.BackColor = Color.Green;
        }

        private void ToggleHold()
        {
            if (holdOn)
            {
                DisableHold();
            }
            else
            {
                EnableHold();
            }

            btnRelease.Enabled = holdOn;
        }

        private void EnableHold()
        {
            holdOn = true;
            btnHold.BackColor = Color.Green;
            midi.Hold = true;
        }

        private void DisableHold()
        {
            holdOn = false;
            btnHold.ResetBackColor();
            midi.Hold = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            midi?.Dispose();
            base.Dispose(disposing);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            midi.ReleaseDelayMs = (int) nmrRelease.Value;
        }
    }
}
