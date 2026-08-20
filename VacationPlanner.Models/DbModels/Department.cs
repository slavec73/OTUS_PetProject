using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationPlanner.Models.DbModels
{
    /// <summary>
    /// Подразделение организации.
    /// У каждого подразделения есть руководитель (Manager) и сотрудники (Users).
    /// </summary>
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public User? Manager { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}