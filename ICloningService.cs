namespace Challenges
{
    public interface ICloningService
    {
         T Clone<T>(T source);
    }
}