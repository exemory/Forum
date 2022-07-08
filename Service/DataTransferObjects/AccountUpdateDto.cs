namespace Service.DataTransferObjects
{
    public class AccountUpdateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CurrentPassword { get; set; }
    }
}