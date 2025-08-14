using TaskStatus = Taskify.Models.TaskStatus;

namespace Taskify.DTOs;

public record TaskItemDto {
        [JsonProperty("title")]
        public string Title {get; set;}
        [JsonProperty("description")]
        public string Description {get; set;}
        [JsonProperty("dueDate")]
        public DateTime? DueDate {get; set;}
        [JsonProperty("status")]
        public TaskStatus Status {get; set;}
        
        public int? UserId {get; set;}
        public User? User {get; set;}
}