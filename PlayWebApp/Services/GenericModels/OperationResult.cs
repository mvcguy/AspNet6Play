#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.GenericModels
{
    public class OperationResult
    {
        public OperationResult()
        {
            Errors = new List<Exception>();
        }

        public bool Succeeded { get; set; }

        public List<Exception> Errors { get; set; }

        public static OperationResult Success(string entityId = null)
        {
            return new OperationResult { Succeeded = true, EntityId = entityId};
        }

        public static OperationResult Failure(params Exception[] errors)
        {
            return new OperationResult
            {
                Succeeded = false,
                Errors = errors?.ToList()
            };
        }

        public string EntityId { get; set; }
    }

}