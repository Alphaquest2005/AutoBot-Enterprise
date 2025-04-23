using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class PipelineRunner<TContext>
    {
        private static readonly ILogger _logger = Log.ForContext<PipelineRunner<TContext>>();

        private readonly IReadOnlyList<IPipelineStep<TContext>> _steps;
        private readonly string _pipelineName;

        public PipelineRunner(IEnumerable<IPipelineStep<TContext>> steps, string pipelineName = "Unnamed Pipeline")
        {
            _steps = steps?.ToList() ?? new List<IPipelineStep<TContext>>();
            _pipelineName = pipelineName;
            LogPipelineInitialization();
        }

        public async Task<bool> Run(TContext context)
        {
            if (!ValidateContext(context))
                return false;

            _logger.Information("Starting execution of {PipelineName}.", _pipelineName);

            int stepCounter = 0;
            bool continuePipeline = true;

            foreach (var step in _steps)
            {
                stepCounter++;
                if (!ProcessStep(step, context, stepCounter, ref continuePipeline))
                    break;
            }

            LogPipelineCompletion(continuePipeline, stepCounter);
            return continuePipeline;
        }

        private bool ProcessStep(IPipelineStep<TContext> step, TContext context, int stepCounter, ref bool continuePipeline)
        {
            string stepName = GetStepName(step, stepCounter);

            if (step == null)
            {
                LogNullStepWarning(stepCounter);
                return true; // Continue to the next step
            }

            return ExecuteStep(step, context, stepName, stepCounter, ref continuePipeline);
        }

        private void LogPipelineInitialization()
        {
            _logger.Debug("{PipelineName} runner initialized with {StepCount} steps.", _pipelineName, _steps.Count);
            if (_steps.Count == 0)
            {
                LogZeroStepsWarning();
            }
        }

        private void LogZeroStepsWarning()
        {
            _logger.Warning("{PipelineName} runner initialized with zero steps.", _pipelineName);
        }

        private bool ValidateContext(TContext context)
        {
            if (context == null)
            {
                LogNullContextError();
                return false;
            }
            return true;
        }

        private void LogNullContextError()
        {
            _logger.Error("{PipelineName} cannot run with a null context.", _pipelineName);
        }



        private void LogNullStepWarning(int stepCounter)
        {
            _logger.Warning("Skipping null step at position {StepNumber} in {PipelineName}.", stepCounter, _pipelineName);
        }

        private bool ExecuteStep(IPipelineStep<TContext> step, TContext context, string stepName, int stepCounter, ref bool continuePipeline)
        {
            try
            {
                LogStepExecutionStart(stepName);
                bool stepResult = step.Execute(context).ConfigureAwait(false).GetAwaiter().GetResult();
                return HandleStepResult(stepResult, stepName, stepCounter, ref continuePipeline);
            }
            catch (Exception ex)
            {
                LogStepExecutionError(ex, stepName, stepCounter);
                continuePipeline = false;
                return false; // Stop pipeline
            }
        }

        private void LogStepExecutionStart(string stepName)
        {
            _logger.Verbose("Executing step {StepName}...", stepName);
        }

        private void LogStepExecutionError(Exception ex, string stepName, int stepCounter)
        {
            _logger.Error(ex, "Error executing Step {StepNumber} ({StepName}) in {PipelineName}. Stopping pipeline execution.",
                stepCounter, stepName, _pipelineName);
        }

        private string GetStepName(IPipelineStep<TContext> step, int stepCounter)
        {
            string stepName = step?.GetType().Name ?? $"Unnamed Step {stepCounter}";
            LogStepProcessing(stepCounter, stepName);
            return stepName;
        }

        private void LogStepProcessing(int stepCounter, string stepName)
        {
            _logger.Verbose("Processing step {StepNumber}: {StepName} in {PipelineName}.", stepCounter, stepName, _pipelineName);
        }

        private bool HandleStepResult(bool stepResult, string stepName, int stepCounter, ref bool continuePipeline)
        {
            if (!stepResult)
            {
                LogStepFailure(stepName, stepCounter);
                continuePipeline = false;
                return false; // Stop pipeline
            }

            LogStepSuccess(stepName, stepCounter);
            return true; // Continue pipeline
        }

        private void LogStepFailure(string stepName, int stepCounter)
        {
            _logger.Warning("Step {StepNumber} ({StepName}) in {PipelineName} returned false. Stopping pipeline execution.",
                stepCounter, stepName, _pipelineName);
        }

        private void LogStepSuccess(string stepName, int stepCounter)
        {
            _logger.Information("Step {StepNumber} ({StepName}) in {PipelineName} completed successfully.",
                stepCounter, stepName, _pipelineName);
        }

        private void LogPipelineCompletion(bool continuePipeline, int stepCounter)
        {
            if (continuePipeline)
            {
                LogPipelineSuccess();
            }
            else
            {
                LogPipelinePrematureStop(stepCounter);
            }
        }

        private void LogPipelineSuccess()
        {
            _logger.Information("{PipelineName} completed all {StepCount} steps successfully.", _pipelineName, _steps.Count);
        }

        private void LogPipelinePrematureStop(int stepCounter)
        {
            _logger.Warning("{PipelineName} execution stopped prematurely after Step {StepNumber}.", _pipelineName, stepCounter);
        }
    }
}
