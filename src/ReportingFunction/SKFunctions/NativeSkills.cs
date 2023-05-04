using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportingFunction.SKFunctions
{
    public class NativeSkills
    {
        private readonly ILogger<NativeSkills> _logger;

        public NativeSkills(ILogger<NativeSkills> logger)
        {
            _logger = logger;
        }

        [SKFunction("Summarize the error and provide a recommendation")]
        [SKFunctionName("SummarizeError")]
        [SKFunctionContextParameter(Name = "Input", Description = "The input to summarize")]
        public async Task<string> Summarize(SKContext context)
        {
            var summarizeFunction = context.Func("SummarizeSkill", "Error");
            var summary = await summarizeFunction.InvokeAsync(context["input"], log: _logger);
            return summary.Result;
        }
    }
}
