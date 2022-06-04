namespace MeetingGrpc.Server.Repositories.LocalServices
{
    public enum EventAction
    {
        Added,
        Removed,
        Changed
    }

    public static class ActionHelper
    {
        public static Protos.Action ToProtosAction(this EventAction action)
        {
            switch (action)
            {   
                case EventAction.Added:
                    return Protos.Action.Added;
                case EventAction.Removed:
                    return Protos.Action.Removed;
                case EventAction.Changed:
                    return Protos.Action.Changed;
            }
            throw new NotImplementedException();
        }

        public static EventAction ToEventAction(this Protos.Action action)
        {
            switch (action)
            {
                case Protos.Action.Added:
                    return EventAction.Added;
                case Protos.Action.Removed:
                    return EventAction.Removed;
                case Protos.Action.Changed:
                    return EventAction.Changed;
            }
            throw new NotImplementedException();
        }
    }
}
