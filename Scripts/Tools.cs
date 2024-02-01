namespace SuperMarioRehashed.Scripts;

public class Tools
{
    // https://stackoverflow.com/a/1082938
    // % in C# is NOT Modulo, it is remainder, this is BULLSHIT
    public static int Mod(int x, int m) {
        return (x%m + m)%m;
    }
}