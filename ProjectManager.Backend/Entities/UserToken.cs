namespace ProjectManager.Backend.Entities; 

public class UserToken {
    public string TokenId { get; set; }
    public string UserId { get; set; }
    public string ClientIp { get; set; }
    public DateTime Created { get; set; }
}