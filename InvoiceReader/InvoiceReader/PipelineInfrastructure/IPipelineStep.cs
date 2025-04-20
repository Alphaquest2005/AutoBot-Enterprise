namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public interface IPipelineStep<TContext>
    {
        Task<bool> Execute(TContext context);
    }
}