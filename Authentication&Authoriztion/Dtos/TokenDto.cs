namespace Authentication_Authoriztion.Dtos
{
    public record TokenDto(string token, DateTime ExpiryDate);
}
