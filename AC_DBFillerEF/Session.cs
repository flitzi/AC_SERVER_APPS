//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AC_DBFillerEF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Session
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Session()
        {
            this.Laps = new HashSet<Lap>();
            this.Results = new HashSet<Result>();
            this.Incidents = new HashSet<Incident>();
        }
    
        public int Id { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string Track { get; set; }
        public Nullable<short> LapCount { get; set; }
        public Nullable<int> Time { get; set; }
        public byte Ambient { get; set; }
        public byte Road { get; set; }
        public string Weather { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lap> Laps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Result> Results { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Incident> Incidents { get; set; }
    }
}
