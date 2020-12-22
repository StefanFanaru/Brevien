namespace IdentityControl.API.Common
{
    public class SecretStamp
    {
         /// <summary>
         ///     Used to stamp secrets to make the ones shown in UI useless without it.
         /// </summary>
         public string Stamp { get; set; }
    }
}
