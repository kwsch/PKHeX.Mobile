namespace PKHeX.Mobile.Logic
{
    public static class BindingUtil
    {
        public static int GetValue(object val)
        {
            if (val is int i)
                return i;
            return int.TryParse(val as string, out int v) ? v : 0;
        }
    }
}
