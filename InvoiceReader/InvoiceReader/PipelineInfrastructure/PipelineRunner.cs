// Added using directive

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class PipelineRunner<TContext>
    {
        private readonly IEnumerable<IPipelineStep<TContext>> _steps;

        public PipelineRunner(IEnumerable<IPipelineStep<TContext>> steps)
        {
            _steps = steps;
        }

        public async Task<bool> Run(TContext context)
        {
            foreach (var step in _steps)
            {
                if (!await step.Execute(context).ConfigureAwait(false))
                {
                    // Stop the pipeline if a step returns false
                    return false;
                }
            }
            return true; // Indicate pipeline completed successfully
        }
    }
}
