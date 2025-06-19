using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class ExceptionFormatUtil
    {
        public static Exception GetFormated(DbUpdateException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.Entries
                .Select(entry => $"Entity of type {entry.Entity.GetType().Name} in state {entry.State} has validation issues.")
                .ToList();

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat("DbUpdateException ", ex.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbUpdateException with the improved exception message.
            return new DbUpdateException(exceptionMessage, ex);
        }
    }
}
