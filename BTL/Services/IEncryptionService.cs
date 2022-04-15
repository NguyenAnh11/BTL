namespace BTL.Services
{
    public interface IEncryptionService
    {
        string CreateSaltKey(int size);
        string CreatePasswordHash(string password, string saltKey, string hashAlgorithm);
    }
}
