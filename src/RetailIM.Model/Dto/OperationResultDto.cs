namespace RetailIM.Model.Dto
{
    public class OperationResultDto<T>
        where T : class
    {
        public string Error { get; set; }
        public T Result { get; set; }

        public OperationResultDto() { }

        public OperationResultDto(T result)
        {
            Result = result;
            Error = null;
        }
        public OperationResultDto(string error)
        {
            Result = null;
            Error = error;
        }
    }
}
