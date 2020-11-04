using Ludiq;
using Minis;
using UnityEngine;

namespace Bolt.Addons.Minis {

[UnitCategory("MIDI"), UnitTitle("MIDI Note")]
public sealed class MidiNoteUnit : Unit, IGraphElementWithData
{
    #region Data class

    public sealed class Data : IGraphElementData
    {
        public MidiDevice Device { get; private set; }

        public bool CheckDevice(int channel)
        {
            if (Device != null && Device.channel == channel) return true;
            Device = DeviceQuery.FindChannel(channel);
            return Device != null;
        }
    }

    public IGraphElementData CreateData() => new Data();

    #endregion

    #region Unit I/O

    [DoNotSerialize]
    public ValueInput Channel { get; private set; }

    [DoNotSerialize]
    public ValueInput NoteNumber { get; private set; }

    [DoNotSerialize]
    public ValueOutput Velocity { get; private set; }

    #endregion

    #region Unit implementation

    protected override void Definition()
    {
        Channel = ValueInput<int>(nameof(Channel), 0);
        NoteNumber = ValueInput<int>(nameof(NoteNumber), 0);
        Velocity = ValueOutput<float>(nameof(Velocity), GetVelocity);
    }

    float GetVelocity(Flow flow)
    {
        var data = flow.stack.GetElementData<Data>(this);
        var vChannel = flow.GetValue<int>(Channel);
        if (!data.CheckDevice(vChannel)) return 0;
        var vNoteNumber = flow.GetValue<int>(NoteNumber);
        return data.Device.GetNote(vNoteNumber).velocity;
    }

    #endregion
}

} // Bolt.Addons.Minis