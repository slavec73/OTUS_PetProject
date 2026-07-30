using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Models.DbModels
{
    public class VacationApproval
    {
        [Key]
        public Guid VacationApprovalId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid VacationRequestId { get; set; }

        [ForeignKey(nameof(VacationRequestId))]
        public VacationRequest? VacationRequest { get; set; }

        [Required]
        public int ApprovalStage { get; set; }

        [Required]
        public Guid ApproverUserId { get; set; }

        [ForeignKey(nameof(ApproverUserId))]
        public User? ApproverUser { get; set; }

        [Required]
        public VacationRequestStatus Decision { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
    }

}
