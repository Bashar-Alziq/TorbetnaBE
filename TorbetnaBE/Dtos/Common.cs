namespace TorbetnaBE.Dtos
{
    public class Common
    {
        public class PagedModel<T>
        {
            public int Total { get; set; }
            public int Limit { get; set; }
            public int Offset { get; set; }
            public IEnumerable<T> Matches { get; set; }
        }
    }
}
