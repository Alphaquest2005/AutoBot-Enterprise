using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public enum LogCategory
    {
        Undefined = 0,         // Default or unspecified
        MethodBoundary = 1,    // For METHOD_ENTRY, METHOD_EXIT_SUCCESS, METHOD_EXIT_FAILURE
        ActionBoundary = 2,    // For ACTION_START, ACTION_END_SUCCESS, ACTION_END_FAILURE
        InternalStep = 3,      // For granular steps within methods/actions
        ExternalCall = 4,      // For EXTERNAL_CALL_START, EXTERNAL_CALL_END
        DiagnosticDetail = 5,  // For general Debug/Verbose messages not fitting other categories
        Performance = 6,       // For performance-specific markers
        StateChange = 7,       // For logging significant state changes
        Security = 8,          // For security-relevant events (audit, authN, authZ)
        MetaLog = 9            // For META_LOG_DIRECTIVEs if they pass through the same system
    }
}