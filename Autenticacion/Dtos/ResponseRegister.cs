namespace Autenticacion.Dtos
{
    public class ResponseRegister
    {
        public string Token { get; set; }
        public DateTime Expiration{ get; set; }
    }
}
