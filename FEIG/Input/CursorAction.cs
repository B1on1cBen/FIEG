using System;

namespace FEIG.Input
{
    // Wraps a System.Action with a boolean
    // to enable / disable turbo for said action
    public class CursorAction
    {
        public Action action;
        public bool useTurbo;

        public CursorAction(Action action, bool useTurbo)
        {
            this.action = action;
            this.useTurbo = useTurbo;
        }
    }
}
