namespace VacationPlanner.Core.Notifications
{
    public class NotificationMessage
    {
        /// <summary>
        /// Кому отправляем
        /// </summary>
        public required IEnumerable<string> RecipientMails { get; set; }

        /// <summary>
        /// Тема сообщения
        /// </summary>
        public required string Subject { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public required string Body { get; set; }
    }
}
