namespace Shared.ROP
{
    /// <summary>
    /// As ROP requires a type, this class simulates a "void" response.
    /// </summary>
    public sealed class Unit
    {
        public static readonly Unit Value = new Unit();
        private Unit() { }
    }
}
