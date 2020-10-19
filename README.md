# midihold

WinForms application implementing MIDI Hold feature. Useful for playing sustained chords live on your midi controller. You input a chord and it is sustained untill you play a next one. You have your hand(s) now free to play leads or fiddle with knobs. You also don't need to hold a sustain pedal.

Should work with anything

# How to

You would need to use LoopMidi (https://www.tobias-erichsen.de/software/loopmidi.html) to create virtual midi device if you want to send resulting MIDI to your DAW

0. Create a virtual MIDI port using LoopMidi
1. Download the latest release from this repo containing the executable
2. Run the thing
3. In the upper list select your MIDI keyboard
4. In the lower list select a virtual MIDI port created with LoopMidi
5. Pressing "Passthrough" will map your keyboard to a virtual port

If doesn't work, your DAW is probably keeping to itself the MIDI port you're trying to listen from. Should be fixed by starting MidiHolder before your DAW.

6. In your DAW, set MIDI In on your track to the virtual port created with LoopMidi. You should now be able to record MIDI as though from your controller directly
7. Now you can press "Hold" to enable Hold Mode. The app will sustain a chord you've played untill you play a new one. You can add any number of notes to a held chord if you are holding down at least one key.

# Plans

I've created this app for personal use. Unless I find any bugs that affect me, or this project gets public attention, I don't plan to update it further. Feel free to ask for features, but I can make no promises for now.

Though, there is a chance that I'll add a tool to only hold a specific zone on a keyboard sometime in the future.
