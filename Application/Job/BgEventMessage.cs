namespace Application.Job;

[Serializable]
public sealed class BgEventMessage
{
    public string EventType { get; private set; }
    public string EventMessage { get; private set; }

    public BgEventMessage(string eventType, string eventMessage)
    {
        EventType = eventType;
        EventMessage = eventMessage;
    }

    public BgEventMessage(string message)
    {
        var s = message.Split(';');
        if (s.Length != 2)
            throw new Exception($"Message is incorrect: '{message}'");

        EventType = s[0];
        EventMessage = s[1];
    }

    public static BgEventMessage? TryJobMessage(string message)
    {
        try
        {
            var result = new BgEventMessage(message);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public override string ToString() => $"{EventType};{EventMessage}";
}