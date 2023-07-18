using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataAccessLayer.Models;

namespace WebCrawler.BusinessLayer
{
    public static class EnumExtensions
    {
        public static string EnumToString(this ExecutionStatus status)
        {
            switch (status)
            {
                case ExecutionStatus.Executing:
                    return "executing";
                case ExecutionStatus.Executed:
                    return "executed";
                default:
                    return "created";
            }
        }

        public static string EnumToString(this ExecutionStatus? status)
        {
            switch (status)
            {
                case ExecutionStatus.Executing:
                    return "executing";
                case ExecutionStatus.Executed:
                    return "executed";
                default:
                    return "created";
            }
        }

        public static ExecutionStatus StringToEnum(this string status)
        {
            switch (status)
            {
                case "executing":
                    return ExecutionStatus.Executing;
                case "executed":
                    return ExecutionStatus.Executed;
                default:
                    return ExecutionStatus.Created;
            }
        }
    }
}
