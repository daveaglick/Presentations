namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    public class InteractiveDiagnostic
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<InteractiveDiagnosticMethod> Methods { get; set; }
    }
}