
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Middleware
{
    public class TransactionMiddleware : IMiddleware
    {
        private readonly ExaminationContext _examinationContext;
        public TransactionMiddleware(ExaminationContext examinationContext)
        {
            _examinationContext = examinationContext;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //for get endpoints
            if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                // Skip transaction for GET requests
                await next(context);
                return;
            }

            using var transaction = await _examinationContext.Database.BeginTransactionAsync();
            try
            {
                await next(context);
                //await _examinationContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
