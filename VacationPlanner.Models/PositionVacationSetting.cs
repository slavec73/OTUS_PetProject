namespace VacationPlanner.Models
{
    public class PositionVacationSetting
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public int VacationDays { get; set; }
    }
}
