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
    
    public partial class Incident
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public byte Type { get; set; }
        public float RelativeSpeed { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public int DriverId1 { get; set; }
        public Nullable<int> DriverId2 { get; set; }
        public float WorldPosX { get; set; }
        public float WorldPosY { get; set; }
        public float WorldPosZ { get; set; }
    
        public virtual Session Session { get; set; }
        public virtual Driver Driver1 { get; set; }
        public virtual Driver Driver2 { get; set; }
    }
}
