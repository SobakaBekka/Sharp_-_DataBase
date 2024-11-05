using System;

namespace OnlineSupermarket.Models
{
    public class UserAuditLog
    {
        public int UserID { get; set; }
        public int OldRoleID { get; set; }
        public int NewRoleID { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}