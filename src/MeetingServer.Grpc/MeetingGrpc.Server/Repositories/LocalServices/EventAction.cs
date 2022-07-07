using Action = MeetingProtobuf.Protos.Action;

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
        public static Action ToProtosAction(this EventAction action)
        {
            switch (action)
            {   
                case EventAction.Added:
                    return Action.Added;
                case EventAction.Removed:
                    return Action.Removed;
                case EventAction.Changed:
                    return Action.Changed;
            }
            throw new NotImplementedException();
        }

        public static EventAction ToEventAction(this Action action)
        {
            switch (action)
            {
                case Action.Added:
                    return EventAction.Added;
                case Action.Removed:
                    return EventAction.Removed;
                case Action.Changed:
                    return EventAction.Changed;
            }
            throw new NotImplementedException();
        }
    }
}
