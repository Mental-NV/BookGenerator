using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace BookGenerator.Client.ApiServices
{
    public class ApiException : Exception
    {
        public ApiException(ProblemDetails problemDetails)
            : base(ToMessage(problemDetails))
        {
        }

        static string ToMessage(ProblemDetails problemDetails)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(problemDetails.Title))
            {
                sb.AppendLine(problemDetails.Title);
            }
            if (!string.IsNullOrWhiteSpace(problemDetails.Detail))
            {
                sb.AppendLine(problemDetails.Detail);
            }
            if (!string.IsNullOrWhiteSpace(problemDetails.Type))
            {
                sb.AppendLine(problemDetails.Type);
            }
            if (problemDetails.Status.HasValue)
            {
                sb.AppendLine($"Status: {problemDetails.Status.Value}");
            }
            if (problemDetails.Extensions != null)
            {
                foreach (var extension in problemDetails.Extensions)
                { 
                    sb.AppendLine($"{extension.Key}:{extension.Value}"); 
                }
            }
            return sb.ToString();
        }
    }
}
