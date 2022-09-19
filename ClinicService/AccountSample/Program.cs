using ClinicService.Utils;

namespace AccountSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = PasswordUtils.CreatePasswordHash("12345");
            Console.WriteLine(result.PasswordSalt);
            Console.WriteLine(result.PasswordHash);
            Console.ReadKey();
        }
    }
}