namespace Application.Utilities
{
    public class Result
    {
        public bool IsSuccessful { get; }
        public List<string> Errors { get; } = new();
        public int? Id { get; }

        private Result(bool isSuccessful, List<string> errors = null, int? id = null)
        {
            IsSuccessful = isSuccessful;
            Errors = errors ?? new List<string>();
            Id = id;
        }

        public static Result Failure(string error) => new Result(false, new List<string> { error });
        public static Result Success(int id) => new Result(true, id: id);
    }
}
