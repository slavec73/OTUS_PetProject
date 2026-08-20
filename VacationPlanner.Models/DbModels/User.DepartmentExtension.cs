using System.ComponentModel.DataAnnotations.Schema;

namespace VacationPlanner.Models.DbModels
{

    public partial class User
    {
        /// <summary>Подразделение, к которому принадлежит сотрудник.</summary>
        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
    }
}