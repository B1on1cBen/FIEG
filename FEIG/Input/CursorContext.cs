using System;
using System.Collections.Generic;

namespace FEIG.Input
{
    // Tells the cursor what it's function is when keys are pressed, 
    // within a certain context. I.E., Map context and action bar context
    public class CursorContext
    {
        public Dictionary<CursorInput, CursorAction> keys;

        // Stores delegate function calls for each key so that any 
        // context can call any function with any key
        public CursorContext(Dictionary<CursorInput, CursorAction> keys)
        {
            this.keys = keys;
        }
    }
}
