using System;

namespace AutoBotUtilities
{
    class SubmitEntryData
    {
        public SubmitEntryData()
        {
        }

        public string CNumber { get; internal set; }
        public string ReferenceNumber { get; internal set; }
        public string DocumentType { get; internal set; }
        public string CustomsProcedure { get; internal set; }
        public DateTime? RegistrationDate { get; internal set; }
        public DateTime? AssessmentDate { get; internal set; }
    }
}