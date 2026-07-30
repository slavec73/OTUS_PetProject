using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Models.DbModels
{
    public class Vacation
    {
        [Key]
        public Guid VacationId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public Guid? VacationRequestId { get; set; }

        [ForeignKey(nameof(VacationRequestId))]
        public VacationRequest? VacationRequest { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        public int TotalDays => (DateTo - DateFrom).Days + 1;

        [Required]
        [MaxLength(200)]
        public string VacationType { get; set; } = "Annual";

        public bool IsPaid { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
